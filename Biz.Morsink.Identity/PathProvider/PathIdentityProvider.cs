using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biz.Morsink.DataConvert;
using Biz.Morsink.Identity.Utils;
using System.Collections.Immutable;

namespace Biz.Morsink.Identity.PathProvider
{
    /// <summary>
    /// Identity Provider that map identity values on Paths and back.
    /// </summary>
    public class PathIdentityProvider : AbstractIdentityProvider
    {
        #region Helper classes
        private class Entry
        {
            public Entry(Type type, Type[] allTypes, params string[] paths)
                : this(type, allTypes, paths.Select(path => Path.Parse(path, type)))
            { }
            public Entry(Type type, Type[] allTypes, IEnumerable<Path> paths)
            {
                Type = type;
                AllTypes = allTypes;
                Paths = paths.ToArray();
            }
            public Type Type { get; }
            public Type[] AllTypes { get; }
            public IReadOnlyList<Path> Paths { get; }
            public Path PrimaryPath => Paths[0];
        }
        /// <summary>
        /// A helper struct to facilitate building entries in a PathIdentityProvider
        /// </summary>
        protected struct EntryBuilder
        {
            /// <summary>
            /// Creates a new EntryBuilder.
            /// </summary>
            /// <param name="parent">The ParentIdentityProvider the entry will be added to.</param>
            /// <param name="allTypes">The entity types of the identity value's components.</param>
            /// <returns>An EntryBuilder.</returns>
            internal static EntryBuilder Create(PathIdentityProvider parent, params Type[] allTypes)
                => new EntryBuilder(parent, allTypes, ImmutableList<(Path, Type[])>.Empty);
            private readonly PathIdentityProvider parent;
            private readonly Type[] allTypes;
            private readonly ImmutableList<(Path, Type[])> paths;

            private EntryBuilder(PathIdentityProvider parent, Type[] allTypes, ImmutableList<(Path, Type[])> paths)
            {
                this.parent = parent;
                this.allTypes = allTypes;
                this.paths = paths;
            }
            /// <summary>
            /// Adds a path to the entry.
            /// </summary>
            /// <param name="path">The path to add to the entry.</param>
            /// <returns>A new EntryBuilder containing the specified path.</returns>
            public EntryBuilder WithPath(string path)
                => WithPath(path, allTypes);
            /// <summary>
            /// Adds a path to the entry, with possibly a capped arity.
            /// </summary>
            /// <param name="path">The path to add to the entry.</param>
            /// <param name="arity">The arity of wildcards in the path.</param>
            /// <returns>A new EntryBuilder containing the specified path.</returns>
            public EntryBuilder WithPath(string path, int arity)
                => WithPath(path, allTypes.Skip(allTypes.Length - arity).ToArray());
            /// <summary>
            /// Adds multiple paths to the entry.
            /// </summary>
            /// <param name="paths">The paths to add to the entry.</param>
            /// <returns>A new EntryBuilder containing the specified paths.</returns>
            public EntryBuilder WithPaths(params string[] paths)
            {
                var res = this;
                foreach (var path in paths)
                    res = res.WithPath(path);
                return res;
            }
            /// <summary>
            /// Adds a path to the entry with a specific list of entity types.
            /// </summary>
            /// <param name="path">The path to add to the entry.</param>
            /// <param name="types">The entity types of the identity value's components.</param>
            /// <returns>A new EntryBuilder containing the specified path.</returns>
            public EntryBuilder WithPath(string path, params Type[] types)
            {
                var p = Path.Parse(path, allTypes[allTypes.Length - 1]);
                if (p.Arity != types.Length && (p.Arity > 0 || types.Length != 1))
                    throw new ArgumentException("Number of wildcards does not match arity of identity value.");
                return new EntryBuilder(parent, allTypes, paths.Add((p, types)));
            }
            /// <summary>
            /// Adds the entry to the parent PathIdentityProvider this builder was created from.
            /// </summary>
            public void Add()
            {
                if (!paths.IsEmpty)
                {
                    parent.entries.Add(allTypes[allTypes.Length - 1], new Entry(allTypes[allTypes.Length - 1], allTypes, paths.Select(t => t.Item1)));
                    parent.matchTree = new Lazy<PathMatchTree>(parent.GetMatchTree);
                }
            }

        }
        private class CreatorForObject : IIdentityCreator<object>
        {
            private readonly PathIdentityProvider parent;
            private readonly IDataConverter converter;

