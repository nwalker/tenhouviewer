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
            Console.WriteLine(" lobby=N - find all games from specified lobby (0-6);");
            Console.WriteLine(" shanten=N - find all hands started with N shanten number (0-6);");
            Console.WriteLine(" shantenmin=N - find all hands started with shanten number greater (or equal) than N (0-6);");
            Console.WriteLine(" shantenmax=N - find all hands started with shanten number less (or equal) than N (0-6);");
            Console.WriteLine(" ratingmin=N - find all players, who has rating greater (or equal) than N (1000-3000);");
            Console.WriteLine(" ratingmax=N - find all players, who has rating less (or equal) than N (1000-3000);");
            Console.WriteLine(" paymentmin=N - find all players, who receive or pay greater (or equal) than N pt (-1000000-1000000);");
            Console.WriteLine(" paymentmax=N - find all players, who receive or pay less (or equal) than N pt (-1000000-1000000);");
            Console.WriteLine(" waitmin=N - find all hands which has N or greater sides of winning waiting (1-13);");
            Console.WriteLine(" waitmax=N - find all hands which has N or less sides of winning waiting (1-13);");
            Console.WriteLine(" hanmin=N - find all hands which has han count greater (or equal) than N (1-13);");
            Console.WriteLine(" hanmax=N - find all hands which has han count less (or equal) than N (1-13);");
            Console.WriteLine(" fumin=N - find all hands which has fu count greater (or equal) than N (1-120);");
            Console.WriteLine(" fumax=N - find all hands which has fu count less (or equal) than N (1-120);");
            Console.WriteLine(" place=N - find all players, who took N place (1-4);");
            Console.WriteLine(" rank=N - find all players, who has rank N (0-20);");
            Console.WriteLine(" nickname=N - find player, who has nickname N (string);");
            Console.WriteLine(" steps=N - find all hands, who exist less (or equal) than N steps (0-60);");
            Console.WriteLine(" yaku=N,M,X - find all hands, which has N,M,X,... yaku (0-54);");
            Console.WriteLine(" wait=N,M,X - find all hands, which has at least one tile from list in waiting: N,M,X,... (0-36);");
            Console.WriteLine(" dealer - find all dealer's hands;");
            Console.WriteLine(" winner - find all completed hands;");
            Console.WriteLine(" loser - find all players (games), who dealt into ron;");
            Console.WriteLine(" players - count of players in game (3-4);");
            Console.WriteLine("TenhouViewer -g<nickname> <fields> - graph rounds (which found by -f flag) with fields:");
            Console.WriteLine(" index - round index in list;");
            Console.WriteLine(" initshanten - shanten in start hand in round;");
            Console.WriteLine(" pay - payment in round;");
            Console.WriteLine(" tempai - is hand was tempai (1 or 0);");
            Console.WriteLine(" dealer - is hand was dealer (1 or 0);");
            Console.WriteLine(" loser - is player dealt in other hand (1 or 0);");
            Console.WriteLine(" winner - is hand completed (1 or 0);");
            Console.WriteLine(" riichi - is riichi declared (1 or 0);");
            Console.WriteLine(" concealed - is hand was concealed (1 or 0);");
            Console.WriteLine(" openedsets - amount of opened sets;");
            Console.WriteLine(" cost - cost of hand;");
            Console.WriteLine(" fu - count of minipoints in hand;");
            Console.WriteLine(" han - count of game points in hand;");
            Console.WriteLine(" step - count of steps to end in round;");
            Console.WriteLine(" balance - balance in hand (pts);");
            Console.WriteLine(" waiting - amount of tile types in waiting;");
            Console.WriteLine(" round - index of round(0=1e,1=2e,2=3e...);");
            Console.WriteLine(" players - count of players in round;");
            Console.WriteLine(" draw - round ended in draw;");
            Console.WriteLine(" draw=N - round ended in draw with reason (yao9,reach4,ron3,kan4,kaze4,nm);");
            Console.WriteLine("TenhouViewer -G<nickname> <fields> - graph games (which found by -f flag) with fields:");
            Console.WriteLine(" index - game index in list;");
            Console.WriteLine(" rating - player rating before this game;");
            Console.WriteLine(" rank - player rank before this game (1=1ku, 10=1dan,...);");
            Console.WriteLine(" place - place in game;");
            Console.WriteLine(" result - game result with uma;");
            Console.WriteLine(" balance - balance in the end of game;");
            Console.WriteLine(" players - count of players in game;");
            Console.WriteLine(" datetime - date of game;");
            Console.WriteLine("TenhouViewer -o<nickname> <fields> - format output results:");
            Console.WriteLine(" link - link to the round;");
            Console.WriteLine(" lobby - lobby;");
            Console.WriteLine(" lobbytype - lobby 0000 type: 9 - common, 1 - dan, 2 - higher dan, 3 - phoenix;");
            Console.WriteLine(" nickname - nickname of the player;");
            Console.WriteLine(" rating - rating of the player;");
            Console.WriteLine(" rank - rank of the player;");
            Console.WriteLine(" place - place (result) in game;");
            Console.WriteLine(" pay - player payment in round;");
            Console.WriteLine(" dealer - is player dealer;");
            Console.WriteLine(" winner - is player complete hand;");
            Console.WriteLine(" loser - is player dealt in other player's hand;");
            Console.WriteLine(" concealed - is hand concealed;");
            Console.WriteLine(" cost - cost of hand;");
            Console.WriteLine(" han - amount of game points in hand;");
            Console.WriteLine(" waiting - amount of tile types in waiting;");
            Console.WriteLine(" step - amount of player steps in round;");
            Console.WriteLine(" yaku - list of yaku;");
            Console.WriteLine(" round - current round (0-1e, 1-2e, ...);");
            Console.WriteLine(" roundindex - index of round in game;");
            Console.WriteLine(" place - player's place in game;");
            Console.WriteLine("TenhouViewer -s<filename> - save find or graph result to specified file;");
            Console.WriteLine("TenhouViewer -U<hash> <params> - get paifu:");
            Console.WriteLine(" dir - directory to save result (for all rounds);");
            Console.WriteLine(" filename - filename to save result (for specified round, without extension);");
            Console.WriteLine(" round - round index (from 0);");
            Console.WriteLine("TenhouViewer -u <params> - get paifu for all rounds, which was found before:");
            Console.WriteLine(" dir - directory to save result (for all rounds);");
        }

        static void ParseArgs(string[] args)
        {
            ArgumentParser Parser = new ArgumentParser(args);
            List<Argument> ArgList = Parser.Arguments;
            List<Search.Result> ResultList = null;

            List<string> GraphResult = null;
            List<string> FindResult = null;

            for (int i = 0; i < ArgList.Count; i++)
            {
                switch(ArgList[i].Name)
                {
                    case "D":
                        // Download game by hash
                        // -D2013070808gm-0089-0000-2f83b7da
                        DownloadHash(ArgList[i].Value);
                        break;
                    case "d":
                        // Download games by log
                        // -dLog.txt
                        DownloadLog(ArgList[i].Value);
                        break;
                    case "P":
                        // Parse game by hash
                        // -P2013070808gm-0089-0000-2f83b7da
                        ParseHash(ArgList[i].Value);
                        break;
                    case "p":
                        // Parse games by log
                        // -pLog.txt
                        ParseLog(ArgList[i].Value);
                        break;
                    case "f":
                        // Parse games by log (can be iterative)
                        // -fLog.txt shanten=1
                        ResultList = Find(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        FindResult = ConvertResultsToString(ResultList);
                        GraphResult = null;
                        break;
                    case "g":
                        // Graph rounds (which found by -f flag)
                        // -gtfizik index winner loser
                        FindResult = null;
                        GraphResult = GraphRounds(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                    case "G":
                        // Graph games (which found by -f flag)
                        // -Gtfizik index rating rank balance
                        FindResult = null;
                        GraphResult = GraphGames(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                    case "s":
                        // Save graph result
                        // -sriichi.txt
                        {
                            if(GraphResult != null)
                            {
                                Statistic.Saver Saver = new Statistic.Saver(ArgList[i].Value, GraphResult);
                                GraphResult = null;
                            }
                            else if(FindResult != null)
                            {
                                Statistic.Saver Saver = new Statistic.Saver(ArgList[i].Value, FindResult);
                                FindResult = null;
                            }
                        }
                        break;
                    case "o":
                        // Format find result
                        {
                            GraphResult = null;
                            FindResult = ConvertResultsToString(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        }
                        break;
                    case "u":
                        // Paifu by find results
                        GraphResult = null;
                        FindResult = null;
                        CreatePaifuList(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                    case "U":
                        // Paifu by hash
                        CreatePaifu(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                }
            }

            if (FindResult != null)
            {
                // Search query result
                Console.WriteLine(String.Format("Found: {0:d}", ResultList.Count));
                foreach(string Line in FindResult) Console.WriteLine(Line);
            }
        }

        static void CreatePaifu(string Hash, List<Argument> ArgList, List<Search.Result> Results)
        {
            string Dir = "paifu";
            string FN = "";
            int Round = -1;

            Hash = new Tenhou.TenhouHash(Hash).DecodedHash;

            foreach(Argument A in ArgList)
            {
                if (A.Name.CompareTo("dir") == 0) Dir = A.Value;
                if (A.Name.CompareTo("round") == 0) Round = Convert.ToInt32(A.Value);

                if (A.Name.CompareTo("filename") == 0) FN = A.Value;
            }

            Mahjong.Replay R = new Mahjong.Replay();
            R.LoadXml(Hash);

            for (int i = 0; i < R.Rounds.Count; i++)
            {
                if ((Round != -1) && (Round != i)) continue;

                Paifu.PaifuGenerator P = new Paifu.PaifuGenerator(R, i);

                string FileName;

                if (FN.CompareTo("") == 0)
                {
                    if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
                    FileName = String.Format("./{0:s}/{1:s}_{2:d}.png", Dir, Hash, Round);
                }
                else
                {
                    FileName = (Round == -1) ? String.Format("{0:s}_{1:d}.png", FN, i) : String.Format("{0:s}.png", FN);
                }
                P.Save(FileName);
            }
        }

        static void CreatePaifuList(string Argument, List<Argument> ArgList, List<Search.Result> Results)
        {
            string Dir = "paifu";

            foreach (Argument A in ArgList)
            {
                if (A.Name.CompareTo("dir") == 0) Dir = A.Value;
            }

            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            for (int i = 0; i < Results.Count; i++)
            {
                Search.Result R = Results[i];

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    if (!R.RoundMark[r]) continue;

                    Paifu.PaifuGenerator P = new Paifu.PaifuGenerator(R.Replay, r);


                    string FileName = String.Format("./{0:s}/{1:s}_{2:d}.png", Dir, R.Replay.Hash, r);
                    P.Save(FileName);
                }
            }
        }

        static List<string> ConvertResultsToString(string Argument, List<Argument> ArgList, List<Search.Result> Results)
        {
            List<string> Output = new List<string>();

            for (int i = 0; i < Results.Count; i++)
            {
                Search.Result R = Results[i];

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    for (int k = 0; k < 4; k++)
                    {
                        if (R.HandMark[r][k])
                        {
                            // Format this result:
                            string Temp = "";

                            for(int o = 0; o < ArgList.Count; o++)
                            {
                                switch(ArgList[o].Name)
                                {
                                    case "link":
                                        Temp += String.Format("http://tenhou.net/0/?log={0:s}&ts={1:d}&tw={2:d}\t",
                                                              R.Replay.Hash, r, k);
                                        break;
                                    case "nickname":
                                        Temp += String.Format("{0:s}\t", R.Replay.Players[k].NickName);
                                        break;
                                    case "rating":
                                        Temp += String.Format("{0:d}\t", R.Replay.Players[k].Rating);
                                        break;
                                    case "place":
                                        Temp += String.Format("{0:d}\t", R.Replay.Place[k]);
                                        break;
                                    case "rank":
                                        Temp += String.Format("{0:d}\t", R.Replay.Players[k].Rank);
                                        break;
                                    case "pay":
                                        Temp += String.Format("{0:d}\t", Rnd.Pay[k]);
                                        break;
                                    case "dealer":
                                        Temp += String.Format("{0:d}\t", Rnd.Dealer[k] ? 1 : 0);
                                        break;
                                    case "winner":
                                        Temp += String.Format("{0:d}\t", Rnd.Winner[k] ? 1 : 0);
                                        break;
                                    case "loser":
                                        Temp += String.Format("{0:d}\t", Rnd.Loser[k] ? 1 : 0);
                                        break;
                                    case "concealed":
                                        Temp += String.Format("{0:d}\t", (Rnd.OpenedSets[k] == 0) ? 1 : 0);
                                        break;
                                    case "cost":
                                        Temp += String.Format("{0:d}\t", Rnd.Cost[k]);
                                        break;
                                    case "han":
                                        Temp += String.Format("{0:d}\t", Rnd.HanCount[k]);
                                        break;
                                    case "waiting":
                                        Temp += String.Format("{0:d}\t", Rnd.WinWaiting[k].Count);
                                        break;
                                    case "step":
                                        Temp += String.Format("{0:d}\t", Rnd.StepCount[k]);
                                        break;
                                    case "yaku":
                                        Temp += String.Format("{0:s}\t", YakuList(Rnd.Yaku[k]));
                                        break;
                                    case "round":
                                        Temp += String.Format("{0:d}\t", Rnd.CurrentRound);
                                        break;
                                    case "lobby":
                                        Temp += String.Format("{0:d}\t", Rnd.Lobby);
                                        break;
                                    case "roundindex":
                                        Temp += String.Format("{0:d}\t", r);
                                        break;
                                }
                            }

                            Output.Add(Temp);
                        }
                    }
                }
            }

            return Output;
        }

        static List<string> ConvertResultsToString(List<Search.Result> Results)
        {
            List<string> Output = new List<string>();

            for (int i = 0; i < Results.Count; i++)
            {
                Search.Result R = Results[i];

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    for (int k = 0; k < 4; k++)
                    {
                        if (R.HandMark[r][k])
                        {
                            string Format = "http://tenhou.net/0/?log={0:s}&ts={1:d}&tw={2:d}\t{3:d}\t{4:d}\t{5:s}\t{6:s}";
                            Output.Add(String.Format(Format,
                                              R.Replay.Hash, r, k, R.Replay.Rounds[r].Pay[k],
                                              Rnd.StepCount[k],
                                              R.Replay.Players[k].NickName, YakuList(Rnd.Yaku[k])));
                        }
                    }
                }
            }

            return Output;
        }

        static List<string> GraphRounds(string NickName, List<Argument> ArgList, List<Search.Result> Results)
        {
            if (Results == null)
            {
                Console.Write("Error: no results to graph.");
                return null;
            }

            Statistic.Graph G = new Statistic.Graph(NickName, Results);

            // Fill fields info
            for (int i = 0; i < ArgList.Count; i++) G.Fields.Add(ArgList[i].Name);

            return G.RoundGraph();
        }

        static List<string> GraphGames(string NickName, List<Argument> ArgList, List<Search.Result> Results)
        {
            if (Results == null)
            {
                Console.Write("Error: no results to graph.");
                return null;
            }

            Statistic.Graph G = new Statistic.Graph(NickName, Results);

            // Fill fields info
            for (int i = 0; i < ArgList.Count; i++) G.Fields.Add(ArgList[i].Name);

            return G.GamesGraph();
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
            Console.WriteLine("Downloading games from log: " + FileName);

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
            Console.WriteLine("Parsing games from log: " + FileName);

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

                if (Mahjong.Replay.IsReplayExist(Hash))
                {
                    Console.WriteLine(" - exists, skip!");
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

        static List<Search.Result> Find(string FileName, List<Argument> ArgList, List<Search.Result> Results)
        {
            Search.GameFinder Finder;

            if (Results != null)
            {
                Console.WriteLine("Finding games from previous search result.");

                Finder = new Search.GameFinder(Results);
            }
            else
            {
                if (!File.Exists(FileName))
                {
                    Console.WriteLine("Error: Log file " + FileName + " not found!");
                    return null;
                }

                Console.WriteLine("Finding games from log: " + FileName + ".");

                Tenhou.LogParser Log = new Tenhou.LogParser(FileName);
                Finder = new Search.GameFinder(Log.HashList);
            }

            for (int i = 0; i < ArgList.Count; i++)
            {
                string Param = ArgList[i].Name;
                string Value = ArgList[i].Value;

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

                        Console.WriteLine(String.Format("Filter: only hands, which started with shanten {0:d};", TempValue));
                        break;
                    case "shantenmin":
                        TempValue = ParseIntArg(Value, 0, 6, "shantenmin");
                        if (TempValue != -1) Finder.ShantenMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which started with shanten greater (or equal) than {0:d};", TempValue));
                        break;
                    case "shantenmax":
                        TempValue = ParseIntArg(Value, 0, 6, "shantenmax");
                        if (TempValue != -1) Finder.ShantenMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which started with shanten less (or equal) than {0:d};", TempValue));
                        break;
                    case "ratingmin":
                        TempValue = ParseIntArg(Value, 1000, 3000, "ratingmin");
                        if (TempValue != -1) Finder.RatingMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only players, who has rating greater (or equal) than {0:d};", TempValue));
                        break;
                    case "ratingmax":
                        TempValue = ParseIntArg(Value, 1000, 3000, "ratingmax");
                        if (TempValue != -1) Finder.RatingMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only players, who has rating less (or equal) than {0:d};", TempValue));
                        break;
                    case "paymentmin":
                        TempValue = ParseIntArg(Value, -1000000, 1000000, "paymentmin");
                        if (TempValue != -1) Finder.PaymentMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only players, who pay(-) or receive(+) greater (or equal) than {0:d} points;", TempValue));
                        break;
                    case "paymentmax":
                        TempValue = ParseIntArg(Value, -1000000, 1000000, "paymentmax");
                        if (TempValue != -1) Finder.PaymentMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only players, who pay(-) or receive(+) less (or equal) than {0:d} points;", TempValue));
                        break;
                    case "waitmin":
                        TempValue = ParseIntArg(Value, 1, 13, "waitmin");
                        if (TempValue != -1) Finder.WaitingCountMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has {0:d}-sided wait and greater;", TempValue));
                        break;
                    case "waitmax":
                        TempValue = ParseIntArg(Value, 1, 13, "waitmax");
                        if (TempValue != -1) Finder.WaitingCountMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has {0:d}-sided wait and less;", TempValue));
                        break;
                    case "hanmin":
                        TempValue = ParseIntArg(Value, 0, 13, "hanmin");
                        if (TempValue != -1) Finder.HanMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has han count greater (or equal) than {0:d};", TempValue));
                        break;
                    case "hanmax":
                        TempValue = ParseIntArg(Value, 0, 13, "hanmax");
                        if (TempValue != -1) Finder.HanMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has han count less (or equal) than {0:d};", TempValue));
                        break;
                    case "fumin":
                        TempValue = ParseIntArg(Value, 0, 120, "fumin");
                        if (TempValue != -1) Finder.FuMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has fu count greater (or equal) than {0:d};", TempValue));
                        break;
                    case "fumax":
                        TempValue = ParseIntArg(Value, 0, 120, "fumax");
                        if (TempValue != -1) Finder.FuMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has fu count less (or equal) than {0:d};", TempValue));
                        break;
                    case "lobby":
                        TempValue = ParseIntArg(Value, 0, 9999, "lobby");
                        if (TempValue != -1) Finder.Lobby = TempValue;

                        Console.WriteLine(String.Format("Filter: only games from {0:d} lobby;", TempValue));
                        break;
                    case "lobbytype":
                        TempValue = ParseIntArg(Value, 0, 10, "lobbytype");
                        if (TempValue != -1) Finder.LobbyType = TempValue;

                        string LobbyTypeName = "";
                        switch (TempValue)
                        {
                            case 0: LobbyTypeName = "common"; break;
                            case 1: LobbyTypeName = "dan"; break;
                            case 2: LobbyTypeName = "higher dan"; break;
                            case 3: LobbyTypeName = "phoenix"; break;
                        }
                        Console.WriteLine(String.Format("Filter: only games from 0000 {0:s} lobby;", LobbyTypeName));
                        break;
                    case "place":
                        TempValue = ParseIntArg(Value, 1, 4, "place");
                        if (TempValue != -1) Finder.Place = TempValue;

                        Console.WriteLine(String.Format("Filter: only players, who took {0:d} place;", TempValue));
                        break;
                    case "rank":
                        TempValue = ParseIntArg(Value, 0, 20, "rank");
                        if (TempValue != -1) Finder.Rank = TempValue;

                        Console.WriteLine(String.Format("Filter: only players, who has rank '{0:d}';", TempValue));
                        break;
                    case "steps":
                        TempValue = ParseIntArg(Value, 0, 60, "steps");
                        if (TempValue != -1) Finder.StepsMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which exists less (or equal) than '{0:d}' steps;", TempValue));
                        break;
                    case "yaku":
                        Finder.YakuList = DecompositeIntList(Value);

                        if(Finder.YakuList != null)
                        {
                            for (int j = 0; j < Finder.YakuList.Length; j++)
                            {
                                Console.WriteLine(String.Format("Filter: only hands with yaku '{0:s}';", Mahjong.YakuName.GetYakuName(Finder.YakuList[j])));
                            }
                        }
                        break;
                    case "wait":
                        Finder.Waitings = DecompositeIntList(Value);

                        if (Finder.Waitings != null)
                        {
                            Console.WriteLine(String.Format("Filter: only hands, which has at least one waiting from list: '{0:s}';", Value));
                        }
                        break;
                    case "nickname":
                        Finder.NickName = Value;

                        Console.WriteLine(String.Format("Filter: only player with nickname '{0:s}';", Value));
                        break;
                    case "dealer":
                        Finder.Dealer = true;

                        Console.WriteLine("Filter: only dealer hands;");
                        break;
                    case "loser":
                        Finder.Loser = true;

                        Console.WriteLine("Filter: only players, who dealt into ron;");
                        break;
                    case "winner":
                        Finder.Winner = true;

                        Console.WriteLine("Filter: only players, who completed hand;");
                        break;
                    case "players":
                        TempValue = ParseIntArg(Value, 3, 4, "players");
                        if (TempValue != -1) Finder.PlayerCount = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with {0:d} players;", TempValue));
                        break;
                    case "draw":
                        string Comment;
                        Finder.Draw = true;
                        switch (Value)
                        {
                            case "yao9": Finder.DrawReason = 0; Comment = "only games which ended in a draw because of kyushu kyuhai"; break;
                            case "reach4": Finder.DrawReason = 1; Comment = "only games which ended in a draw because of 4 reach"; break;
                            case "ron3": Finder.DrawReason = 2; Comment = "only games which ended in a draw because of triple ron"; break; 
                            case "kan4": Finder.DrawReason = 3; Comment = "only games which ended in a draw because of 4 kans"; break;
                            case "kaze4": Finder.DrawReason = 4; Comment = "only games which ended in a draw because of 4 winds"; break;
                            case "nm": Finder.DrawReason = 5; Comment = "only games which ended in a draw with nagashi mangan"; break;
                            default: Finder.DrawReason = -1; Comment = "only games which ended in a draw (no agari)"; break;
                        }

                        Console.WriteLine(String.Format("Filter: {0:s} ;", Comment));
                        break;
                }
            }

            return Finder.Find();
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

        static private int[] DecompositeIntList(string Text)
        {
            string[] delimiter = new string[] { "," };
            string[] Temp;
            int[] Result = null;

            if (Text == null) return null;

            Temp = Text.Split(delimiter, StringSplitOptions.None);
            Result = new int[Temp.Length];

            for (int i = 0; i < Temp.Length; i++)
            {
                // Отрежем текст до точки
                int Index = Temp[i].IndexOf('.');
                if (Index >= 0) Temp[i] = Temp[i].Substring(0, Index);

                Result[i] = Convert.ToInt32(Temp[i]);
            }

            return Result;
        }
    }
}
