using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Aptxcode.eMule
{
    class ed2k
    {
        const int EMPARTSIZE = 9728000;
        BackgroundWorker worker;

        public BackgroundWorker Worker 
        { 
            set { worker = value; }
        }


        public string GetLink(string filename)
        {

            bool isdir = Directory.Exists(filename);

            if (isdir)
            {
                return GenLink(new DirectoryInfo(filename));
            }
            else
            {
                return GenLink(filename);
            }
            
        }

        string GenLink(DirectoryInfo dir)
        {
            string links = string.Empty;
            foreach (FileInfo fi in dir.GetFiles())
            {
                links += "\r\n" + GetLink(fi.FullName);
            }
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                links += "\r\n" + GenLink(subdir);
            }
            return links;
        }

        string GenLink(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                AICHHash aich = new AICHHash(fi.Length);
                List<byte[]> hashset = new List<byte[]>();
                MD4 md4 = MD4.Create();
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] buffer = new byte[EMPARTSIZE];
                    int readCount;
                    do
                    {
                        readCount = fs.Read(buffer, 0, buffer.Length);
                        if (readCount > 0)
                        {
                            hashset.Add(md4.ComputeHash(buffer, 0, readCount));
                            aich.CalcAICH(buffer, 0, readCount);
                            if (worker != null && worker.WorkerReportsProgress)
                            {
                                worker.ReportProgress((int)(fs.Position * 100 / fs.Length), this);
                            }
                        }
                        else if (fs.Length % EMPARTSIZE == 0)
                        {
                            hashset.Add(md4.ComputeHash(new byte[] { }));
                        }
                    }
                    while (readCount != 0);
                }
                byte[] filehash = new byte[16];
                if (hashset.Count == 1)
                    filehash = hashset[0];
                else
                    filehash = md4.ComputeHash(hashset.SelectMany(bytes => bytes).ToArray());
                return string.Format("ed2k://|file|{0}|{1}|{2}|h={3}|/", System.Web.HttpUtility.UrlEncode(fi.Name), fi.Length, String.Concat(filehash.Select(b => b.ToString("X2")).ToArray()), aich.RootHash);
            }
            else
                throw new FileNotFoundException(); 
        }
    }
}
