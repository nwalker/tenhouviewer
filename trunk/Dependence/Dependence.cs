using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Dependence
{
    class Dependence
    {
        private List<Search.Result> Results = null;

        public Dependence(List<Search.Result> Results)
        {
            this.Results = Results;
        }
    }
}
