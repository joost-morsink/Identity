using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Biz.Morsink.Identity
{
    public static class Identity
    {
        internal static Builder BuildIdentity(this IIdentityProvider provider)
            => new Builder(provider);

        public struct Builder
        {
            private readonly IIdentityProvider _provider;

            internal Builder(IIdentityProvider provider)
            {
                _provider = provider;
            }
            public Builder<T1> For<T1>()
            {
                return new Builder<T1>(_provider);
            }
        }
        public struct Builder<T1>
        {
            private readonly IIdentityProvider _provider;
            internal Builder(IIdentityProvider provider)
            {
                _provider = provider;
            }
            public Builder<T1, K1> Value<K1>(K1 value)
                => new Builder<T1, K1>(new Identity<T1, K1>(_provider, value));
        }
        public struct Builder<T1, K1>
        {
            private readonly Identity<T1, K1> identity;

            public Builder(Identity<T1, K1> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, K1> For<T2>()
                => new Builder<T1, T2, K1>(identity);
            public Identity<T1, K1> Id()
                => identity;
            public static implicit operator Identity<T1, K1>(Builder<T1, K1> bld)
                => bld.Id();
        }

        public struct Builder<T1, T2, K1>
        {
            private readonly Identity<T1, K1> identity;

            public Builder(Identity<T1, K1> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, K1, K2> Value<K2>(K2 value)
                => new Builder<T1, T2, K1, K2>(new Identity<T1, T2, K1, K2>(identity, value));
        }

        public struct Builder<T1, T2, K1, K2>
        {
            private Identity<T1, T2, K1, K2> identity;

            public Builder(Identity<T1, T2, K1, K2> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, T3, K1, K2> For<T3>()
                => new Builder<T1, T2, T3, K1, K2>(identity);
            public Identity<T1, T2, K1, K2> Id()
                => identity;
            public static implicit operator Identity<T1, T2, K1, K2>(Builder<T1, T2, K1, K2> bld)
                => bld.Id();
        }
        public struct Builder<T1, T2, T3, K1, K2>
        {
            private readonly Identity<T1, T2, K1, K2> identity;

            public Builder(Identity<T1, T2, K1, K2> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, T3, K1, K2, K3> Value<K3>(K3 value)
                => new Builder<T1, T2, T3, K1, K2, K3>(new Identity<T1, T2, T3, K1, K2, K3>(identity, value));
        }

        public struct Builder<T1, T2, T3, K1, K2, K3>
        {
            private Identity<T1, T2, T3, K1, K2, K3> identity;

            public Builder(Identity<T1, T2, T3, K1, K2, K3> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, T3, T4, K1, K2, K3> For<T4>()
                => new Builder<T1, T2, T3, T4, K1, K2, K3>(identity);
            public Identity<T1, T2, T3, K1, K2, K3> Id()
                => identity;
            public static implicit operator Identity<T1, T2, T3, K1, K2, K3>(Builder<T1, T2, T3, K1, K2, K3> bld)
                => bld.Id();
        }
        public struct Builder<T1, T2, T3, T4, K1, K2, K3>
        {
            private readonly Identity<T1, T2, T3, K1, K2, K3> identity;

            public Builder(Identity<T1, T2, T3, K1, K2, K3> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, T3, T4, K1, K2, K3, K4> Value<K4>(K4 value)
                => new Builder<T1, T2, T3, T4, K1, K2, K3, K4>(new Identity<T1, T2, T3, T4, K1, K2, K3, K4>(identity, value));
        }

        public struct Builder<T1, T2, T3, T4, K1, K2, K3, K4>
        {
            private Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity;

            public Builder(Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4> For<T5>()
                => new Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4>(identity);
            public Identity<T1, T2, T3, T4, K1, K2, K3, K4> Id()
                => identity;
            public static implicit operator Identity<T1, T2, T3, T4, K1, K2, K3, K4>(Builder<T1, T2, T3, T4, K1, K2, K3, K4> bld)
                => bld.Id();
        }
        public struct Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4>
        {
            private readonly Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity;

            public Builder(Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity)
            {
                this.identity = identity;
            }
            public Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> Value<K5>(K5 value)
                => new Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>(new Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>(identity, value));
        }

        public struct Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>
        {
            private Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> identity;

            public Builder(Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> identity)
            {
                this.identity = identity;
            }
            public Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> Id()
                => identity;
            public static implicit operator Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>(Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> bld)
                => bld.Id();
        }

        internal static GeneralBuilder BuildGeneralIdentity(this IIdentityProvider provider)
            => new GeneralBuilder(provider);

        public struct GeneralBuilder
        {
            private static readonly Type[] ID_TYPES = new[] {
                null,
                typeof(Identity<,>),
                typeof(Identity<,,,>),
                typeof(Identity<,,,,,>),
                typeof(Identity<,,,,,,,>),
                typeof(Identity<,,,,,,,,,>)
            };

            private readonly IIdentityProvider _provider;
            private readonly ImmutableList<(Type, Type, object)> _components;
            internal GeneralBuilder(IIdentityProvider provider, ImmutableList<(Type, Type, object)> components = null)
            {
                _provider = provider;
                _components = components ?? ImmutableList<(Type, Type, object)>.Empty;
            }
            public GeneralBuilder Add(Type t, Type v, object value)
                => new GeneralBuilder(_provider, _components.Add((t, v, value)));
            public GeneralBuilder AddRange(IEnumerable<(Type, Type, object)> items)
                => new GeneralBuilder(_provider, _components.AddRange(items));
            public GeneralBuilder Add(params (Type, Type, object)[] items)
                => new GeneralBuilder(_provider, _components.AddRange(items));

            public Odd<T> For<T>()
                => new Odd<T>(this);
            public Odd For(Type t)
                => new Odd(this, t);
            public struct Odd
            {
                private readonly GeneralBuilder _builder;
                private readonly Type _type;

                internal Odd(GeneralBuilder builder, Type type)
                {
                    _builder = builder;
                    _type = type;
                }
                public GeneralBuilder Value<K>(K value)
                    => _builder.Add(_type, typeof(K), value);
                public GeneralBuilder Value(Type v, object value)
                    => _builder.Add(_type, v, value);
            }
            public struct Odd<T>
            {
                private readonly GeneralBuilder _builder;

                internal Odd(GeneralBuilder builder)
                {
                    _builder = builder;
                }
                public Even<T> Value<K>(K value)
                    => new Even<T>(_builder._provider, _builder._components.Add((typeof(T), typeof(K), value)));
                public Even<T> Value(Type v, object value)
                    => new Even<T>(_builder._provider, _builder._components.Add((typeof(T), v, value)));
            }
            public struct Even<T>
            {
                private readonly GeneralBuilder _builder;
                internal Even(IIdentityProvider provider, ImmutableList<(Type, Type, object)> components = null)
                {
                    _builder = new GeneralBuilder(provider, components);

                }
                public GeneralBuilder Add(Type t, Type v, object value)
                    => _builder.Add(t, v, value);
                public GeneralBuilder AddRange(IEnumerable<(Type, Type, object)> items)
                    => _builder.AddRange(items);
                public GeneralBuilder Add(params (Type, Type, object)[] items)
                    => _builder.Add(items);
                public GeneralBuilder Builder()
                    => _builder;
                public Odd<U> For<U>()
                    => new Odd<U>(_builder);
                public Odd For(Type t)
                    => new Odd(_builder, t);
                public IIdentity<T> Id()
                    => (IIdentity<T>)_builder.Id();
            }
            public IIdentity Id()
            {
                var n = _components.Count;
                var t = ID_TYPES[n];
                var types = new Type[n * 2];
                var parameters = new object[n + 1];
                parameters[0] = _provider;
                for (int i = 0; i < n; i++)
                {
                    types[i] = _components[i].Item1;
                    types[n + i] = _components[i].Item2;
                    parameters[i + 1] = _components[i].Item3;
                }
                return (IIdentity)Activator.CreateInstance(t.MakeGenericType(types), parameters);
            }
        }
    }
}
