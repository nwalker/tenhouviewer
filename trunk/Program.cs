using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace TenhouViewer
{
    class Program
    {
        static string LogDir = "logs";
        static string MjlogDir = "mjlog";

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

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -dLog.txt - download all games from log Log.txt;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -PHash - parse game;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -pLog.txt - parse all games from log Log.txt;");
            Console.WriteLine(" force - force parsing (don't skip exists files);");
            Console.WriteLine(" mjlog=D - check replays .mjlog in specified folder if exists (if not - from download folder);");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -fLog.txt - find games from log Log.txt with query:");
            Console.WriteLine(" lobby=N - find all games from specified lobby (0-6);");
            Console.WriteLine(" aka=N - only games with aka-dora nashi/ari (0-1);");
            Console.WriteLine(" kuitan=N - only games with aka-dora nashi/ari (0-1);");
            Console.WriteLine(" south=N - only games without/with south round (0-1);");
            Console.WriteLine(" speedy=N - only normal/high speed games (0-1);");
            Console.WriteLine(" dan -  only games from 1+dan lobby;");
            Console.WriteLine(" upperdan -  only games from upper dan lobby;");
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
            Console.WriteLine(" dangermin=N - find all hands which has danger tiles count greater (or equal) than N (0-14);");
            Console.WriteLine(" dangermax=N - find all hands which has danger tiles  count less (or equal) than N (0-14);");
            Console.WriteLine(" deadoutsmin=N - find all hands which has count of outs in dead wall greater (or equal) than N (1-14);");
            Console.WriteLine(" deadoutsmax=N - find all hands which has count of outs in dead wall less (or equal) than N (1-14);");
            Console.WriteLine(" place=N - find all players, who took N place (1-4);");
            Console.WriteLine(" rank=N - find all players, who has rank N (0-20);");
            Console.WriteLine(" nickname=N - find player, who has nickname N (string);");
            Console.WriteLine(" sex=N - find player of pecified sex N (char: f - female, m - male, c - computer);");
            Console.WriteLine(" hash=N - find game with hash N (string);");
            Console.WriteLine(" steps=N - find all hands, who exist less (or equal) than N steps (0-60);");
            Console.WriteLine(" yaku=N,M,X - find all hands, which has N,M,X,... yaku (0-54);");
            Console.WriteLine(" anyyaku=N,M,X - find all hands, which has any of N,M,X,... yaku (0-54 or name);");
            Console.WriteLine(" wait=N,M,X - find all hands, which has at least one tile from list in waiting: N,M,X,... ((1p,1p,2m,6z,...);");
            Console.WriteLine(" dealer=N - find all (not) dealer's hands (0-1);");
            Console.WriteLine(" winner=N - find all (not) completed hands (0-1);");
            Console.WriteLine(" loser=N - find all players (games), who (not) dealt into ron (0-1);");
            Console.WriteLine(" riichi=N - find all hands with declared (or not) riichi (0-1);");
            Console.WriteLine(" furiten=N - find all players (games), who has (has not) furiten (0-1);");
            Console.WriteLine(" ron=N - find all rounds ended with ron (0-1);");
            Console.WriteLine(" tsumo=N - find all rounds ended with tsumo (0-1);");
            Console.WriteLine(" tempai=N - find all hands, which has (not) tempai (0-1);");
            Console.WriteLine(" players=N - count of players in game (3-4);");
            Console.WriteLine(" roundwind=N - wind of current round (0-3);");
            Console.WriteLine(" round=N - index of current round (0:w1, 1:w2, 2:w3, 3: w4, 4: s1, ...);");
            Console.WriteLine(" playerwind=N - player's wind (0-3);");
            Console.WriteLine(" riichicount=N - find all rounds with N declared riichi (0-4);");
            Console.WriteLine(" renchanstick=N - find all rounds with N renchan stick (0-30);");
            Console.WriteLine(" nakicount=N - find all hands with N declared nakies (0-12);");
            Console.WriteLine(" openedsets=N - find all hands with N opened sets (0-4);");
            Console.WriteLine(" form=NNN - find all hands, which contains specified form in specified suits (numbers + suits m,p,s);");
            Console.WriteLine(" drowntiles=N,M,X - find all hands, in which player drown specified tiles (1p,1p,2m,6z,...);");
            Console.WriteLine(" color=m,p,s,M,P,S,0,1 - find all colored (>80% for lower case, 100% for upper case) hands;");
            Console.WriteLine(" omotesuji=N - find all hands which has (not) omote-suji waiting to discard (0-1);");
            Console.WriteLine(" senkisuji=N - find all hands which has (not) senki-suji waiting to discard (0-1);");
            Console.WriteLine(" urasuji=N - find all hands which has (not) ura-suji waiting to discard (0-1);");
            Console.WriteLine(" matagisuji=N - find all hands which has (not) matagi-suji waiting to discard (0-1);");
            Console.WriteLine(" draw=N - round ended in draw with reason (yao9,reach4,ron3,kan4,kaze4,nm);");
            Console.WriteLine(" last=N - find only last N games;");
            Console.WriteLine(" limit=N - find omaximum N games;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -g<nickname> <fields> - graph rounds (which found by -f flag) with fields:");
            Console.WriteLine(" lobby - lobby index;");
            Console.WriteLine(" index - round index in list;");
            Console.WriteLine(" players - player count;");
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
            Console.WriteLine(" roundwind - wind of current round;");
            Console.WriteLine(" playerwind - player's wind;");
            Console.WriteLine(" players - count of players in round;");
            Console.WriteLine(" draw - round ended in draw;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -G<nickname> <fields> - graph games (which found by -f flag) with fields:");
            Console.WriteLine(" index - game index in list;");
            Console.WriteLine(" lobby - lobby index;");
            Console.WriteLine(" rating - player rating before this game;");
            Console.WriteLine(" rank - player rank before this game (1=1ku, 10=1dan,...);");
            Console.WriteLine(" jrating - player rating before this game (1256R);");
            Console.WriteLine(" jrank - player rank before this game (四段);");
            Console.WriteLine(" place - place in game;");
            Console.WriteLine(" result - game result with uma;");
            Console.WriteLine(" balance - balance in the end of game;");
            Console.WriteLine(" players - count of players in game;");
            Console.WriteLine(" datetime - date of game;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -o<nickname> <fields> - format output results:");
            Console.WriteLine(" link - link to the round;");
            Console.WriteLine(" lobby - lobby index;");
            Console.WriteLine(" type - lobby type (aka,kui,nan,dan,...);");
            Console.WriteLine(" nickname - nickname of the player;");
            Console.WriteLine(" rating - rating of the player (number);");
            Console.WriteLine(" rank - rank of the player (number);");
            Console.WriteLine(" jrating - rating of the player (1256R);");
            Console.WriteLine(" jrank - rank of the player (四段);");
            Console.WriteLine(" place - place (result) in game;");
            Console.WriteLine(" pay - player payment in round;");
            Console.WriteLine(" dealer - is player dealer;");
            Console.WriteLine(" winner - is player complete hand;");
            Console.WriteLine(" loser - is player dealt in other player's hand;");
            Console.WriteLine(" concealed - is hand concealed;");
            Console.WriteLine(" tsumo - is tsumo-agari;");
            Console.WriteLine(" ron - is ron-agari ;");
            Console.WriteLine(" cost - cost of hand;");
            Console.WriteLine(" han - amount of game points in hand;");
            Console.WriteLine(" fu - amount of minipoints in hand;");
            Console.WriteLine(" waiting - amount of tile types in waiting;");
            Console.WriteLine(" step - amount of player steps in round;");
            Console.WriteLine(" yaku - list of yaku;");
            Console.WriteLine(" jyaku - list of yaku (japanese);");
            Console.WriteLine(" round - current round (0-1e, 1-2e, ...);");
            Console.WriteLine(" roundindex - index of round in game;");
            Console.WriteLine(" place - player's place in game;");
            Console.WriteLine(" roundwind - wind of current round;");
            Console.WriteLine(" playerwind - player's wind;");
            Console.WriteLine(" from - nickname of player who p[layed into ron;");
            Console.WriteLine(" furiten - furiten hand;");
            Console.WriteLine(" draw - round ended in draw;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -s<filename> - save find or graph result to specified file;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -U<hash> <params> - get paifu:");
            Console.WriteLine(" dir - directory to save result (for all rounds);");
            Console.WriteLine(" filename - filename to save result (for specified round, without extension);");
            Console.WriteLine(" round - round index (from 0);");
            Console.WriteLine(" shanten - add shanten info in paifu (+furiten marking; [0]-1);");
            Console.WriteLine(" yaku - add yaku and cost info in paifu (0-[1]);");
            Console.WriteLine(" nickname - add nicknames info in paifu instead A,B,C,D (0-[1]);");
            Console.WriteLine(" danger - highlight danger tiles (0-[1]);");
            Console.WriteLine(" color - mark shanten number by colorized rectangle (0-[1]);");
            Console.WriteLine(" sex - mark player's sex by color ([0]-1);");
            Console.WriteLine(" tileset=dir - alternate directory with tile images (folder name);");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -u <params> - get paifu for all rounds, which was found before:");
            Console.WriteLine(" dir - directory to save result (for all rounds);");
            Console.WriteLine(" shanten - add shanten info in paifu (+furiten marking; [0]-1);");
            Console.WriteLine(" yaku - add yaku and cost info in paifu (0-[1]);");
            Console.WriteLine(" nickname - add nicknames info in paifu instead A,B,C,D (0-[1]);");
            Console.WriteLine(" danger - highlight danger tiles (0-[1]);");
            Console.WriteLine(" color - mark shanten number by colorized rectangle (0-[1]);");
            Console.WriteLine(" sex - mark player's sex by color ([0]-1);");
            Console.WriteLine(" tileset=dir - alternate directory with tile images (folder name);");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -I<hash> <params> - get discard:");
            Console.WriteLine(" dir - directory to save result (for all rounds);");
            Console.WriteLine(" filename - filename to save result (for specified round, without extension);");
            Console.WriteLine(" round - round index (from 0);");
            Console.WriteLine(" player - player index (from 0);");
            Console.WriteLine(" riichi - limit discard to riichi declaration");
            Console.WriteLine(" naki - highlight tiles got other players;");
            Console.WriteLine(" tsumogiri - highlight tiles discarded from wall;");
            Console.WriteLine(" hand - output hand image for this discard;");
            Console.WriteLine(" tileset=dir - alternate directory with tile images (folder name);");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -i <params> - get discards for all rounds, which was found before:");
            Console.WriteLine(" dir - directory to save result (for all rounds);");
            Console.WriteLine(" riichi - limit discard to riichi declaration;");
            Console.WriteLine(" naki - highlight tiles got by other players;");
            Console.WriteLine(" tsumogiri - highlight tiles discarded from wall;");
            Console.WriteLine(" hand - output hand image for this discard;");
            Console.WriteLine(" tileset=dir - alternate directory with tile images (folder name);");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -t <params> - get results table for all rounds, which was found before (tournier mode):");
            Console.WriteLine(" mingames=N - only players, who has at least N games;");
            Console.WriteLine(" sort=N - sort by parameter (place,points,balance,loss,acq,rank,rating);");
            Console.WriteLine(" sortdesc=N - sort by parameter by descending (place,points,balance,loss,acq,rank,rating);");
            Console.WriteLine(" index - player's index (in table, from 1);");
            Console.WriteLine(" nickname - player's nickname;");
            Console.WriteLine(" rank - player's rank;");
            Console.WriteLine(" rating - player's rating;");
            Console.WriteLine(" games - player's games count;");
            Console.WriteLine(" placelist - player's places as string (1123412);");
            Console.WriteLine(" place - player's average place;");
            Console.WriteLine(" points - player's total points;");
            Console.WriteLine(" balance - player's balance (+uma);");
            Console.WriteLine(" ron - how many times player dealt in ron;");
            Console.WriteLine(" agari - player's completed hands count;");
            Console.WriteLine(" acq - player's total acquisitions;");
            Console.WriteLine(" loss - player's total losses;");
            Console.WriteLine(" 1st - percent of first place;");
            Console.WriteLine(" 2nd - percent of second place;");
            Console.WriteLine(" 3rd - percent of third place;");
            Console.WriteLine(" 4th - percent of fourth place;");

            Console.WriteLine("");
            Console.WriteLine("TenhouViewer -b<directory> - build log from directory's content with mjlog files;");
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
                    case "c":
                        // Convert to mjlog from log
                        // -cLog.txt
                        //ConvertLogToMJLog(ArgList[i].Value);
                        break;
                    case "C":
                        // Convert to mjlog by hash
                        // -C2013070808gm-0089-0000-2f83b7da
                        ConvertHashToMJLog(ArgList[i].Value);
                        break;
                    case "D":
                        // Download game by hash
                        // -D2013070808gm-0089-0000-2f83b7da
                        DownloadHash(ArgList[i].Value);
                        break;
                    case "d":
                        // Download games by log
                        // -dLog.txt
                        DownloadLog(ArgList[i].Value, ArgList[i].Arguments);
                        break;
                    case "P":
                        // Parse game by hash
                        // -P2013070808gm-0089-0000-2f83b7da
                        ParseHash(ArgList[i].Value);
                        break;
                    case "p":
                        // Parse games by log
                        // -pLog.txt
                        ParseLog(ArgList[i].Value, ArgList[i].Arguments);
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
                    case "i":
                        // Discards by find results
                        GraphResult = null;
                        FindResult = null;
                        CreateDiscardList(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                    case "I":
                        // Discard by hash
                        CreateDiscard(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                    case "t":
                        // Analyze find results as tournier games list
                        FindResult = AnalyzeTournier(ArgList[i].Value, ArgList[i].Arguments, ResultList);
                        break;
                    case "b":
                        // Build log from directory's content with mjlog files
                        FindResult = BuildLogFromMjlogFiles(ArgList[i].Value, ArgList[i].Arguments);
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

        static List<string> BuildLogFromMjlogFiles(string Argument, List<Argument> ArgList)
        {
            string Dir = Argument;
            List<string> Result = new List<string>();

            if (!Directory.Exists(Dir))
            {
                Console.WriteLine(String.Format("Error: directory '{0:s}' not found", Dir));
                return null;
            }

            string[] FileList = Directory.GetFiles(Dir);

            for (int i = 0; i < FileList.Length; i++)
            {
                string FileName = FileList[i];

                if (FileName.IndexOf(".mjlog") >= 0)
                {
                    // This is mjlog file

                    string Link = String.Format("http://tenhou.net/0/?log={0:s}", Path.GetFileNameWithoutExtension(FileName));
                    Result.Add(Link);
                }
            }

            return Result;
        }

        static List<string> AnalyzeTournier(string Argument, List<Argument> ArgList, List<Search.Result> Results)
        {
            Tournier.Tournier T = new Tournier.Tournier(Results);
            List<Tournier.Result> ResultList = T.Analyze();

            Tournier.Plotter Plotter = new Tournier.Plotter(ResultList);

            // Fields analyze
            for (int i = 0; i < ArgList.Count; i++) Plotter.Fields.Add(ArgList[i].Name);

            // Parse options
            foreach (Argument A in ArgList)
            {
                int TempValue;

                switch (A.Name)
                {
                    case "mingames":
                        TempValue = ParseIntArg(A.Value, 0, 1000, "mingames");
                        if (TempValue != -1)
                        {
                            Plotter.MinimalGamesCount = TempValue;
                            Console.WriteLine(String.Format("Filter: players, who has at least {0:d} games;", TempValue));
                        }
                        break;
                    case "sort":
                        Plotter.Sort(A.Value);
                        break;
                    case "sortdesc":
                        Plotter.SortDescending(A.Value);
                        break;
                }
            }

            return Plotter.GamesGraph();
        }

        static void CreateDiscard(string Hash, List<Argument> ArgList, List<Search.Result> Results)
        {
            int Round = -1;
            int Player = -1;
            bool RiichiLimit = false;
            bool NakiHL = false;
            bool TsumogiriHL = false;
            bool ShowHand = false;

            string FN = null;
            string Dir = "discard";

            if (Hash.CompareTo("") == 0)
            {
                Console.WriteLine("Error: Hash not defined.");
                return;
            }

            Hash = new Tenhou.TenhouHash(Hash).DecodedHash;

            // Parse options
            foreach (Argument A in ArgList)
            {
                switch (A.Name)
                {
                    case "tileset":
                        Paifu.PaifuTileImage.TilesDirectory = A.Value;
                        break;
                    case "dir":
                        Dir = A.Value;
                        break;
                    case "round":
                        Round = Convert.ToInt32(A.Value);
                        break;
                    case "player":
                        Player = Convert.ToInt32(A.Value);
                        break;
                    case "filename":
                        FN = A.Value;
                        break;
                    case "riichi":
                        RiichiLimit = true;
                        break;
                    case "naki":
                        NakiHL = true;
                        break;
                    case "tsumogiri":
                        TsumogiriHL = true;
                        break;
                    case "hand":
                        ShowHand = true;
                        break;
                    default:
                        break;
                }
            }

            Mahjong.Replay R = new Mahjong.Replay();
            R.LoadXml(Hash);

            for (int i = 0; i < R.Rounds.Count; i++)
            {
                if ((Round != -1) && (Round != i)) continue;

                for (int p = 0; p < R.PlayerCount; p++)
                {
                    if ((Player != -1) && (Player != p)) continue;

                    Discarder.Discard D = new Discarder.Discard(R, i, p);
                    string FileName, HandFileName;
                    if (FN == null)
                    {
                        if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
                        FileName = String.Format("./{0:s}/{1:s}_{2:d}_{3:d}.png", Dir, Hash, Round, p);
                        HandFileName = String.Format("./{0:s}/{1:s}_{2:d}_{3:d}_hand.png", Dir, Hash, Round, p);
                    }
                    else
                    {
                        FileName = (Round == -1) ? String.Format("{0:s}_{1:d}_{2:d}.png", FN, i, p) : String.Format("{0:s}.png", FN);
                        HandFileName = (Round == -1) ? String.Format("{0:s}_{1:d}_{2:d}_hand.png", FN, i, p) : String.Format("{0:s}_hand.png", FN);
                    }

                    D.RiichiLimit = RiichiLimit;
                    D.HighlightNaki = NakiHL;
                    D.HighlightTsumogiri = TsumogiriHL;

                    D.Generate();
                    D.Save(FileName);

                    if (ShowHand)
                    {
                        Discarder.HandOutput HO = new Discarder.HandOutput(R, i, p);
                        HO.Generate();
                        HO.Save(HandFileName);
                    }
                }
            }
        }

        static void CreateDiscardList(string Argument, List<Argument> ArgList, List<Search.Result> Results)
        {
            string Dir = "discard";
            bool RiichiLimit = false;
            bool NakiHL = false;
            bool TsumogiriHL = false;
            bool ShowHand = false;

            // Parse options
            foreach (Argument A in ArgList)
            {
                switch (A.Name)
                {
                    case "tileset":
                        Paifu.PaifuTileImage.TilesDirectory = A.Value;
                        break;
                    case "dir":
                        Dir = A.Value;
                        if (!Directory.Exists(Dir))
                            Directory.CreateDirectory(Dir);
                        break;
                    case "riichi":
                        RiichiLimit = true;
                        break;
                    case "naki":
                        NakiHL = true;
                        break;
                    case "tsumogiri":
                        TsumogiriHL = true;
                        break;
                    case "hand":
                        ShowHand = true;
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < Results.Count; i++)
            {
                Search.Result R = Results[i];

                Console.Title = String.Format("Discard creating {0:d}/{1:d}", i + 1, Results.Count);

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    if (!R.RoundMark[r]) continue;

                    for (int p = 0; p < R.Replay.PlayerCount; p++)
                    {
                        if (!R.HandMark[r][p]) continue;

                        Discarder.Discard D = new Discarder.Discard(R.Replay, r, p);

                        string FileName = String.Format("./{0:s}/{1:s}_{2:d}_{3:d}.png", Dir, R.Replay.Hash, r, p);
                        string HandFileName = String.Format("./{0:s}/{1:s}_{2:d}_{3:d}_hand.png", Dir, R.Replay.Hash, r, p);

                        D.RiichiLimit = RiichiLimit;
                        D.HighlightNaki = NakiHL;
                        D.HighlightTsumogiri = TsumogiriHL;

                        D.Generate();
                        D.Save(FileName);

                        if (ShowHand)
                        {
                            Discarder.HandOutput HO = new Discarder.HandOutput(R.Replay, r, p);
                            HO.Generate();
                            HO.Save(HandFileName);
                        }
                    }
                }
            }
        }

        static void CreatePaifu(string Hash, List<Argument> ArgList, List<Search.Result> Results)
        {
            string Dir = "paifu";
            string FN = null;
            int Round = -1;
            int ShowShanten = 0;
            int ShowYaku = 1;
            int ShowNames = 1;
            int ShowDanger = 1;
            int ShowColor = 1;
            int ShowSex = 0;

            if (Hash.CompareTo("") == 0)
            {
                Console.WriteLine("Error: Hash not defined.");
                return;
            }

            Hash = new Tenhou.TenhouHash(Hash).DecodedHash;

            // Parse options
            foreach (Argument A in ArgList)
            {
                switch (A.Name)
                {
                    case "tileset":
                        Paifu.PaifuTileImage.TilesDirectory = A.Value;
                        break;
                    case "dir":
                        Dir = A.Value;
                        break;
                    case "round":
                        Round = Convert.ToInt32(A.Value);
                        break;
                    case "filename":
                        FN = A.Value;
                        break;
                    case "shanten":
                        ShowShanten = ParseBoolArg(A.Value, "shanten");
                        break;
                    case "yaku":
                        ShowYaku = ParseBoolArg(A.Value, "yaku");
                        break;
                    case "nickname":
                        ShowNames = ParseBoolArg(A.Value, "nickname");
                        break;
                    case "danger":
                        ShowDanger = ParseBoolArg(A.Value, "danger");
                        break;
                    case "color":
                        ShowColor = ParseBoolArg(A.Value, "color");
                        break;
                    case "sex":
                        ShowSex = ParseBoolArg(A.Value, "sex");
                        break;
                }
            }

            Mahjong.Replay R = new Mahjong.Replay();
            R.LoadXml(Hash);

            for (int i = 0; i < R.Rounds.Count; i++)
            {
                if ((Round != -1) && (Round != i)) continue;

                Paifu.PaifuGenerator P = new Paifu.PaifuGenerator(R, i);

                string FileName;

                if (FN == null)
                {
                    if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
                    FileName = String.Format("./{0:s}/{1:s}_{2:d}.png", Dir, Hash, i);
                }
                else
                {
                    FileName = (Round == -1) ? String.Format("{0:s}_{1:d}.png", FN, i) : String.Format("{0:s}.png", FN);
                }

                P.ShowShanten = ShowShanten;
                P.ShowDanger = ShowDanger;
                P.ShowYakuInfo = ShowYaku;
                P.ShowNames = ShowNames;
                P.ShowColor = ShowColor;
                P.ShowSex = ShowSex;

                P.Generate();
                P.Save(FileName);
            }
        }

        static void ConvertHashToMJLog(string Hash)
        {
            string FileName = LogDir + "/" + Hash + ".xml";

            if (!Directory.Exists("mjlog")) Directory.CreateDirectory("mjlog");

            if (File.Exists(FileName))
            {
                string ResultFN = MjlogDir + "/" + Hash + ".mjlog";
                byte[] bytes = System.IO.File.ReadAllBytes(FileName);

                using (FileStream MjLog = File.Create(ResultFN))
                {
                    using (GZipStream compressionStream = new GZipStream(MjLog, CompressionMode.Compress))
                    {
                        compressionStream.Write(bytes, 0, bytes.Length);

                        Console.WriteLine(" - ok!");
                    }
                }
            }
            else
            {
                Console.WriteLine(" - not found!");
            }
        }

        static void CreatePaifuList(string Argument, List<Argument> ArgList, List<Search.Result> Results)
        {
            string Dir = "paifu";
            int ShowShanten = 0;
            int ShowYaku = 1;
            int ShowNames = 1;
            int ShowDanger = 1;
            int ShowColor = 1;
            int ShowSex = 0;

            // Parse options
            foreach (Argument A in ArgList)
            {
                switch (A.Name)
                {
                    case "tileset":
                        Paifu.PaifuTileImage.TilesDirectory = A.Value;
                        break;
                    case "dir":
                        Dir = A.Value;
                        if (!Directory.Exists(Dir))
                            Directory.CreateDirectory(Dir);
                        break;
                    case "shanten":
                        ShowShanten = ParseBoolArg(A.Value, "shanten");
                        break;
                    case "yaku":
                        ShowYaku = ParseBoolArg(A.Value, "yaku");
                        break;
                    case "nickname":
                        ShowNames = ParseBoolArg(A.Value, "nickname");
                        break;
                    case "danger":
                        ShowDanger = ParseBoolArg(A.Value, "danger");
                        break;
                    case "color":
                        ShowColor = ParseBoolArg(A.Value, "color");
                        break;
                    case "sex":
                        ShowSex = ParseBoolArg(A.Value, "sex");
                        break;
                }
            }

            for (int i = 0; i < Results.Count; i++)
            {
                Search.Result R = Results[i];

                Console.Title = String.Format("Paifu creating {0:d}/{1:d}", i + 1, Results.Count);

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    if (!R.RoundMark[r]) continue;

                    Paifu.PaifuGenerator P = new Paifu.PaifuGenerator(R.Replay, r);

                    string FileName = String.Format("./{0:s}/{1:s}_{2:d}.png", Dir, R.Replay.Hash, r);

                    P.ShowShanten = ShowShanten;
                    P.ShowDanger = ShowDanger;
                    P.ShowYakuInfo = ShowYaku;
                    P.ShowNames = ShowNames;
                    P.ShowColor = ShowColor;
                    P.ShowSex = ShowSex;

                    P.Generate();
                    P.Save(FileName);
                }
            }
        }

        static int CountDangerous(Mahjong.Round Rnd, int Player)
        {
            int MaxCount = 0;

            for (int j = 0; j < Rnd.Steps.Count; j++)
            {
                int[] D = Rnd.Steps[j].Danger;
                if (D == null) continue;
                if (Rnd.Steps[j].Player != Player) continue;

                if (D.Length > MaxCount) MaxCount = D.Length;
            }

            return MaxCount;
        }

        static bool HasFuriten(Mahjong.Round Rnd, int Player)
        {
            for (int j = 0; j < Rnd.Steps.Count; j++)
            {
                if (Rnd.Steps[j].Player != Player) continue;

                if (Rnd.Steps[j].Furiten) return true;
            }

            return false;
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
                                    case "jrating":
                                        Temp += String.Format("{0:d}R\t", R.Replay.Players[k].Rating);
                                        break;
                                    case "jrank":
                                        Temp += String.Format("{0:s}\t", Tenhou.Rank.GetName(R.Replay.Players[k].Rank));
                                        break;
                                    case "pay":
                                        Temp += String.Format("{0:d}\t", Rnd.Pay[k]);
                                        break;
                                    case "dealer":
                                        Temp += String.Format("{0:d}\t", Rnd.Dealer[k] ? 1 : 0);
                                        break;
                                    case "tsumo":
                                        Temp += String.Format("{0:d}\t", (Rnd.Winner[k] && (GetFirstNotNullIndex(Rnd.Loser) == -1)) ? 1 : 0);
                                        break;
                                    case "ron":
                                        Temp += String.Format("{0:d}\t", (Rnd.Winner[k] && (GetFirstNotNullIndex(Rnd.Loser) != -1)) ? 1 : 0);
                                        break;
                                    case "from":
                                        Temp += String.Format("{0:s}\t", (Rnd.Winner[k] && (GetFirstNotNullIndex(Rnd.Loser) != -1)) ? R.Replay.Players[GetFirstNotNullIndex(Rnd.Loser)].NickName : "");
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
                                    case "fu":
                                        Temp += String.Format("{0:d}\t", Rnd.FuCount[k]);
                                        break;
                                    case "waiting":
                                        Temp += String.Format("{0:d}\t", Rnd.WinWaiting[k].Count);
                                        break;
                                    case "step":
                                        Temp += String.Format("{0:d}\t", Rnd.StepCount[k]);
                                        break;
                                    case "yaku":
                                        Temp += String.Format("{0:s}\t", YakuList(Rnd.Yaku[k], "en"));
                                        break;
                                    case "jyaku":
                                        Temp += String.Format("{0:s}\t", YakuList(Rnd.Yaku[k], "jp"));
                                        break;
                                    case "round":
                                        Temp += String.Format("{0:d}\t", Rnd.CurrentRound);
                                        break;
                                    case "lobby":
                                        Temp += String.Format("{0:d}\t", Rnd.Lobby);
                                        break;
                                    case "type":
                                        Temp += String.Format("{0:s}\t", Tenhou.LobbyType.GetText(Rnd.LobbyType));
                                        break;
                                    case "roundindex":
                                        Temp += String.Format("{0:d}\t", r);
                                        break;
                                    case "roundwind":
                                        Temp += String.Format("{0:s}\t", Tenhou.Wind.GetText(Rnd.CurrentRound / 4));
                                        break;
                                    case "playerwind":
                                        Temp += String.Format("{0:s}\t", Tenhou.Wind.GetText(Rnd.Wind[k]));
                                        break;
                                    case "players":
                                        Temp += String.Format("{0:d}\t", Rnd.PlayerCount);
                                        break;
                                    case "danger":
                                        Temp += String.Format("{0:d}\t", CountDangerous(Rnd, k));
                                        break;
                                    case "furiten":
                                        Temp += String.Format("{0:d}\t", HasFuriten(Rnd, k) ? 1 : 0);
                                        break;
                                    case "draw":
                                        Temp += String.Format("{0:d}\t", (Rnd.Result == Mahjong.RoundResult.Draw) ? 1 : 0);
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

            if (Results == null) return Output;

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
                                              R.Replay.Players[k].NickName, YakuList(Rnd.Yaku[k], "en")));
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

        static void DownloadLog(string FileName, List<Argument> ArgList)
        {
            string Dir = LogDir;

            foreach (Argument A in ArgList)
            {
                switch (A.Name)
                {
                    case "dir":
                        Dir = A.Value + "/" + LogDir;
                        if (!Directory.Exists(Dir))
                            Directory.CreateDirectory(Dir);
                        break;

                }
            }

            Console.WriteLine("Downloading games from log: " + FileName);

            if (!File.Exists(FileName))
            {
                Console.Write("Error: Log file " + FileName + " not found!");
                return;
            }

            Tenhou.LogParser Log = new Tenhou.LogParser(FileName);
            Log.DownloadAll(Dir);
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

        static string GetMjlogFilename(string[] Files, string Hash)
        {
            if (Files == null) return null;

            foreach (string FileName in Files)
            {
                if (FileName.IndexOf(Hash) >= 0) return FileName;
            }

            return null;
        }

        static void ParseLog(string FileName, List<Argument> ArgList)
        {
            string Dir = LogDir;
            string[] MjlogDir = null;
            string Mjlog = null;
            bool Forced = false;
            bool Rewrited;

            foreach (Argument A in ArgList)
            {
                switch (A.Name)
                {
                    case "dir":
                        Dir = A.Value + "/" + LogDir;
                        if (!Directory.Exists(Dir))
                            Directory.CreateDirectory(Dir);
                        break;
                    case "force":
                        Forced = true;
                        break;
                    case "mjlog":
                        {
                            Mjlog = A.Value;
                            if(Directory.Exists(Mjlog)) MjlogDir = Directory.GetFiles(Mjlog);
                        }
                        break;
                }
            }

            Console.WriteLine("Parsing games from log: " + FileName);

            if (!File.Exists(FileName))
            {
                Console.Write("Error: Log file " + FileName + " not found!");
                return;
            }

            Tenhou.LogParser Log = new Tenhou.LogParser(FileName);
            List<string> Hashes = Log.HashList.Hashes;

            for (int i = 0; i < Hashes.Count; i++)
            {
                string Hash = Hashes[i];
                string ReplayFileName = Log.GetFileName(Hash, Dir);

                Console.Title = String.Format("Parsing {0:d}/{1:d}", i + 1, Hashes.Count);
                Console.Write(Hash);

                Rewrited = false;

                if (Mahjong.Replay.IsReplayExist(Hash))
                {
                    if (!Forced)
                    {
                        Console.WriteLine(" - exists, skip!");
                        continue;
                    }
                    else
                    {
                        Rewrited = true;
                    }
                }


                string MjLogFile = GetMjlogFilename(MjlogDir, Hash);
                Tenhou.ReplayDecoder R = new Tenhou.ReplayDecoder();

                if (MjLogFile != null)
                {
                    R.OpenGZ(MjLogFile, Hash);
                }
                else
                {
                    if (!File.Exists(ReplayFileName))
                    {
                        Console.WriteLine(" - file not found!");
                        continue;
                    }

                    if (new FileInfo(ReplayFileName).Length == 0)
                    {
                        File.Delete(ReplayFileName);
                        Console.WriteLine(" - zero size, removed!");
                        continue;
                    }

                    R.OpenPlainText(ReplayFileName, Hash);
                }
                // replay (calc shanten, waitings and other) and save result
                Mahjong.Replay Replay = R.R;
                Replay.ReplayGame();
                Replay.Save();

                Console.WriteLine(Rewrited ? " - replaced, ok!" : " - ok!");
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
                    case "dangermin":
                        TempValue = ParseIntArg(Value, 0, 14, "dangermin");
                        if (TempValue != -1) Finder.DangerMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has danger tiles count greater (or equal) than {0:d};", TempValue));
                        break;
                    case "dangermax":
                        TempValue = ParseIntArg(Value, 0, 14, "dangermax");
                        if (TempValue != -1) Finder.DangerMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has has danger tiles count less (or equal) than {0:d};", TempValue));
                        break;
                    case "doramin":
                        TempValue = ParseIntArg(Value, 0, 35, "doramin");
                        if (TempValue != -1) Finder.DoraMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has dora count greater (or equal) than {0:d};", TempValue));
                        break;
                    case "doramax":
                        TempValue = ParseIntArg(Value, 0, 35, "doramax");
                        if (TempValue != -1) Finder.DoraMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has dora count less (or equal) than {0:d};", TempValue));
                        break;
                    case "deadoutsmin":
                        TempValue = ParseIntArg(Value, 0, 14, "deadoutsmin");
                        if (TempValue != -1) Finder.DeadOutsMin = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has count of outs in dead wall greater (or equal) than {0:d};", TempValue));
                        break;
                    case "deadoutsmax":
                        TempValue = ParseIntArg(Value, 0, 14, "deadoutsmax");
                        if (TempValue != -1) Finder.DeadOutsMax = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands, which has count of outs in dead wall less (or equal) than {0:d};", TempValue));
                        break;
                    case "furiten":
                        Finder.Furiten = ParseBoolArg(Value, "furiten");

                        Console.WriteLine(String.Format("Filter: only hands, which has {0:s} furiten;", (Finder.Furiten == 0) ? "no" : ""));
                        break;
                    case "riichi":
                        Finder.Riichi = ParseBoolArg(Value, "riichi");

                        Console.WriteLine(String.Format("Filter: only hands {0:s} riichi;", (Finder.Riichi == 0) ? "without" : "with"));
                        break;
                    case "ron":
                        Finder.Ron = ParseBoolArg(Value, "ron");

                        Console.WriteLine(String.Format("Filter: only rounds, which ended {0:s} ron agari;", (Finder.Ron == 0) ? "without" : "with"));
                        break;
                    case "tsumo":
                        Finder.Tsumo = ParseBoolArg(Value, "tsumo");

                        Console.WriteLine(String.Format("Filter: only rounds, which ended {0:s} tsumo agari;", (Finder.Tsumo == 0) ? "without" : "with"));
                        break;
                    case "riichicount":
                        TempValue = ParseIntArg(Value, 0, 4, "riichicount");
                        if (TempValue != -1) Finder.RiichiCount = TempValue;

                        Console.WriteLine(String.Format("Filter: only rounds with {0:d} declared riichi;", Finder.RiichiCount));
                        break;
                    case "nakicount":
                        TempValue = ParseIntArg(Value, 0, 12, "nakicount");
                        if (TempValue != -1) Finder.NakiCount = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands with {0:d} declared nakies;", Finder.NakiCount));
                        break;
                    case "openedsets":
                        TempValue = ParseIntArg(Value, 0, 4, "openedsets");
                        if (TempValue != -1) Finder.OpenedSets = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands with {0:d} opened sets;", Finder.OpenedSets));
                        break;
                    case "renchanstick":
                        TempValue = ParseIntArg(Value, 0, 15, "renchanstick");
                        if (TempValue != -1) Finder.RenchanStick = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with {0:d} renchan sticks;", Tenhou.Wind.GetText(TempValue)));
                        break;
                    case "lobby":
                        TempValue = ParseIntArg(Value, 0, 9999, "lobby");
                        if (TempValue != -1) Finder.Lobby = TempValue;

                        Console.WriteLine(String.Format("Filter: only games from {0:d} lobby;", TempValue));
                        break;
                    case "aka":
                        TempValue = ParseIntArg(Value, 0, 1, "aka");
                        if (TempValue != -1) Finder.Aka = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with aka-dora {0:s};", (TempValue == 0) ? "nashi" : "ari"));
                        break;
                    case "kuitan":
                        TempValue = ParseIntArg(Value, 0, 1, "kuitan");
                        if (TempValue != -1) Finder.Kuitan = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with kuitan {0:s};", (TempValue == 0) ? "nashi" : "ari"));
                        break;
                    case "south":
                        TempValue = ParseIntArg(Value, 0, 1, "south");
                        if (TempValue != -1) Finder.Kuitan = TempValue;

                        Console.WriteLine(String.Format("Filter: only {0:s} games;", (TempValue == 0) ? "tonpuusen" : "hanchan"));
                        break;
                    case "speedy":
                        TempValue = ParseIntArg(Value, 0, 1, "speedy");
                        if (TempValue != -1) Finder.Saku = TempValue;

                        Console.WriteLine(String.Format("Filter: only {0:s} speed games;", (TempValue == 0) ? "normal" : "high"));
                        break;
                    case "dan":
                        Finder.High = 1;

                        Console.WriteLine(String.Format("Filter: only games from 1+dan lobby;"));
                        break;
                    case "upperdan":
                        Finder.Toku = 1;

                        Console.WriteLine(String.Format("Filter: only games from upper dan lobby;"));
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
                                Console.WriteLine(String.Format("Filter: only hands with yaku '{0:s}';", Mahjong.YakuName.GetYakuName("en", Finder.YakuList[j])));
                            }
                        }
                        break;
                    case "anyyaku":
                        string[] TempYakuList = DecompositeStringList(Value);
                        Finder.AnyYakuList = Tenhou.YakuNameParser.Parse(TempYakuList);

                        if (Finder.AnyYakuList != null)
                        {
                            for (int j = 0; j < Finder.AnyYakuList.Length; j++)
                            {
                                Console.WriteLine(String.Format("Filter: only hands with yaku '{0:s}';", Mahjong.YakuName.GetYakuName("en", Finder.AnyYakuList[j])));
                            }
                        }
                        break;
                    case "wait":
                        Finder.Waitings = ParseTilesList(Value);

                        if (Finder.Waitings != null)
                        {
                            Console.WriteLine(String.Format("Filter: only hands, which has at least one waiting from list: '{0:s}';", GetTilesListString(Finder.Waitings)));
                        }
                        break;
                    case "nickname":
                        Finder.NickName = Value;

                        Console.WriteLine(String.Format("Filter: only player with nickname '{0:s}';", Value));
                        break;
                    case "hash":
                        Finder.Hash = Value;

                        Console.WriteLine(String.Format("Filter: only rounds from game with hash '{0:s}';", Value));
                        break;
                    case "dealer":
                        Finder.Dealer = ParseBoolArg(Value, "dealer");

                        Console.WriteLine(String.Format("Filter: only {0:s} dealer hands;", (Finder.Dealer == 0) ? "not" : ""));
                        break;
                    case "loser":
                        Finder.Loser = ParseBoolArg(Value, "loser");

                        Console.WriteLine(String.Format("Filter: only players, who {0:s} dealt into ron;", (Finder.Loser == 0) ? "not" : ""));
                        break;
                    case "winner":
                        Finder.Winner = ParseBoolArg(Value, "winner");

                        Console.WriteLine(String.Format("Filter: only players, who {0:s} completed hand;", (Finder.Winner == 0) ? "not" : ""));
                        break;
                    case "tempai":
                        Finder.Tempai = ParseBoolArg(Value, "tempai");

                        Console.WriteLine(String.Format("Filter: only hands, which {0:s} had tempai;", (Finder.Tempai == 0) ? "not" : ""));
                        break;
                    case "omotesuji":
                        Finder.OmoteSujiWait = ParseBoolArg(Value, "omotesuji");

                        Console.WriteLine(String.Format("Filter: only hands, which had {0:s}omote-suji wait to discarded tile;", (Finder.OmoteSujiWait == 0) ? "not " : ""));
                        break;
                    case "suji":
                        Finder.OmoteSujiWait = ParseBoolArg(Value, "suji");

                        Console.WriteLine(String.Format("Filter: only hands, which had {0:s}(omote)suji wait to discarded tile;", (Finder.OmoteSujiWait == 0) ? "not " : ""));
                        break;
                    case "senkisuji":
                        Finder.SenkiSujiWait = ParseBoolArg(Value, "senkisuji");

                        Console.WriteLine(String.Format("Filter: only hands, which had {0:s}senki-suji wait to discarded tile;", (Finder.SenkiSujiWait == 0) ? "not " : ""));
                        break;
                    case "urasuji":
                        Finder.UraSujiWait = ParseBoolArg(Value, "urasuji");

                        Console.WriteLine(String.Format("Filter: only hands, which had {0:s}ura-suji wait to discarded tile;", (Finder.UraSujiWait == 0) ? "not " : ""));
                        break;
                    case "matagisuji":
                        Finder.MatagiSujiWait = ParseBoolArg(Value, "matagisuji");

                        Console.WriteLine(String.Format("Filter: only hands, which had {0:s}matagi-suji wait to discarded tile;", (Finder.MatagiSujiWait == 0) ? "not " : ""));
                        break;
                    case "karatennoten":
                        Finder.KaratenNoten = ParseBoolArg(Value, "karatennoten");

                        Console.WriteLine(String.Format("Filter: only hands, which had {0:s} only waiting, all tiles of that type already in hand;", (Finder.KaratenNoten == 0) ? "not" : ""));
                        break;
                    case "players":
                        TempValue = ParseIntArg(Value, 3, 4, "players");
                        if (TempValue != -1) Finder.PlayerCount = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with {0:d} players;", TempValue));
                        break;
                    case "roundwind":
                        TempValue = ParseIntArg(Value, 0, 3, "roundwind");
                        if (TempValue != -1) Finder.RoundWind = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with round wind {0:s};", Tenhou.Wind.GetText(TempValue)));
                        break;
                    case "playerwind":
                        TempValue = ParseIntArg(Value, 0, 3, "playerwind");
                        if (TempValue != -1) Finder.PlayerWind = TempValue;

                        Console.WriteLine(String.Format("Filter: only hands with player wind {0:s};", Tenhou.Wind.GetText(TempValue)));
                        break;
                    case "round":
                        TempValue = ParseIntArg(Value, 0, 15, "round");
                        if (TempValue != -1) Finder.RoundIndex = TempValue;

                        Console.WriteLine(String.Format("Filter: only games with round index {0:s};", Tenhou.Wind.GetText(TempValue)));
                        break;
                    case "draw":
                        {
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
                        }
                        break;
                    case "form":
                        Finder.Form = ParseForm(Value);

                        Console.WriteLine(String.Format("Filter: only hands with form '{0:s}';", Value));
                        break;
                    case "drowntiles":
                        Finder.DrownTiles = ParseTilesList(Value);

                        if(Finder.DrownTiles != null)
                        {
                            Console.WriteLine(String.Format("Filter: only hands with drown tiles '{0:s}';", GetTilesListString(Finder.DrownTiles)));
                        }
                        break;
                    case "sex":
                        {
                            string Comment = "";
                            bool Skip = false;

                            switch (Value)
                            {
                                case "f": Finder.Sex = Mahjong.Sex.Female; Comment = "only female players"; break;
                                case "m": Finder.Sex = Mahjong.Sex.Male; Comment = "only male players"; break;
                                case "c": Finder.Sex = Mahjong.Sex.Computer; Comment = "only bot players (computer)"; break;
                                default:
                                    Console.WriteLine("Invalid 'sex' query - skipped, should be 'f', 'm' or 'c';");
                                    Skip = true;
                                    break;
                            }

                            if(!Skip) Console.WriteLine(String.Format("Filter: {0:s} ;", Comment));
                        }
                        break;
                    case "color":
                        {
                            string Comment = "";

                            switch (Value)
                            {
                                case "m": Finder.Colored = 1; Finder.ColoredSuit = 0; Comment = "only man-colored hands"; break;
                                case "p": Finder.Colored = 1; Finder.ColoredSuit = 1; Comment = "only pin-colored hands"; break;
                                case "s": Finder.Colored = 1; Finder.ColoredSuit = 2; Comment = "only sou-colored hands"; break;
                                case "M": Finder.Colored = 1; Finder.ColoredSuit = 0; Finder.ColoredForced = 1; Comment = "only man-colored hands (without other suits)"; break;
                                case "P": Finder.Colored = 1; Finder.ColoredSuit = 1; Finder.ColoredForced = 1; Comment = "only pin-colored hands (without other suits)"; break;
                                case "S": Finder.Colored = 1; Finder.ColoredSuit = 2; Finder.ColoredForced = 1; Comment = "only sou-colored hands (without other suits)"; break;
                                case "0": Finder.Colored = 0; Comment = "only not colored hands"; break;
                                default: Finder.Colored = 1; Comment = "only colored hands"; break;
                            }

                            Console.WriteLine(String.Format("Filter: {0:s} ;", Comment));
                        }
                        break;
                    case "last":
                        TempValue = ParseIntArg(Value, 0, 10000000, "last");
                        if (TempValue != -1) Finder.Last = TempValue;

                        Console.WriteLine(String.Format("Filter: only in {0:d} last games;", TempValue));
                        break;
                    case "limit":
                        TempValue = ParseIntArg(Value, 0, 10000000, "limit");
                        if (TempValue != -1) Finder.Limit = TempValue;

                        Console.WriteLine(String.Format("Filter: return maximum {0:d} games;", TempValue));
                        break;
                }
            }

            return Finder.Find();
        }

        static string GetTilesListString(int[] Tiles)
        {
            if (Tiles.Length == 0) return "";

            List<string> StrDrownTiles = new List<string>();
            for (int k = 0; k < Tiles.Length; k++)
            {
                int TileCount = Tiles[k];

                if (TileCount == 0) continue;

                int TileType = k / 10;
                int TileValue = k % 10;

                string StringName = "";
                switch (TileType)
                {
                    case 0: StringName = TileValue.ToString() + "m"; break;
                    case 1: StringName = TileValue.ToString() + "p"; break;
                    case 2: StringName = TileValue.ToString() + "s"; break;
                    case 3: StringName = TileValue.ToString() + "z"; break;
                }

                for (int l = 0; l < TileCount; l++) StrDrownTiles.Add(StringName);
            }

            return String.Join(",", StrDrownTiles.ToArray());
        }

        static private string YakuList(List<Mahjong.Yaku> Yaku, string Lang)
        {
            string Text = "";

            for (int i = 0; i < Yaku.Count; i++)
            {
                string Name = Mahjong.YakuName.GetYakuName(Lang, Yaku[i].Index);

                if (Yaku[i].Index >= 52) // Dora
                {
                    switch (Lang)
                    {
                        case "en": Text += String.Format("{0:s} {1:d} ", Name, Yaku[i].Cost); break;
                        case "jp": Text += String.Format("{0:s}{1:d} ", Name, Yaku[i].Cost); break;
                    }
                }
                else
                {
                    Text += String.Format("{0:s} ", Name);
                }
            }

            return Text;
        }

        private static int[] ParseForm(string Value)
        {
            int[] Form = new int[14]; // 11 for tiles (exclude 0, 10), 3 for suit flags
            int MinNumber = 11;
            int MaxNumber = 0;

            bool[] SuitFilter = new bool[3];

            for (int i = 0; i < SuitFilter.Length; i++) SuitFilter[i] = false;
            for (int i = 0; i < Form.Length; i++) Form[i] = -1;

            Value = Value.ToLower();

            foreach (char C in Value)
            {
                if ((C >= '1') && (C <= '9'))
                {
                    int Number = (C - '0');

                    if (Number < MinNumber) MinNumber = Number;
                    if (Number > MaxNumber) MaxNumber = Number;

                    if (Form[Number] == -1)
                        Form[Number] = 1;
                    else
                        Form[Number]++;
                }
                if (C == 'm') SuitFilter[0] = true;
                if (C == 'p') SuitFilter[1] = true;
                if (C == 's') SuitFilter[2] = true;
            }

            if (MaxNumber != 0)
            {
                Form[MinNumber - 1] = 0;
                Form[MaxNumber + 1] = 0;
            }

            if(SuitFilter.Contains(true))
            {
                // Find specified
                for (int i = 0; i < SuitFilter.Length; i++) Form[11 + i] = (SuitFilter[i]) ? 1 : 0;
            }
            else
            {
                // Find all
                for (int i = 0; i < SuitFilter.Length; i++) Form[11 + i] = 1;
            }

            return Form;
        }

        private static int[] ParseTilesList(string Value)
        {
            int[] TilesList = new int[38];
            for (int i = 0; i < TilesList.Length; i++) TilesList[i] = 0;

            string[] delimiter = new string[] { "," };
            string[] Temp;

            if (Value == null) return null;
            Temp = Value.Split(delimiter, StringSplitOptions.None);

            foreach (string TileName in Temp)
            {
                string CropTileName = TileName.Trim();

                if (CropTileName.Length != 2) continue;

                char TileValue = CropTileName[0];
                char TileType = CropTileName[1];

                // Check format
                if ((TileValue < '1') || (TileValue > '9')) continue;
                if ((TileType != 'm') || (TileType != 'p') || (TileType != 's') || (TileType != 'z')) continue;

                int TileIntValue = TileValue - '0';

                switch (TileType)
                {
                    case 'm':
                        TileIntValue += 0;
                        break;
                    case 'p':
                        TileIntValue += 10;
                        break;
                    case 's':
                        TileIntValue += 20;
                        break;
                    case 'z':
                        TileIntValue += 30;
                        break;
                }

                // ignore 8z, 9z
                if (TileIntValue >= TilesList.Length) continue;

                TilesList[TileIntValue]++;
            }

            return TilesList;
        }

        private static int ParseBoolArg(string Value, string ArgName)
        {
            int Temp = 1;

            if (Value == "") return 1;

            try
            {
                Temp = Convert.ToInt32(Value);
            }
            catch (Exception)
            {
                Console.Write(String.Format("Error: incorrect argument for '{s:0}' query (must be number, 0 or 1, or not specified): '{s:3}'",
                    ArgName, 0, 1, Value));
                return 1;
            }

            if ((Temp < 0) || (Temp > 1))
            {
                Console.Write(String.Format("Error: incorrect argument for '{s:0}' query (must be number, 0 or 1, or not specified): '{s:1}'",
                    ArgName, Value));
                return 1;
            }

            return Temp;
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
                Console.Write(String.Format("Error: incorrect argument for '{s:0}' query (must be number, {d:1}-{d:2}): '{s:3}'",
                    ArgName, Min, Max, Value));
                return -1;
            }
            if ((Temp < Min) || (Temp > Max))
            {
                Console.Write(String.Format("Error: incorrect argument for '{s:0}' query (must be number, {d:1}-{d:2}): '{s:3}'",
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

        static private string[] DecompositeStringList(string Text)
        {
            string[] delimiter = new string[] { "," };

            return Text.Split(delimiter, StringSplitOptions.None);
        }

        static int GetFirstNotNullIndex(bool[] BoolArray)
        {
            for (int i = 0; i < BoolArray.Length; i++)
            {
                if (BoolArray[i]) return i;
            }

            return -1;
        }
    }
}
