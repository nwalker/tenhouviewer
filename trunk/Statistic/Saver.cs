using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TenhouViewer.Statistic
{
    class Saver
    {
        public Saver(string FileName, List<string> Data)
        {
            if (FileName == null) return;
            if (Data == null) return;

            string FilePath = Path.GetDirectoryName(Path.GetFullPath(FileName));
            
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
            if (File.Exists(FileName)) File.Delete(FileName);

            var fileStream = File.Open(FileName, FileMode.CreateNew);
            if (fileStream == null) return;

            StreamWriter  Output = new StreamWriter(fileStream);

            foreach (string Line in Data)
                Output.WriteLine(Line);

            Output.Close();
        }
    }
}
