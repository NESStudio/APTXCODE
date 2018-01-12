using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace Aptxcode.BitTorrent
{
    class Torrent
    {
        //readonly string[] _greedland = new string[]
        //{
        //    "http://tk.greedland.net/announce",
        //    "http://tk2.greedland.net/announce"
        //};

        string[] _trackers;
        string _name;
        int? _picelength;
        string _comment;
        bool _isdir;
        bool _isprivate;
        long _total = 0;
        long _current = 0;
        BackgroundWorker worker;

        public BackgroundWorker Worker 
        { 
            set { worker = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackers">Tracker 列表</param>
        /// <param name="name">文件或目录名</param>
        /// <param name="blocksize">分块大小</param>
        /// <param name="comment">备注</param>
        /// <param name="isdir">是否是目录</param>
        /// <param name="isprivate">是否设为私有种子</param>
        public Torrent(string[] trackers, string name, int? blocksize, string comment, bool isprivate)
        {
            _trackers = trackers;
            _name = name;
            _picelength = blocksize * 1024;
            _comment = comment;
            _isdir = Directory.Exists(name);
            _isprivate = isprivate;
        }

        public void Create(out byte[] normal, out string magnet)//, out byte[] greedland)
        {
            SortedDictionary<string, object> root = new SortedDictionary<string, object>();
            string trs = string.Empty;
            if (_trackers != null && _trackers.Length > 0)
            {
                root.Add("announce", _trackers[0]);
                if (_trackers.Length > 1)
                {
                    ArrayList trackerlist = new ArrayList();
                    foreach(var tracker in _trackers)
                    {
                        ArrayList list = new ArrayList();
                        list.Add(tracker);
                        trackerlist.Add(list);
                        trs += string.Format("&tr={0}", System.Web.HttpUtility.UrlEncode(tracker));
                    }
                    root.Add("announce-list", trackerlist);
                }
            }
            if (!string.IsNullOrEmpty(_comment))
                root.Add("comment", _comment);
            root.Add("created by", "APTXCODE");////Excalibur
            TimeSpan unix = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            root.Add("creation date", (long)unix.TotalSeconds);
            root.Add("encoding", "UTF-8");
            SortedDictionary<string, object> info = new SortedDictionary<string, object>();
            if (_isdir)
            {
                DirectoryInfo dir = new DirectoryInfo(_name);
                info.Add("name", dir.Name);
                long fileslength = 0;
                List<string> filenames = new List<string>();
                ArrayList files = GetFiles(dir, dir.FullName, ref fileslength, ref filenames);
                info.Add("files", files);
                if (!_picelength.HasValue)
                {
                    _picelength = GetPieceLength(fileslength);
                }
                info.Add("piece length", _picelength.Value);
                _total = fileslength;
                byte[] pieces = GetPieces(filenames, _picelength.Value);
                info.Add("pieces", pieces);
            }
            else
            {
                FileStream fs = new FileStream(_name, FileMode.Open, FileAccess.Read);
                info.Add("name", Path.GetFileName(_name));
                info.Add("length", fs.Length);
                if (!_picelength.HasValue)
                {
                    _picelength = GetPieceLength(fs.Length);
                }
                info.Add("piece length", _picelength.Value);
                _total = fs.Length;
                byte[] pieces = GetPieces(fs, _picelength.Value);
                fs.Close();
                fs.Dispose();
                info.Add("pieces", pieces);
            }
            info.Add("publisher", "APTX分流组");
            info.Add("publisher-url", "http://bbs.aptx.cn");
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                string infohash = Aptxcode.eMule.Base32Encode.Encode(sha1.ComputeHash(BEncoder.BEncode(info)));
                FileInfo fi = new FileInfo(_name);
                magnet = string.Format("magnet:?xt=urn:btih:{0}&dn={1}{2}", infohash, System.Web.HttpUtility.UrlEncode(fi.Name), trs);
            }
            if (_isprivate)
                info.Add("private", 1);
            root.Add("info", info);
            normal = BEncoder.BEncode(root);
            //root.Remove("announce-list");
            //ArrayList listgl = new ArrayList(_greedland);
            //ArrayList trackerlistgl = new ArrayList();
            //trackerlistgl.Add(listgl);
            //root.Add("announce-list", trackerlistgl);
            //greedland = BEncoder.BEncode(root); 
        }

        private static int GetPieceLength(long size)
        {
            int ret = 128 * 1024;
            if (size >= 256 * 1024 * 1024)
                ret = 256 * 1024;
            else if (size >= 512 * 1024 * 1025)
                ret = 512 * 1024;
            else if (size >= 1024 * 1024 * 1024)
                ret = 1024 * 1024;
            ////else if (size >= 2048 * 1024 * 1024)
            ////    ret = 2048 * 1024;
            return ret;
        }

        private byte[] GetPieces(FileStream fs, int picelength)
        {
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (fs.Position < fs.Length)
                    {
                        byte[] buf = new byte[picelength];
                        int read = fs.Read(buf, 0, picelength);
                        byte[] sha1hash = sha1.ComputeHash(buf, 0, read);
                        ms.Write(sha1hash, 0, sha1hash.Length);
                        if (worker != null && worker.WorkerReportsProgress)
                            worker.ReportProgress((int)(fs.Position * 100 / fs.Length), this);
                    }
                    return ms.ToArray();
                }
            }
        }

        private byte[] GetPieces(List<string> files,int picelength)
        {
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int i = 0;
                    FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                    byte[] buf = new byte[picelength];
                    int read = fs.Read(buf, 0, picelength);
                    while (read != 0 || i < files.Count)
                    {
                        if (read < picelength && ++i < files.Count)
                        {
                            fs.Close();
                            fs = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                            read += fs.Read(buf, read, picelength - read);
                            continue;
                        }
                        byte[] tmp = sha1.ComputeHash(buf, 0, read);
                        ms.Write(tmp, 0, tmp.Length);
                        _current += read;
                        read = fs.Read(buf, 0, picelength);
                        if (worker != null && worker.WorkerReportsProgress)
                            worker.ReportProgress((int)(_current * 100 / _total), this);
                    }
                    fs.Close();
                    fs.Dispose();
                    return ms.ToArray();
                }
            }
        }

        private ArrayList GetFiles(DirectoryInfo dir,string name , ref long fileslength, ref List<string> filenames)
        {
            ArrayList files = new ArrayList();
            FileInfo[] fileinfos = dir.GetFiles();

            DirectoryInfo[] dirinfos = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirinfos)
            {
                files.AddRange(GetFiles(subdir, name, ref fileslength,ref filenames));
            }

            foreach (FileInfo fi in fileinfos)
            {
                SortedDictionary<string, object> file = new SortedDictionary<string, object>();
                file.Add("length", fi.Length);
                ArrayList path = new ArrayList();
                path.AddRange(fi.FullName.Replace(name + "\\", string.Empty).Split('\\'));
                file.Add("path", path);
                files.Add(file);
                fileslength += fi.Length;
                filenames.Add(fi.FullName);
            }
            return files;
        }
    }
}
