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

        private XmlWriter F;

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
            XmlWriterSettings settings = new XmlWriterSettings();

            {
                // включаем отступ для элементов XML документа
                // (позволяет наглядно изобразить иерархию XML документа)
                settings.Indent = true;
                settings.IndentChars = "    "; // задаем отступ, здесь у меня 4 пробела

                // задаем переход на новую строку
                settings.NewLineChars = "\r\n";

                // Нужно ли опустить строку декларации формата XML документа
                // речь идет о строке вида "<?xml version="1.0" encoding="utf-8"?>"
                settings.OmitXmlDeclaration = true;
            }

            // (рассмотрен выше)
            F = XmlWriter.Create(FileName, settings);

            StartXML("mjreplay");

            // Что это за раздача?
            WriteTag("hash", "value", Hash);

            // список раундов
            {
                F.WriteStartElement("rounds");
                F.WriteAttributeString("count", Rounds.Count.ToString());
                for (int i = 0; i < Rounds.Count; i++)
                {
                    F.WriteStartElement("round");
                    F.WriteAttributeString("index", i.ToString());
                    F.WriteAttributeString("filename", "round/" + Hash + "_" + i.ToString() + ".xml");

                    F.WriteEndElement();
                }
                F.WriteEndElement();
            }

            EndXML();
            F.Close();
        }

        private void StartXML(string Element)
        {
            F.WriteStartElement(Element);
            F.WriteAttributeString("date", DateTime.Now.ToString());
            F.WriteAttributeString("v", "1");
        }

        private void EndXML()
        {
            F.WriteEndElement();
        }

        // Тег с одним аргументом
        private void WriteTag(string Tag, string Attribute, string Value)
        {
            F.WriteStartElement(Tag);
            F.WriteAttributeString(Attribute, Value);
            F.WriteEndElement();
        }
    }
}
