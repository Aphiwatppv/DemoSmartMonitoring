using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoLister.Model
{
    public sealed class FileEntry
    {
        public string Name { get; init; } = "";
        public string Path { get; init; } = "";
        public bool IsDir { get; init; }
        public long Size { get; init; }
        public DateTime? MTime { get; init; }
        public string? Mode { get; init; }
    }
}
