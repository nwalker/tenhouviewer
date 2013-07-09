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

        }
    }
}
