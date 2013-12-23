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
                if (Buffer.Count == 0) return 0;

                float Min = Buffer[0];
                foreach (float T in Buffer)
                {
                    if (T < Min) Min = T;
                }

                return Min;
            }
        }

        public float Maximum
        {
            get
            {
                if (Buffer.Count == 0) return 0;

                float Max = Buffer[0];
                foreach (float T in Buffer)
                {
                    if (T > Max) Max = T;
                }

                return Max;
            }
        }

        public float Average
        {
            get
            {
                float Avg = 0.0f;
                foreach (float T in Buffer)
                {
                    Avg += T;
                }

                return (Buffer.Count > 0) ? (Avg / Buffer.Count) : 0;
            }
        }
    }
}
