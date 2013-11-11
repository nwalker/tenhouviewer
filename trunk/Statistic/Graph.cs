using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TenhouViewer.Statistic
{
    class Graph
    {
        private List<Search.Result> GameList;

        public string NickName;

        public List<string> Fields = new List<string>();

        public Graph(string NickName, List<Search.Result> Games)
        {
            GameList = Games;

            this.NickName = NickName;
        }

        private int GetPlayerIndex(Mahjong.Replay R, string NickName)
        {
            for (int i = 0; i < 4; i++)
            {
                if (R.Players[i].NickName.CompareTo(NickName) == 0) return i;
            }

            return -1;
        }

        public List<string> GamesGraph()
        {
            List<string> Result = new List<string>();
            int GameIndex = 0;

            // Add fields name
            {
                string FieldNames = "";
                for (int i = 0; i < Fields.Count; i++) FieldNames += String.Format("{0:s}\t", Fields[i]);

                Result.Add(FieldNames);
            }

            for (int i = 0; i < GameList.Count; i++)
            {
                string Temp = "";
                Search.Result R = GameList[i];

                int Index = GetPlayerIndex(R.Replay, NickName);
                if (Index < 0) continue;

                // Compose data
                for (int j = 0; j < Fields.Count; j++)
                {
                    switch (Fields[j])
                    {
                        case "index": Temp += String.Format("{0:d}\t", GameIndex); break;
                        case "lobby": Temp += String.Format("{0:d}\t", R.Replay.Lobby); break;
                        case "rating": Temp += String.Format("{0:d}\t", R.Replay.Players[Index].Rating); break;
                        case "rank": Temp += String.Format("{0:d}\t", R.Replay.Players[Index].Rank); break;
                        case "place": Temp += String.Format("{0:d}\t", R.Replay.Place[Index]); break;
                        case "result": Temp += String.Format("{0:d}\t", R.Replay.Result[Index]); break;
                        case "balance": Temp += String.Format("{0:d}\t", R.Replay.Balance[Index]); break;
                        case "players": Temp += String.Format("{0:d}\t", R.Replay.PlayerCount); break;
                        case "datetime": Temp += String.Format("{0:s}\t", R.Replay.Date.ToString()); break;
                        case "type": Temp += String.Format("{0:s}\t", Tenhou.LobbyType.GetText(R.Replay.LobbyType)); break;
                        default: Temp += "\t"; break;
                    }
                }

                GameIndex++;
                Result.Add(Temp);
            }

            return Result;
        }

        public List<string> RoundGraph()
        {
            List<string> Result = new List<string>();

            int RoundIndex = 0;

            // Add fields name
            {
                string FieldNames = "";
                for (int i = 0; i < Fields.Count; i++) FieldNames += String.Format("{0:s}\t", Fields[i]);

                Result.Add(FieldNames);
            }
            for (int i = 0; i < GameList.Count; i++)
            {
                Search.Result R = GameList[i];

                int Index = GetPlayerIndex(R.Replay, NickName);
                if (Index < 0) continue;

                for (int r = 0; r < R.Replay.Rounds.Count; r++)
                {
                    string Temp = "";

                    if (!R.RoundMark[r]) continue;
                    Mahjong.Round Rnd = R.Replay.Rounds[r];

                    // Compose data
                    for (int j = 0; j < Fields.Count; j++)
                    {
                        switch (Fields[j])
                        {
                            case "index": Temp += String.Format("{0:d}\t", RoundIndex); break;
                            case "initshanten": Temp += String.Format("{0:d}\t", Rnd.Shanten[Index][0]); break;
                            case "pay": Temp += String.Format("{0:d}\t", Rnd.Pay[Index]); break;
                            case "tempai": Temp += String.Format("{0:d}\t", Rnd.Tempai[Index] ? 1 : 0); break;
                            case "dealer": Temp += String.Format("{0:d}\t", Rnd.Dealer[Index] ? 1 : 0); break;
                            case "winner": Temp += String.Format("{0:d}\t", Rnd.Winner[Index] ? 1 : 0); break;
                            case "loser": Temp += String.Format("{0:d}\t", Rnd.Loser[Index] ? 1 : 0); break;
                            case "riichi": Temp += String.Format("{0:d}\t", (Rnd.Riichi[Index] >= 0) ? 1 : 0); break;
                            case "concealed": Temp += String.Format("{0:d}\t", (Rnd.OpenedSets[Index] == 0) ? 1 : 0); break;
                            case "openedsets": Temp += String.Format("{0:d}\t", Rnd.OpenedSets[Index]); break;
                            case "cost": Temp += String.Format("{0:d}\t", Rnd.Cost[Index]); break;
                            case "fu": Temp += String.Format("{0:d}\t", Rnd.FuCount[Index]); break;
                            case "han": Temp += String.Format("{0:d}\t", Rnd.HanCount[Index]); break;
                            case "step": Temp += String.Format("{0:d}\t", Rnd.StepCount[Index]); break;
                            case "balance": Temp += String.Format("{0:d}\t", Rnd.BalanceBefore[Index]); break;
                            case "waiting": Temp += String.Format("{0:d}\t", Rnd.WinWaiting[Index].Count); break;
                            case "round": Temp += String.Format("{0:d}\t", Rnd.CurrentRound); break;
                            case "players": Temp += String.Format("{0:d}\t", Rnd.PlayerCount); break;
                            case "lobby": Temp += String.Format("{0:d}\t", Rnd.Lobby); break;
                            case "type": Temp += String.Format("{0:s}\t", Tenhou.LobbyType.GetText(Rnd.LobbyType)); break;
                            default: Temp += "\t"; break;
                        }
                    }

                    Result.Add(Temp);
                    RoundIndex++;
                }
            }

            return Result;
        }
    }
}
