using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TenhouViewer.Tenhou
{
    class Downloader
    {
        private string Hash = "";

        public Downloader(string Hash)
        {
            this.Hash = Hash;
        }

        private bool TryFFServer(string FileName)
        {
            string Url = "http://ff.mjv.jp/0/log/?" + Hash;

            return DownloadFile(Url, FileName);
        }

        private bool TryEEServer(string FileName)
        {
            string Url = "http://ee.mjv.jp/0/log_/?" + Hash;

            return DownloadFile(Url, FileName);
        }

        private bool TryEServer(string FileName)
        {
            string Url = "http://e.mjv.jp/0/log/?" + Hash;

            return DownloadFile(Url, FileName);
        }

        private bool DownloadFile(string Url, string FileName)
        {
            if (!File.Exists(FileName))
            {
                WebClient webClient = new WebClient();

                try
                {
                    webClient.DownloadFile(Url, FileName);
                }
                catch (Exception)
                {
                    return false;
                }

                long length = (new FileInfo(FileName)).Length;

                if (length == 0)
                {
                    File.Delete(FileName);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        public bool Download(string FileName)
        {
            // Try servers with log
            if (!TryEServer(FileName))
            {
                if (!TryEEServer(FileName))
                {
                    return TryFFServer(FileName);
                }
            }

            return true;
        }
    }
}
