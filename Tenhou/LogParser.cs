using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TenhouViewer.Tenhou
{
    class LogParser
    {
        private string FileName;
        public TenhouHashList HashList = new TenhouHashList();

        public LogParser(string FileName)
        {
            this.FileName = FileName;

            if (!File.Exists(FileName)) return;

            string[] lines = System.IO.File.ReadAllLines(FileName);

            foreach (string line in lines)
            {
                const string url = "http://tenhou.net/0/?log=";
                int Pos = line.IndexOf(url);
                int PosEnd = line.IndexOf("&tw=");

                if (Pos >= 0)
                {
                    Pos += url.Length;

                    string Hash = line.Substring(Pos, PosEnd - Pos);
                    TenhouHash H = new TenhouHash(Hash);

                    HashList.Hashes.Add(H.DecodedHash);
                }
            }
        }

        public string GetFileName(string Hash, string Dir)
        {
            return Dir + "/" + Hash + ".xml";
        }

        public bool Download(string Hash, string Dir)
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            Downloader D = new Downloader(Hash);
            string FileName = GetFileName(Hash, Dir);

            if (File.Exists(FileName))
            {
                return true;
            }
            else
            {
                return D.Download(FileName);
            }
        }

        public void DownloadAll(string Dir)
        {
            for (int i = 0; i < HashList.Hashes.Count; i++)
            {
                string Hash = HashList.Hashes[i];

                if (Download(Hash, Dir))
                {
                    Console.WriteLine(Hash + ": ok");
                }
                else
                {
                    Console.WriteLine(Hash + ": fail");
                }
            }
        }
    }
}
