using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Dependence
{
    enum XType
    {
        Rating,
        Rank
    }

    enum YType
    {

    }

    class Dependence
    {
        private List<Search.Result> Results = null;
        private List<Value> Values = new List<Value>();

        public Dependence(List<Search.Result> Results)
        {
            this.Results = Results;
        }
    }
}
