using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biz.Morsink.DataConvert;

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
            public Entry(Type type, Type[] allTypes, string path)
                : this(type, allTypes, Path.Parse(path, type))
            { }
            public Entry(Type type, Type[] allTypes, Path path)
            {
                Type = type;
                AllTypes = allTypes;
                Path = path;
            }
            public Type Type { get; }
            public Type[] AllTypes { get; }
            public Path Path { get; }
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
                        return Identity.Create<T>(parent, entry.AllTypes, converter.Convert(value).To<string[]>());
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
        private PathMatchTree getMatchTree()
            => new PathMatchTree(entries.Select(e => e.Value.Path));

        /// <summary>
        /// Constructor.
        /// </summary>
        public PathIdentityProvider()
        {
            entries = new Dictionary<Type, Entry>();
            matchTree = new Lazy<PathMatchTree>(getMatchTree);
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
            var type = types.Last();
            var path = Path.Parse(pathstring, type);
            if (path.Arity != types.Length)
                throw new ArgumentException("Number of wildcards does not match arity of identity value.");
            entries[type] = new Entry(type, types, path);
            matchTree = new Lazy<PathMatchTree>(getMatchTree);
        }

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
                    return new Identity<object, string>(this, entry.Path.FillWildcards(new[] { converter.Convert(id.Value).To<string>() }).PathString);
                else
                    return new Identity<object, string>(this, entry.Path.FillWildcards(converter.Convert(id.Value).To<string[]>()).PathString);
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
            => ToGeneralIdentity(id).Value.ToString();

    }
}
