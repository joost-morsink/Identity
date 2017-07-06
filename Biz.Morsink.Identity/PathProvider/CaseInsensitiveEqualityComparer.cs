using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
    {
        public static CaseInsensitiveEqualityComparer Instance { get; } = new CaseInsensitiveEqualityComparer();
        public bool Equals(string x, string y)
            => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(string obj)
            => obj.ToUpperInvariant().GetHashCode();
    }
}
