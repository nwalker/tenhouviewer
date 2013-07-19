using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer
{
    class ArgumentParser
    {
        private string[] arglist;

        public List<Argument> Arguments = new List<Argument>();

        public ArgumentParser(string[] args)
        {
            arglist = args;

            Parse();
        }

        public int Count
        {
            get { return arglist.Length; }
        }

        private void Parse()
        {
            Argument Temp = null;

            foreach (string a in arglist)
            {
                if ((a[0] == '-') || (a[0] == '\\'))
                {
                    Temp = new Argument();
                    Temp.Name = Convert.ToString(a[1]);
                    Temp.Value = a.Substring(2);
                    Arguments.Add(Temp);
                }
                else
                {
                    // Additional argument
                    if (Temp == null) continue;

                    string Param, Value;

                    int Delimiter = a.IndexOf('=');
                    if (Delimiter < 0)
                    {
                        Param = a;
                        Value = "";
                    }
                    else
                    {
                        Param = a.Substring(0, Delimiter);
                        Value = a.Substring(Delimiter + 1);
                    }

                    Argument SubArg = new Argument();
                    SubArg.Name = Param;
                    SubArg.Value = Value;

                    Temp.Arguments.Add(SubArg);
                }
            }
        }
    }
}
