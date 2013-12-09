using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class XmlLoad
    {
        private XmlReader F;

        public XmlLoad()
        {
 
        }

        public XmlLoad(XmlReader F)
        {
            this.F = F;
        }

        public bool Load(string FileName)
        {
            if (!File.Exists(FileName)) return false;
            if ((new FileInfo(FileName).Length) == 0)
            {
                File.Delete(FileName);
                return false;
            }

            F = XmlReader.Create(FileName);

            try
            {
                F.MoveToContent();
            }
            catch (Exception)
            {
                F.Close();

                File.Delete(FileName);
                return false;
            }

            return true;
        }

        public XmlLoad GetSubtree()
        {
            XmlReader R = F.ReadSubtree();

            // Read extern element
            R.Read();

            return new XmlLoad(R);
        }

        public void Close()
        {
            // F.Skip();
        }

        public bool Read()
        {
            while (true)
            {
                if (!F.Read()) return false;

                if (F.NodeType == XmlNodeType.Element) return true;
            }
        }

        public string ElementName
        {
            get { return F.Name;  }
        }

        public string GetAttribute(string Name)
        {
            return F.GetAttribute(Name);
        }

        public int GetIntAttribute(string Name)
        {
            return Convert.ToInt32(F.GetAttribute(Name));
        }

        public int[] ReadIntArray()
        {
            int[] Array = new int[4];
            Array[0] = Convert.ToInt32(F.GetAttribute("A"));
            Array[1] = Convert.ToInt32(F.GetAttribute("B"));
            Array[2] = Convert.ToInt32(F.GetAttribute("C"));
            if(F.GetAttribute("D") != null) Array[3] = Convert.ToInt32(F.GetAttribute("D"));
            return Array;
        }

        public string[] ReadStringArray()
        {
            string[] Array = new string[4];
            Array[0] = F.GetAttribute("A");
            Array[1] = F.GetAttribute("B");
            Array[2] = F.GetAttribute("C");
            if (F.GetAttribute("D") != null) Array[3] = F.GetAttribute("D");
            return Array;
        }

        public bool[] ReadBoolArray()
        {
            bool[] Array = new bool[4];
            Array[0] = (Convert.ToUInt32(F.GetAttribute("A")) != 0);
            Array[1] = (Convert.ToUInt32(F.GetAttribute("B")) != 0);
            Array[2] = (Convert.ToUInt32(F.GetAttribute("C")) != 0);
            if (F.GetAttribute("D") != null) Array[3] = (Convert.ToUInt32(F.GetAttribute("D")) != 0);
            return Array;
        }
    }
}
