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
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("TenhouViewer [dev.]");

            ParseArgs(args);
            if (args.Length == 0) ShowHelp();
        }

        static void ShowHelp()
        {
            Console.WriteLine("Help:");
            Console.WriteLine("TenhouViewer -DHash - download game;");
            Console.WriteLine("TenhouViewer -dLog.txt - download all games from log Log.txt;");
            Console.WriteLine("TenhouViewer -PHash - parse game;");
            Console.WriteLine("TenhouViewer -pLog.txt - parse all games from log Log.txt;");
            Console.WriteLine("TenhouViewer -fLog.txt - find games from log Log.txt with query:");
            Console.WriteLine(" shanten=N - find all hands started with N shanten number (0-6);");
            Console.WriteLine(" shantenmin=N - find all hands started with shanten number greater than N (0-6);");
            Console.WriteLine(" shantenmax=N - find all hands started with shanten number less than N (0-6);");
            Console.WriteLine(" ratingmin=N - find all players, who has rating greater than N (1000-3000);");
            Console.WriteLine(" ratingmax=N - find all players, who has rating less than N (1000-3000);");
            Console.WriteLine(" paymentmin=N - find all players, who receive or pay greater than N pt (-1000000-1000000);");
            Console.WriteLine(" paymentmax=N - find all players, who receive or pay less than N pt (-1000000-1000000);");
            Console.WriteLine(" place=N - find all players, who took N place (1-4);");
            Console.WriteLine(" rank=N - find all players, who has rank N (0-20);");
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
                    case 'P':
                        // Parse game by hash
                        // -P2013070808gm-0089-0000-2f83b7da
                        ParseHash(Argument);
                        break;
                    case 'p':
                        // Parse games by log
                        // -pLog.txt
                        ParseLog(Argument);
                        break;
                    case 'f':
                        // Parse games by log
                        // -fLog.txt shanten=1
                        Find(Argument, args);
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

        static void ParseHash(string Hash)
        {
            Console.Write("Parsing game: " + Hash);
            string ReplayFileName = Hash + ".xml";

            Console.Write(Hash);

            if (!File.Exists(ReplayFileName))
            {
                Console.WriteLine(" - file not found!");
                return;
            }

            Tenhou.ReplayDecoder R = new Tenhou.ReplayDecoder();
            R.OpenPlainText(ReplayFileName, Hash);

            // replay (calc shanten, waitings and other) and save result
            Mahjong.Replay Replay = R.R;
            Replay.ReplayGame();
            Replay.Save();

            Console.WriteLine(" - ok!");

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

        static void Find(string FileName, string[] args)
        {
            Console.WriteLine("Finding games from log: " + FileName);

            if (!File.Exists(FileName))
            {
                Console.WriteLine("Error: Log file " + FileName + " not found!");
                return;
            }

            Tenhou.LogParser Log = new Tenhou.LogParser(FileName);

            Search.GameFinder Finder = new Search.GameFinder(Log.HashList);

            for (int i = 1; i < args.Length; i++)
            {
                string a = args[i];
                int Delimiter = a.IndexOf('=');
                if(Delimiter < 0) continue;

                string Param = a.Substring(0, Delimiter);
                string Value = a.Substring(Delimiter + 1);

                int TempValue;

                switch (Param)
                {
                    case "shanten":
                        TempValue = ParseIntArg(Value, 0, 6, "shanten");
                        if (TempValue != -1)
                        {
                            Finder.ShantenMax = TempValue;
                            Finder.ShantenMin = TempValue;
                        }
                        break;
                    case "shantenmin":
                        TempValue = ParseIntArg(Value, 0, 6, "shantenmin");
                        if (TempValue != -1) Finder.ShantenMin = TempValue;
                        break;
                    case "shantenmax":
                        TempValue = ParseIntArg(Value, 0, 6, "shantenmax");
                        if (TempValue != -1) Finder.ShantenMax = TempValue;
                        break;
                    case "ratingmin":
                        TempValue = ParseIntArg(Value, 1000, 3000, "ratingmin");
                        if (TempValue != -1) Finder.RatingMin = TempValue;
                        break;
                    case "ratingmax":
                        TempValue = ParseIntArg(Value, 1000, 3000, "ratingmax");
                        if (TempValue != -1) Finder.RatingMax = TempValue;
                        break;
                    case "paymentmin":
                        TempValue = ParseIntArg(Value, -1000000, 1000000, "paymentmin");
                        if (TempValue != -1) Finder.PaymentMin = TempValue;
                        break;
                    case "paymentmax":
                        TempValue = ParseIntArg(Value, -1000000, 1000000, "paymentmax");
                        if (TempValue != -1) Finder.PaymentMax = TempValue;
                        break;
                    case "place":
                        TempValue = ParseIntArg(Value, 1, 4, "place");
                        if (TempValue != -1) Finder.Place = TempValue;
                        break;
                    case "rank":
                        TempValue = ParseIntArg(Value, 0, 20, "rank");
                        if (TempValue != -1) Finder.Rank = TempValue;
                        break;
                }
            }

            List<Search.Result> ResultList = Finder.Find();

            Console.WriteLine(String.Format("Found: {0:d}", ResultList.Count));
            PrintList(ResultList);
        }

        static private string YakuList(List<Mahjong.Yaku> Yaku)
        {
            string Text = "";

            for (int i = 0; i < Yaku.Count; i++)
            {
                Text += Mahjong.YakuName.GetYakuName(Yaku[i].Index) + " ";
            }

            return Text;
        }

        static private void PrintList(List<Search.Result> ResultList)
        {
            for (int i = 0; i < ResultList.Count; i++)
            {
                Search.Result R = ResultList[i];

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    for (int k = 0; k < 4; k++)
                    {
                        if (R.HandMark[r][k])
                        {
                            string Format = "http://tenhou.net/0/?log={0:s}&ts={1:d}&tw={2:d}\t{3:d}\t{4:d}\t{5:s}\t{6:s}";
                            Console.WriteLine(String.Format(Format,
                                              R.Replay.Hash, r, k, R.Replay.Rounds[r].Pay[k],
                                              Rnd.StepCount[k],
                                              R.Replay.Players[k].NickName, YakuList(Rnd.Yaku[k])));                        }
                    }
                }
            }
        }

        private static int ParseIntArg(string Value, int Min, int Max, string ArgName)
        {
            int Temp = -1;

            try
            {
                Temp = Convert.ToInt32(Value);
            }
            catch (Exception)
            {
                Console.Write(String.Format("Error: incorrect argument for '{s:0}' query (must be number, {d:1}-{d:2}): {s:3}",
                    ArgName, Min, Max, Value));
                return -1;
            }
            if ((Temp < Min) || (Temp > Max))
            {
                Console.Write(String.Format("Error: incorrect argument for '{s:0}' query (must be number, {d:1}-{d:2}): {s:3}",
                    ArgName, Min, Max, Value));
                return -1;
            }

            return Temp;
        }
    }
}
