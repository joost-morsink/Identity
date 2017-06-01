using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    /// <summary>
    /// Represents a match on a path.
    /// </summary>
    public struct Match
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The matched path.</param>
        /// <param name="wildcardParts">The wildcard matches. null on unsuccesful match.</param>
        public Match(Path path, IReadOnlyList<string> wildcardParts)
        {
            Path = path;
            Parts = wildcardParts;
        }
        /// <summary>
        /// Indicates if the match is successful.
        /// </summary>
        public bool IsSuccessful => Parts != null;
        /// <summary>
        /// The path that was matched.
        /// </summary>
        public Path Path { get; }
        /// <summary>
        /// The content of the wildcard matches.
        /// </summary>
        public IReadOnlyList<string> Parts { get; }
    }
}
