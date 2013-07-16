using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class Round
    {
        public string Hash = "";
        public int Index = 0;

        public Wall Wall = new Wall();
        public List<Step> Steps = new List<Step>();
        public Hand[] Hands = new Hand[4]; // Start hands

        private XmlWriter F;

        public Round()
        {
            
        }

        public void Save(string FileName)
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

            StartXML("mjround");

            // Что это за раздача?
            WriteTag("hash", "value", Hash);
            WriteTag("game", "index", Index.ToString());

            // Действия
            {
                F.WriteStartElement("steps");
                F.WriteAttributeString("count", Steps.Count.ToString());

                for (int j = 0; j < Steps.Count; j++)
                {
                    Steps[j].WriteXml(F);
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
