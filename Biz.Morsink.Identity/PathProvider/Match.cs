using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    public struct Match
    {
        public Match(Path path, IReadOnlyList<string> parts)
        {
            Path = path;
            Parts = parts;
        }
        public bool IsSuccess => Parts != null;
        public Path Path { get; }
        public IReadOnlyList<string> Parts { get; }
    }
}