            public CreatorForObject(PathIdentityProvider parent)
            {
                this.parent = parent;
                converter = parent.GetConverter(typeof(object), true);
            }

            public IIdentity<object> Create<K>(K value)
            {
                if (converter.Convert(value).TryTo(out string res))
                    return new Identity<object, string>(parent, res);
                else
                    return null;
            }

            IIdentity IIdentityCreator.Create<K>(K value)
                => Create(value);
        }
        private class Creator<T, U> : IIdentityCreator<T>
        {
            private readonly PathIdentityProvider parent;
            private readonly IDataConverter converter;
            private readonly Entry entry;

            public Creator(PathIdentityProvider parent, Entry entry)
            {
                this.parent = parent;
                this.entry = entry;
                converter = parent.GetConverter(typeof(T), true);
            }

            public IIdentity<T> Create<K>(K value)
            {
                var res = converter.DoConversion<K, U>(value);
                if (res.IsSuccessful)
                {
                    if (this.entry.AllTypes.Length == 1)
                        return new Identity<T, U>(parent, res.Result);
                    else
                    {
                        if (!converter.Convert(res.Result).TryTo(out string[] compValues) || compValues.Length != entry.AllTypes.Length)
                            throw new ArgumentException("The number of component values does not match the arity of the identity value.");
                        var bld = parent.BuildGeneralIdentity()
                            .AddRange(entry.AllTypes.Zip(compValues, (t, cv) => (t, cv.GetType(), (object)cv)));
                        return (IIdentity<T>)bld.Id();
                    }
                }
                else
                    return null;
            }

            IIdentity IIdentityCreator.Create<K>(K value)
                => Create(value);
        }
        #endregion

        private Dictionary<Type, Entry> entries;
        private Lazy<PathMatchTree> matchTree;
        private PathMatchTree GetMatchTree()
            => new PathMatchTree(entries.SelectMany(e => e.Value.Paths));

        /// <summary>
        /// Constructor.
        /// </summary>
        public PathIdentityProvider()
        {
            entries = new Dictionary<Type, Entry>();
            matchTree = new Lazy<PathMatchTree>(GetMatchTree);
        }
        /// <summary>
        /// Add an entity type entry into this providers registry.
        /// </summary>
        /// <param name="pathstring">The path containing wildcards for the identity value's underlying value.</param>
        /// <param name="types">The entity type hierarchy.</param>
        protected void AddEntry(string pathstring, params Type[] types)
        {
            if (types.Length == 0)
                throw new ArgumentException("Please specify at least one type");
            BuildEntry(types).WithPath(pathstring).Add();
        }
        /// <summary>
        /// Creates an entry builder
        /// </summary>
        /// <param name="types">The entity types for the identity value's components</param>
        /// <returns>An EntryBuilder</returns>
        protected EntryBuilder BuildEntry(params Type[] types)
            => EntryBuilder.Create(this, types);

