using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class Replay
    {
        public string Hash = "";
        public Player[] Players = new Player[4];
        public List<Round> Rounds = new List<Round>();

        public int[] Place = new int[4];

        public Replay()
        {

        }

        public void Save()
        {
            SaveXml("replay/" + Hash + ".xml");
            // Save round info in files
            for (int i = 0; i < Rounds.Count; i++)
            {
                Rounds[i].Save("round/"+ Hash + "_" + i.ToString() + ".xml");
            }
        }

        public void SaveXml(string FileName)
        {
            // (рассмотрен выше)
            Xml X = new Xml(FileName);

            X.StartXML("mjreplay");

            // Что это за раздача?
            X.WriteTag("hash", "value", Hash);

            // список раундов
            {
                X.StartTag("rounds");
                X.Attribute("count", Rounds.Count);

                for (int i = 0; i < Rounds.Count; i++)
                {
                    X.StartTag("round");
                    X.Attribute("index", i);
                    X.Attribute("filename", "round/" + Hash + "_" + i.ToString() + ".xml");

                    X.EndTag();
                }
                X.EndTag();
            }

            X.EndXML();
            X.Close();
        }
    }
}
