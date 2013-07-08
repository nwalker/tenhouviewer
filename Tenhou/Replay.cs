using System;
using System.Collections.Generic;
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
            XmlReader reader = XmlReader.Create(Filename);

            Parse(reader);
        }

        // .mjlog
        public void OpenGZ(string Filename)
        {

        }

        private void Parse(XmlReader Reader)
        {

        }
    }
}
