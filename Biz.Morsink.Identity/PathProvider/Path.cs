﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    /// <summary>
    /// This class represents a path.
    /// It may contain wildcard parts "*".
    /// </summary>
    public struct Path
    {
        /// <summary>
        /// Parses a string into a Path instance.
        /// </summary>
        /// <param name="pathString">The path string to parse.</param>
        /// <param name="forType">The entity type the path belongs to.</param>
        /// <param name="separator">An optional separator character, default '/'.</param>
        /// <returns>A parsed Path instance.</returns>
        public static Path Parse(string pathString, Type forType = null, char separator = '/')
            => new Path(pathString.Split(separator), forType);
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parts">All the parts of the path.</param>
        /// <param name="forType">The entity type the path belongs to.</param>
        public Path(IEnumerable<string> parts, Type forType)
        {
            this.parts = parts.ToArray();
            skip = 0;
            ForType = forType;
        }
        private Path(string[] parts, int skip, Type forType)
        {
            this.parts = parts;
            this.skip = skip;
            ForType = forType;
        }
        private string[] parts;
        private int skip;

        /// <summary>
        /// Gets the number of path parts in this Path.
        /// </summary>
        public int Count => parts.Length - skip;
        /// <summary>
        /// Gets the number of wildcard parts in this Path.
        /// </summary>
        public int Arity => parts.Where(p => p == "*").Count();
        /// <summary>
        /// Gets the entity type this Path is for.
        /// </summary>
        public Type ForType { get; }
        /// <summary>
        /// Gets a specific element of this Path.
        /// </summary>
        /// <param name="index">The index of the part.</param>
        /// <returns>A specific element of the Path.</returns>
        public string this[int index] => parts[skip + index];
        /// <summary>
        /// Constructs a new Path, based on skipping some of the first elements.
        /// </summary>
        /// <param name="num">The number of parts to skip.</param>
        /// <returns>A shorter Path.</returns>
        public Path Skip(int num = 1)
            => new Path(parts, skip + num, ForType);
        /// <summary>
        /// Tries to match another Path to this one.
        /// </summary>
        /// <param name="other">The Path to match.</param>
        /// <returns>A Match instance containing the match results.</returns>
        public Match Match(Path other)
        {
            if (Count != other.Count)
                return default(Match);
            var result = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == "*")
                    result.Add(other[i]);
                else if (this[i] != other[i])
                    return default(Match);
            }
            return new Match(this, result.ToArray());
        }
        /// <summary>
        /// Gets the full path this path was constructed from.
        /// </summary>
        /// <returns>A Path.</returns>
        public Path GetFullPath()
            => new Path(parts, 0, ForType);
        private IEnumerable<string> fillHelper(IEnumerable<string> stars)
        {
            var s = stars.ToArray();
            int n = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "*")
                    yield return s[n++];
                else
                    yield return parts[i];
            }
        }
        /// <summary>
        /// Constructs a new Path by assigning values to the wildcards in the Path.
        /// </summary>
        /// <param name="wildcards">The values for the wildcards</param>
        /// <returns>A new Path</returns>
        public Path FillWildcards(IEnumerable<string> wildcards)
            => new Path(fillHelper(wildcards), ForType);
        /// <summary>
        /// Gets a string representation for the Path.
        /// </summary>
        public string PathString => string.Join("/", parts);
    }
}
