using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Tenhou
{
    class Replay
    {
        public Replay()
        {

        }

        // .xml
        public void OpenPlainText(string Filename)
        {
            XmlReader Reader = XmlReader.Create(Filename);

            Parse(Reader);
        }

        // .mjlog
        public void OpenGZ(string Filename)
        {
            FileStream File = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            GZipStream Stream = new GZipStream(File, CompressionMode.Decompress);
            XmlReader Reader = XmlReader.Create(Stream);

            Parse(Reader);
        }

        private void Parse(XmlReader Reader)
        {
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch(reader.Name)
                    {
                    case "GO":
                        // Player goes online
                        break;
                    case "UN":
                        // Player info
                        break;
                    case "BYE":
                        // Player goes offline
                        break;
                    case "SHUFFLE":
                        // Seed for generating walls
                        break;
                    case "INIT":
                        // Init game: hands
                        break;
                    case "TAIKYOKU":
                        // Current round
                        break;
                    case "RYUUKYOKU":
                        // Draw
                        break;
                    case "N":
                        // Naki - open set
                        break;
                    case "DORA":
                        // Open new dora indicator
                        break;
                    case "AGARI":
                        // Ron or Tsumo
                        break;
                    case "REACH":
                        // declare riichi! 2 steps
                        break;
                    default:
                        // Action: draw and discard tile
                        break;
                    }
                }
            }
        }
    }
}
