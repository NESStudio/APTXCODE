using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aptxcode.eMule
{
    class AICHHash : IDisposable
    {
        const int EMPARTSIZE = 9728000;
        const int EMBLOCKSIZE = 184320;

        public class AICHHashTree
        {
            public AICHHashTree LeftTree { get; set; }
            public AICHHashTree RightTree { get; set; }
            public long DataSize { get; set; }
            public long BaseSize { get; set; }
            byte[] _hash;

            public byte[] Hash
            {
                get
                {
                    if (_hash == null)
                    {
                        Debug.Assert(LeftTree != null && RightTree != null);
                        _hash = sha1.ComputeHash(LeftTree.Hash.Concat(RightTree.Hash).ToArray());
                    }
                    return _hash;
                }
                set
                {
                    _hash = value;
                }
            }
        }

        List<AICHHashTree> _blockHashes = new List<AICHHashTree>();
        int _currentIndex = 0;
        MemoryStream _currentBlockData = new MemoryStream(EMBLOCKSIZE);
        AICHHashTree _rootNode;
        string _rootHash;
        private static readonly SHA1 sha1 = SHA1.Create();
        public long FinishedSize { get; private set; }
        long _fileLength;
        public bool IsFinished
        {
            get { return FinishedSize == _fileLength; }
        }

        public string RootHash
        {
            get
            {
                if (string.IsNullOrEmpty(_rootHash))
                {
                    _rootHash = Base32Encode.Encode(_rootNode.Hash);
                }
                return _rootHash;
            }
        }

        public AICHHash(long fileLength)
        {
            _fileLength = fileLength;
            _rootNode = Create(0, fileLength, true);
        }

        public AICHHashTree Create(long startpos, long length, bool islefttree)
        {
            long blocks;
            AICHHashTree node = new AICHHashTree();
            node.DataSize = length;
            if (length > EMPARTSIZE)
            {
                node.BaseSize = EMPARTSIZE;
            }
            else if (length <= EMPARTSIZE && length > EMBLOCKSIZE)
            {
                node.BaseSize = EMBLOCKSIZE;
            }
            else if (length <= EMBLOCKSIZE && length > 0)
            {
                _blockHashes.Add(node);
                return node;
            }

            if (node.DataSize % node.BaseSize == 0)
            {
                blocks = node.DataSize / node.BaseSize;
            }
            else
            {
                blocks = node.DataSize / node.BaseSize + 1;
            }

            long leftsize, rightsize;
            if (blocks % 2 == 0)
            {
                leftsize = blocks / 2 * node.BaseSize;
            }
            else
            {
                if (islefttree)
                {
                    leftsize = (blocks + 1) / 2 * node.BaseSize;
                }
                else
                {
                    leftsize = blocks / 2 * node.BaseSize;
                }
            }
            rightsize = node.DataSize - leftsize;

            node.LeftTree = Create(startpos, leftsize, true);
            node.RightTree = Create(startpos + leftsize, rightsize, false);
            return node;
        }

        public void CalcAICH(byte[] buffer, int pos, int length)
        {
            int currentPos = pos;
            int posLimit = pos + length;
            while (currentPos < posLimit)
            {
                AICHHashTree currentNode = _blockHashes[_currentIndex];
                int writeCount = (int)Math.Min(length, currentNode.DataSize - _currentBlockData.Length);
                _currentBlockData.Write(buffer, currentPos, writeCount);
                length -= writeCount;
                currentPos += writeCount;
                FinishedSize += writeCount;
                if (_currentBlockData.Length <= currentNode.DataSize)
                {
                    _currentBlockData.Seek(0, SeekOrigin.Begin);
                    currentNode.Hash = sha1.ComputeHash(_currentBlockData);
                    _currentBlockData.SetLength(0);
                    _currentIndex++;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _currentBlockData.Close();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
