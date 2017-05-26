using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biz.Morsink.DataConvert;

namespace Biz.Morsink.Identity.PathProvider
{
    public class PathIdentityProvider : AbstractIdentityProvider
    {
        #region Helper classes
        private class Entry
        {
            public Entry(Type type, Type[] fullTypes, string path)
            {
                Type = type;
                FullTypes = fullTypes;
                Path = Path.Parse(path, type);
            }
            public Type Type { get; }
            public Type[] FullTypes { get; }
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
                    if (this.entry.FullTypes.Length == 1)
                        return new Identity<T, U>(parent, res.Result);
                    else
                        return Identity.Create<T>(parent, entry.FullTypes, converter.Convert(value).To<string[]>());
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
        public PathIdentityProvider()
        {
            entries = new Dictionary<Type, Entry>();
            matchTree = new Lazy<PathMatchTree>(getMatchTree);
        }
        protected void AddEntry(Type type, Type[] fullTypes, string path)
        {
            entries[type] = new Entry(type, fullTypes, path);
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

        public override Type GetUnderlyingType(Type forType)
        {
            return entries.TryGetValue(forType, out var e) ? GetUnderlyingType(e.FullTypes.Length) : null;
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

        public IIdentity Parse(string path)
        {
            var match = matchTree.Value.Walk(Path.Parse(path));
            if (match.IsSuccess)
            {
                if (match.Parts.Count == 1)
                    return Create(match.Path.Data, match.Parts[0]);
                else
                    return Create(match.Path.Data, match.Parts.ToArray());
            }
            else
                return null;
        }
        public IIdentity<T> Parse<T>(string path)
            => Parse(path) as IIdentity<T>;
        public IIdentity Parse(IIdentity<object> objectId)
            => Parse(Translate(objectId).Value.ToString());
        public IIdentity<T> Parse<T>(IIdentity<object> objectId)
            => Parse(objectId) as IIdentity<T>;
        
        public IIdentity<object> ToGeneralIdentity(IIdentity id)
        {
            if (id.ForType == typeof(object))
                return (IIdentity<object>)id;
            var converter = GetConverter(id.ForType, false);
            if (entries.TryGetValue(id.ForType, out var entry))
            {
                if (id.Arity == 1)
                    return new Identity<object, string>(this, entry.Path.FillStars(new[] { converter.Convert(id.Value).To<string>() }).PathString);
                else
                    return new Identity<object, string>(this, entry.Path.FillStars(converter.Convert(id.Value).To<string[]>()).PathString);
            }
            else
                return null;
        }
        public string ToPath(IIdentity id)
            => ToGeneralIdentity(id).Value.ToString();

    }
}
