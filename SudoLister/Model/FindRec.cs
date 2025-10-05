using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoLister.Model
{
    internal sealed class FindRec
    {
        public string t { get; set; } = ""; // %y
        public long s { get; set; }         // %s
        public string m { get; set; } = ""; // %T@
        public string p { get; set; } = ""; // %p
    }
}
