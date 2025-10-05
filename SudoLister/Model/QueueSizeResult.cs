using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoLister.Model
{
    public sealed class QueueSizeResult
    {
        public int Pending { get; init; }
        public List<FileEntry> Items { get; init; } = new();
    }
}
