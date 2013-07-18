using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TenhouViewer
{
    class Program
    {
        static string LogDir = "logs";

        static void Main(string[] args)
        {
            Console.WriteLine("TenhouViewer [dev.]");

            ParseArgs(args);
            if (args.Length == 0) ShowHelp();
        }

        static void ShowHelp()
        {
            Console.WriteLine("Help:");
            Console.WriteLine("TenhouViewer -DHash - download game");
            Console.WriteLine("TenhouViewer -dLog.txt - download all games from log Log.txt");
            Console.WriteLine("TenhouViewer -pLog.txt - parse all games from log Log.txt");
        }

        static void ParseArgs(string[] args)
        {
            foreach (string a in args)
            {
                if (!((a[0] == '-') || (a[0] == '\\')))
                {
                    Console.WriteLine("Unknown argument: " + a);
                    continue;
                }

                string Argument = a.Substring(2);
                switch (a[1])
                {
                    case 'D':
                        // Download game by hash
                        // -D2013070808gm-0089-0000-2f83b7da
                        DownloadHash(Argument);
                        break;
                    case 'd':
                        // Download games by log
                        // -dLog.txt
                        DownloadLog(Argument);
                        break;
                    case 'p':
                        // Parse games by log
                        // -pLog.txt
                        ParseLog(Argument);
                        break;
                }
            }
        }

        static void DownloadHash(string Hash)
        {
            string FileName = Hash + ".xml";

            Console.Write("Downloading game: " + Hash);

            Tenhou.Downloader D = new Tenhou.Downloader(Hash);
            if (!D.Download(FileName))
            {
                Console.WriteLine(" - error!");
                return;
            }

            Console.WriteLine(" - ok!");
        }

        static void DownloadLog(string FileName)
        {
            Console.Write("Downloading games from log: " + FileName);

            if (!File.Exists(FileName))
            {
                Console.Write("Error: Log file " + FileName + " not found!");
                return;
            }

            Tenhou.LogParser Log = new Tenhou.LogParser(FileName);
            Log.DownloadAll(LogDir);
        }

        static void ParseLog(string FileName)
        {
            Console.Write("Parsing games from log: " + FileName);

            if (!File.Exists(FileName))
            {
                Console.Write("Error: Log file " + FileName + " not found!");
                return;
            }

            Tenhou.LogParser Log = new Tenhou.LogParser(FileName);
            List<string> Hashes = Log.HashList.Hashes;

            foreach (string Hash in Hashes)
            {
                string ReplayFileName = Log.GetFileName(Hash, LogDir);

                Console.Write(Hash);

                if (!File.Exists(ReplayFileName))
                {
                    Console.WriteLine(" - file not found!");
                    continue;
                }

                Tenhou.ReplayDecoder R = new Tenhou.ReplayDecoder();
                R.OpenPlainText(ReplayFileName, Hash);

                // replay (calc shanten, waitings and other) and save result
                Mahjong.Replay Replay = R.R;
                Replay.ReplayGame();
                Replay.Save();

                Console.WriteLine(" - ok!");
            }
        }
    }
}