        private Type GetUnderlyingType(int arity)
        {
            switch (arity)
            {
                case 1:
                    return typeof(string);
                case 2:
                    return typeof((string, string));
                case 3:
                    return typeof((string, string, string));
                case 4:
                    return typeof((string, string, string, string));
                case 5:
                    return typeof((string, string, string, string, string));
                default:
                    return null;
            }
        }
        /// <summary>
        /// Gets the underlying type for some entity type.
        /// This provider only uses strings and ValueTuples of strings.
        /// </summary>
        /// <param name="forType">The entity type.</param>
        /// <returns>An underlying value type for the specified entity type.</returns>
        public override Type GetUnderlyingType(Type forType)
        {
            return entries.TryGetValue(forType, out var e) ? GetUnderlyingType(e.AllTypes.Length) : null;
        }
        /// <summary>
        /// Gets an IIdentityCreator&lt;T&gt; instance for some type.
        /// </summary>
        /// <param name="type">The type to get an IIdentityCreator for.</param>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected override IIdentityCreator GetCreator(Type type)
        {
            if (type == typeof(object))
            {
                return new CreatorForObject(this);
            }
            else if (entries.TryGetValue(type, out var ent))
                return (IIdentityCreator)Activator.CreateInstance(typeof(Creator<,>).MakeGenericType(type, GetUnderlyingType(type)), this, ent);
            else
                return null;
        }
        /// <summary>
        /// Gets an IIdentityCreator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">The type to get an IIdentityCreator for.</typeparam>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected override IIdentityCreator<T> GetCreator<T>()
            => GetCreator(typeof(T)) as IIdentityCreator<T>;

        /// <summary>
        /// Parses a path string into an IIdentity value.
        /// If a match is found, the IIdentity value is properly typed.
        /// </summary>
        /// <param name="path">The path to parse.</param>
        /// <param name="nullOnFailure">When there is not match found for the Path, this boolean indicates whether to return a null or an IIdentity&lt;object&gt;.</param>
        /// <returns>An IIdentity value for the path.</returns>
        public IIdentity Parse(string path, bool nullOnFailure = false)
        {
            var match = matchTree.Value.Walk(Path.Parse(path));
            if (match.IsSuccessful)
            {
                if (match.Parts.Count == 1)
                    return Create(match.Path.ForType, match.Parts[0]);
                else if (match.Parts.Count == 0)
                    return Create(match.Path.ForType, "");
                else
                    return Create(match.Path.ForType, match.Parts.ToArray());
            }
            else
                return nullOnFailure ? null : new Identity<object, string>(this, path);
        }
        /// <summary>
        /// Parses a path string into an IIdentity&lt;T&gt; value.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="path">The path to parse.</param>
        /// <returns>An IIdentity&lt;T&gt; value if the parse and match were successful.</returns>
        public IIdentity<T> Parse<T>(string path)
            => Parse(path, true) as IIdentity<T>;
        /// <summary>
        /// Tries to translate a general IIdentity&lt;object&gt; into a more specific type.
        /// </summary>
        /// <param name="objectId">The input identity value.</param>
        /// <param name="nullOnFailure">If no match is found, this boolean indicates whether to return a null or the original input identity value.</param>
        /// <returns>An identity value.</returns>
        public IIdentity Parse(IIdentity<object> objectId, bool nullOnFailure)
            => Parse(Translate(objectId).Value.ToString(), nullOnFailure);
        /// <summary>
        /// Tries to translate a general IIdentity&lt;object&gt; into a more specific type.
        /// </summary>
        /// <typeparam name="T">The entity type to parse the value for.</typeparam>
        /// <param name="objectId">The input identity value.</param>
        /// <returns>An identity value.=, null if the match is unsuccessful.</returns>
        public IIdentity<T> Parse<T>(IIdentity<object> objectId)
            => Parse(objectId, true) as IIdentity<T>;

        /// <summary>
        /// Converts any identity value for a known type into a general identity value with a pathstring as underlying value.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IIdentity<object> ToGeneralIdentity(IIdentity id)
        {
            if (id.ForType == typeof(object))
                return (IIdentity<object>)id;
            var converter = GetConverter(id.ForType, false);
            if (entries.TryGetValue(id.ForType, out var entry))
            {
                if (id.Arity == 1)
                    return new Identity<object, string>(this, entry.PrimaryPath.FillWildcards(new[] { converter.Convert(id.Value).To<string>() }).PathString);
                else
                    return new Identity<object, string>(this, entry.PrimaryPath.FillWildcards(converter.Convert(id.Value).To<string[]>()).PathString);
            }
            else
                return null;
        }
        /// <summary>
        /// Converts any identity value for a known type into a pathstring.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ToPath(IIdentity id)
            => ToGeneralIdentity(id)?.Value.ToString();

    }
}
