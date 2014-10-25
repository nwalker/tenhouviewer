using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            for (int i = 0; i < lines.Length; i++) 
            {
                string line = lines[i];
                const string url = "http://tenhou.net/0/?log=";
                int Pos = line.IndexOf(url);
                int PosEndTW = line.IndexOf("&tw=");
                int PosEndTS = line.IndexOf("&ts=");
                int PosEnd = PosEndTW;

                Console.Title = String.Format("Parsing log {0:d}/{1:d}", i + 1, lines.Length);

                if ((PosEndTW != -1) && (PosEndTS != -1)) PosEnd = (PosEndTW > PosEndTS) ? PosEndTS : PosEndTW;

                if (PosEnd == -1) PosEnd = line.IndexOf("\">");

                if (Pos >= 0)
                {
                    Pos += url.Length;

                    string Hash = line.Substring(Pos, PosEnd - Pos);
                    TenhouHash H = new TenhouHash(Hash);

                    if(!HashList.Hashes.Contains(H.DecodedHash))
                        HashList.Hashes.Add(H.DecodedHash);
                }
            }
        }

        public static string GetFileName(string Hash, string Dir)
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

                Console.Title = String.Format("Downloading {0:d}/{1:d}: {2:s}", i, HashList.Hashes.Count, Hash);
                if (Download(Hash, Dir))
                    Console.WriteLine(Hash + ": ok");
                else
                    Console.WriteLine(Hash + ": fail");
            }
        }

        private string GetMjlogFilename(string Hash, string[] Files)
        {
            // find in directory
            foreach (string F in Files)
            {
                if (F.Contains(Hash))
                    return F;
            }

            return null;
        }

        public void DownloadAllFromMjlog(string Dir, string[] Files)
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            for (int i = 0; i < HashList.Hashes.Count; i++)
            {
                string Hash = HashList.Hashes[i];
                Console.Title = String.Format("Loading {0:d}/{1:d}: {2:s}", i, HashList.Hashes.Count, Hash);

                string FileName = GetFileName(Hash, Dir);

                if (File.Exists(FileName))
                    Console.WriteLine(Hash + ": exists");
                else
                {
                    string F = GetMjlogFilename(Hash, Files);
                    if(F != null)
                    {
                        try
                        {
                            FileStream GzFile = new FileStream(F, FileMode.Open, FileAccess.Read);
                            GZipStream Stream = new GZipStream(GzFile, CompressionMode.Decompress);

                            using (var fileStream = File.Create(FileName))
                            {
                                Stream.CopyTo(fileStream);
                            }
                            Console.WriteLine(Hash + ": ok");
                        }
                        catch(Exception E)
                        {
                            Console.WriteLine(Hash + ": fail [" + E.Message + "]");
                        }
                    }
                    else
                       Console.WriteLine(Hash + ": file not found");
                }
            }
        }
    }
}
