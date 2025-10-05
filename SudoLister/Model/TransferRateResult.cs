using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoLister.Model
{
    public sealed class TransferRateResult
    {
        public int Files { get; init; }
        public TimeSpan Window { get; init; }
        public DateTime WindowStart { get; init; }
        public DateTime WindowEnd { get; init; }
        public double PerMinute { get; init; }
        public double PerHour { get; init; }
    }
}
