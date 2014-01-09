using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Dependence
{
    class Value
    {
        List<float> Buffer = new List<float>();

        public Value()
        {

        }

        public void Add(float Value)
        {
            Buffer.Add(Value); 
        }

        public float Minimum
        {
            get
            {
                return Buffer.Min();
            }
        }

        public float Maximum
        {
            get
            {
                return Buffer.Max();
            }
        }

        public float Average
        {
            get
            {
                return Buffer.Average();
            }
        }
    }
}
