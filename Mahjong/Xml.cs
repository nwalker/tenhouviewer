using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class Xml
    {
        private XmlWriterSettings settings;
        private XmlWriter F;

        public Xml(string FileName)
        {
            settings = new XmlWriterSettings();

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
        }

        public void Close()
        {
            F.Close();
        }

        public void StartXML(string Element)
        {
            StartTag(Element);
            Attribute("date", DateTime.Now.ToString());
            Attribute("v", "1");
        }

        public void EndXML()
        {
            EndTag();
        }

        // Тег с одним аргументом
        public void WriteTag(string Tag, string Attribute, string Value)
        {
            StartTag(Tag);
            this.Attribute(Attribute, Value);
            EndTag();
        }

        // Тег с одним аргументом
        public void WriteTag(string Tag, string Attribute, int Value)
        {
            WriteTag(Tag, Attribute, Value.ToString());
        }

        // Запись тега с 4мя числовыми аттрибутами
        public void WriteTag(string Tag, int[] Values)
        {
            StartTag(Tag);
            Attribute("A", Values[0]);
            Attribute("B", Values[1]);
            Attribute("C", Values[2]);
            Attribute("D", Values[3]);

            EndTag();
        }

        public void StartTag(string Tag)
        {
            F.WriteStartElement(Tag);
        }

        public void EndTag()
        {
            F.WriteEndElement();
        }

        public void Attribute(string Name, int Value)
        {
            F.WriteAttributeString(Name, Value.ToString());
        }

        public void Attribute(string Name, string Value)
        {
            F.WriteAttributeString(Name, Value);
        }
    }
}
