using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Helper class for building typed identity values with a fluent interface.
    /// It is meant for use in Identity Provider implementations.
    /// </summary>
    public static class Identity
    {
        internal static Builder BuildIdentity(this IIdentityProvider provider)
            => new Builder(provider);

        /// <summary>
        /// This Builder represents an IIdentityProvider reference.
        /// </summary>
        public struct Builder
        {
            private readonly IIdentityProvider _provider;

            internal Builder(IIdentityProvider provider)
            {
                _provider = provider;
            }
            /// <summary>
            /// Sets the next entity type for the next entity in the identity sequence.
            /// </summary>
            /// <typeparam name="T1">The next entity type.</typeparam>
            /// <returns>A Builder for the new context</returns>
            public Builder<T1> For<T1>()
            {
                return new Builder<T1>(_provider);
            }
        }
        /// <summary>
        /// This Builder represents an IIdentityProvider reference and a single entity type.
        /// </summary>
        /// <typeparam name="T1">The entity type.</typeparam>
        public struct Builder<T1>
        {
            private readonly IIdentityProvider _provider;
            internal Builder(IIdentityProvider provider)
            {
                _provider = provider;
            }
            /// <summary>
            /// Sets the underlying value for the first type.
            /// </summary>
            /// <typeparam name="K1">The type of the underlying value.</typeparam>
            /// <param name="value">The underlying value.</param>
            /// <returns>A new Builder representing a unary identity value.</returns>
            public Builder<T1, K1> Value<K1>(K1 value)
                => new Builder<T1, K1>(new Identity<T1, K1>(_provider, value));
        }
        /// <summary>
        /// This Builder represents a unary identity value.
        /// </summary>
        /// <typeparam name="T1">The entity type.</typeparam>
        /// <typeparam name="K1">The underlying value type.</typeparam>
        public struct Builder<T1, K1>
        {
            private readonly Identity<T1, K1> identity;

            internal Builder(Identity<T1, K1> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Creates a Builder for a binary identity value.
            /// </summary>
            /// <typeparam name="T2">The entity type of the second component.</typeparam>
            /// <returns>A builder for the new context.</returns>
            public Builder<T1, T2, K1> For<T2>()
                => new Builder<T1, T2, K1>(identity);
            /// <summary>
            /// Constructs the unary identity value.
            /// </summary>
            /// <returns>A unary identity value.</returns>
            public Identity<T1, K1> Id()
                => identity;
            /// <summary>
            /// A Builder can be used with an implicit conversion to the identity value it represents.
            /// </summary>
            /// <param name="bld">The value to convert.</param>
            public static implicit operator Identity<T1, K1>(Builder<T1, K1> bld)
                => bld.Id();
        }
        /// <summary>
        /// This Builder represents a unary identity value with a second component type.
        /// </summary>
        /// <typeparam name="T1">The entity type for the first component.</typeparam>
        /// <typeparam name="T2">The entity type for the second component.</typeparam>
        /// <typeparam name="K1">The underlying value for the first component.</typeparam>
        public struct Builder<T1, T2, K1>
        {
            private readonly Identity<T1, K1> identity;

            internal Builder(Identity<T1, K1> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Sets the underlying value for the second type.
            /// </summary>
            /// <typeparam name="K2">The type of the underlying value.</typeparam>
            /// <param name="value">The underlying value.</param>
            /// <returns>A new Builder representing a binary identity value.</returns>
            public Builder<T1, T2, K1, K2> Value<K2>(K2 value)
                => new Builder<T1, T2, K1, K2>(new Identity<T1, T2, K1, K2>(identity, value));
        }
        /// <summary>
        /// This Builder represents a binary identity value.
        /// </summary>
        /// <typeparam name="T1">The first entity type.</typeparam>
        /// <typeparam name="T2">The second entity type.</typeparam>
        /// <typeparam name="K1">The first underlying value type.</typeparam>
        /// <typeparam name="K2">The second underlying value type.</typeparam>
        public struct Builder<T1, T2, K1, K2>
        {
            private Identity<T1, T2, K1, K2> identity;

            internal Builder(Identity<T1, T2, K1, K2> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Creates a Builder for a 3-ary identity value.
            /// </summary>
            /// <typeparam name="T3">The entity type of the third component.</typeparam>
            /// <returns>A builder for the new context.</returns>
            public Builder<T1, T2, T3, K1, K2> For<T3>()
                => new Builder<T1, T2, T3, K1, K2>(identity);
            /// <summary>
            /// Constructs the binary identity value.
            /// </summary>
            /// <returns>A binary identity value.</returns>
            public Identity<T1, T2, K1, K2> Id()
                => identity;
            /// <summary>
            /// A Builder can be used with an implicit conversion to the identity value it represents.
            /// </summary>
            /// <param name="bld">The value to convert.</param>
            public static implicit operator Identity<T1, T2, K1, K2>(Builder<T1, T2, K1, K2> bld)
                => bld.Id();
        }
        /// <summary>
        /// This Builder represents a binary identity value with a third component type.
        /// </summary>
        /// <typeparam name="T1">The entity type for the first component.</typeparam>
        /// <typeparam name="T2">The entity type for the second component.</typeparam>
        /// <typeparam name="T3">The entity type for the third component.</typeparam>
        /// <typeparam name="K1">The underlying value for the first component.</typeparam>
        /// <typeparam name="K2">The underlying value for the second component.</typeparam>
        public struct Builder<T1, T2, T3, K1, K2>
        {
            private readonly Identity<T1, T2, K1, K2> identity;

            internal Builder(Identity<T1, T2, K1, K2> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Sets the underlying value for the third type.
            /// </summary>
            /// <typeparam name="K3">The type of the underlying value.</typeparam>
            /// <param name="value">The underlying value.</param>
            /// <returns>A new Builder representing a 3-ary identity value.</returns>
            public Builder<T1, T2, T3, K1, K2, K3> Value<K3>(K3 value)
                => new Builder<T1, T2, T3, K1, K2, K3>(new Identity<T1, T2, T3, K1, K2, K3>(identity, value));
        }
        /// <summary>
        /// This Builder represents a 3-ary identity value.
        /// </summary>
        /// <typeparam name="T1">The first entity type.</typeparam>
        /// <typeparam name="T2">The second entity type.</typeparam>
        /// <typeparam name="T3">The third entity type.</typeparam>
        /// <typeparam name="K1">The first underlying value type.</typeparam>
        /// <typeparam name="K2">The second underlying value type.</typeparam>
        /// <typeparam name="K3">The third underlying value type.</typeparam>
        public struct Builder<T1, T2, T3, K1, K2, K3>
        {
            private Identity<T1, T2, T3, K1, K2, K3> identity;

            internal Builder(Identity<T1, T2, T3, K1, K2, K3> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Creates a Builder for a 4-ary identity value.
            /// </summary>
            /// <typeparam name="T4">The entity type of the fourth component.</typeparam>
            /// <returns>A builder for the new context.</returns>
            public Builder<T1, T2, T3, T4, K1, K2, K3> For<T4>()
                => new Builder<T1, T2, T3, T4, K1, K2, K3>(identity);
            /// <summary>
            /// Constructs the 3-ary identity value.
            /// </summary>
            /// <returns>A 3-ary identity value.</returns>
            public Identity<T1, T2, T3, K1, K2, K3> Id()
                => identity;
            /// <summary>
            /// A Builder can be used with an implicit conversion to the identity value it represents.
            /// </summary>
            /// <param name="bld">The value to convert.</param>
            public static implicit operator Identity<T1, T2, T3, K1, K2, K3>(Builder<T1, T2, T3, K1, K2, K3> bld)
                => bld.Id();
        }
        /// <summary>
        /// This Builder represents a 3-ary identity value with a fourth component type.
        /// </summary>
        /// <typeparam name="T1">The entity type for the first component.</typeparam>
        /// <typeparam name="T2">The entity type for the second component.</typeparam>
        /// <typeparam name="T3">The entity type for the third component.</typeparam>
        /// <typeparam name="T4">The entity type for the fourth component.</typeparam>
        /// <typeparam name="K1">The underlying value for the first component.</typeparam>
        /// <typeparam name="K2">The underlying value for the second component.</typeparam>
        /// <typeparam name="K3">The underlying value for the third component.</typeparam>
        public struct Builder<T1, T2, T3, T4, K1, K2, K3>
        {
            private readonly Identity<T1, T2, T3, K1, K2, K3> identity;

            internal Builder(Identity<T1, T2, T3, K1, K2, K3> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Sets the underlying value for the fourth type.
            /// </summary>
            /// <typeparam name="K4">The type of the underlying value.</typeparam>
            /// <param name="value">The underlying value.</param>
            /// <returns>A new Builder representing a 4-ary identity value.</returns>
            public Builder<T1, T2, T3, T4, K1, K2, K3, K4> Value<K4>(K4 value)
                => new Builder<T1, T2, T3, T4, K1, K2, K3, K4>(new Identity<T1, T2, T3, T4, K1, K2, K3, K4>(identity, value));
        }
        /// <summary>
        /// This Builder represents a 4-ary identity value.
        /// </summary>
        /// <typeparam name="T1">The first entity type.</typeparam>
        /// <typeparam name="T2">The second entity type.</typeparam>
        /// <typeparam name="T3">The third entity type.</typeparam>
        /// <typeparam name="T4">The fourth entity type.</typeparam>
        /// <typeparam name="K1">The first underlying value type.</typeparam>
        /// <typeparam name="K2">The second underlying value type.</typeparam>
        /// <typeparam name="K3">The third underlying value type.</typeparam>
        /// <typeparam name="K4">The fourth underlying value type.</typeparam>
        public struct Builder<T1, T2, T3, T4, K1, K2, K3, K4>
        {
            private Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity;

            internal Builder(Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Creates a Builder for a 5-ary identity value.
            /// </summary>
            /// <typeparam name="T5">The entity type of the fifth component.</typeparam>
            /// <returns>A builder for the new context.</returns>
            public Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4> For<T5>()
                => new Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4>(identity);
            /// <summary>
            /// Constructs the 4-ary identity value.
            /// </summary>
            /// <returns>A 4-ary identity value.</returns>
            public Identity<T1, T2, T3, T4, K1, K2, K3, K4> Id()
                => identity;
            /// <summary>
            /// A Builder can be used with an implicit conversion to the identity value it represents.
            /// </summary>
            /// <param name="bld">The value to convert.</param>
            public static implicit operator Identity<T1, T2, T3, T4, K1, K2, K3, K4>(Builder<T1, T2, T3, T4, K1, K2, K3, K4> bld)
                => bld.Id();
        }
        /// <summary>
        /// This Builder represents a 4-ary identity value with a fifth component type.
        /// </summary>
        /// <typeparam name="T1">The entity type for the first component.</typeparam>
        /// <typeparam name="T2">The entity type for the second component.</typeparam>
        /// <typeparam name="T3">The entity type for the third component.</typeparam>
        /// <typeparam name="T4">The entity type for the fourth component.</typeparam>
        /// <typeparam name="T5">The entity type for the fifth component.</typeparam>
        /// <typeparam name="K1">The underlying value for the first component.</typeparam>
        /// <typeparam name="K2">The underlying value for the second component.</typeparam>
        /// <typeparam name="K3">The underlying value for the third component.</typeparam>
        /// <typeparam name="K4">The underlying value for the fourth component.</typeparam>
        public struct Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4>
        {
            private readonly Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity;

            internal Builder(Identity<T1, T2, T3, T4, K1, K2, K3, K4> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Sets the underlying value for the fifth type.
            /// </summary>
            /// <typeparam name="K5">The type of the underlying value.</typeparam>
            /// <param name="value">The underlying value.</param>
            /// <returns>A new Builder representing a 5-ary identity value.</returns>
            public Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> Value<K5>(K5 value)
                => new Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>(new Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>(identity, value));
        }
        /// <summary>
        /// This Builder represents a 5-ary identity value.
        /// </summary>
        /// <typeparam name="T1">The first entity type.</typeparam>
        /// <typeparam name="T2">The second entity type.</typeparam>
        /// <typeparam name="T3">The third entity type.</typeparam>
        /// <typeparam name="T4">The fourth entity type.</typeparam>
        /// <typeparam name="T5">The fifth entity type.</typeparam>
        /// <typeparam name="K1">The first underlying value type.</typeparam>
        /// <typeparam name="K2">The second underlying value type.</typeparam>
        /// <typeparam name="K3">The third underlying value type.</typeparam>
        /// <typeparam name="K4">The fourth underlying value type.</typeparam>
        /// <typeparam name="K5">The fifth underlying value type.</typeparam>
        public struct Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>
        {
            private Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> identity;

            internal Builder(Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> identity)
            {
                this.identity = identity;
            }
            /// <summary>
            /// Constructs the 5-ary identity value.
            /// </summary>
            /// <returns>A 5-ary identity value.</returns>
            public Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> Id()
                => identity;
            /// <summary>
            /// A Builder can be used with an implicit conversion to the identity value it represents.
            /// </summary>
            /// <param name="bld">The value to convert.</param>
            public static implicit operator Identity<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5>(Builder<T1, T2, T3, T4, T5, K1, K2, K3, K4, K5> bld)
                => bld.Id();
        }

        internal static GeneralBuilder BuildGeneralIdentity(this IIdentityProvider provider)
            => new GeneralBuilder(provider);

        /// <summary>
        /// Helper class for building identity values with a fluent interface in a dynamic way.
        /// It is meant for use in Identity Provider implementations.
        /// </summary>
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
            /// <summary>
            /// Adds a component to the Builder.
            /// </summary>
            /// <param name="t">The entity type.</param>
            /// <param name="v">The underlying value type.</param>
            /// <param name="value">The underlying value.</param>
            /// <returns>A new Builder.</returns>
            public GeneralBuilder Add(Type t, Type v, object value)
                => new GeneralBuilder(_provider, _components.Add((t, v, value)));
            /// <summary>
            /// Adds a number of components to the Builder.
            /// </summary>
            /// <param name="items">A collection of components.</param>
            /// <returns>A new builder.</returns>
            public GeneralBuilder AddRange(IEnumerable<(Type, Type, object)> items)
                => new GeneralBuilder(_provider, _components.AddRange(items));
            /// <summary>
            /// Adds a number of components to the Builder.
            /// </summary>
            /// <param name="items">A collection of components.</param>
            /// <returns>A new builder.</returns>
            public GeneralBuilder Add(params (Type, Type, object)[] items)
                => new GeneralBuilder(_provider, _components.AddRange(items));
            /// <summary>
            /// Sets the entity type for the next component.
            /// </summary>
            /// <typeparam name="T">The entity type for the next component.</typeparam>
            /// <returns>A new builder context based on the current and the type of the next component.</returns>
            public Odd<T> For<T>()
                => new Odd<T>(this);
            /// <summary>
            /// Sets the entity type for the next component.
            /// </summary>
            /// <param name="t">The entity type for the next component.</param>
            /// <returns>A new builder context based on the current and the type of the next component.</returns>
            public Odd For(Type t)
                => new Odd(this, t);
            /// <summary>
            /// Helper struct that represents an identity value with a next component entity type.
            /// </summary>
            public struct Odd
            {
                private readonly GeneralBuilder _builder;
                private readonly Type _type;

                internal Odd(GeneralBuilder builder, Type type)
                {
                    _builder = builder;
                    _type = type;
                }
                /// <summary>
                /// Sets the underlying value for the next component.
                /// </summary>
                /// <typeparam name="K">The type of the underlying value.</typeparam>
                /// <param name="value">The underlying value for the next component.</param>
                /// <returns>A new builder with an added component.</returns>
                public GeneralBuilder Value<K>(K value)
                    => _builder.Add(_type, typeof(K), value);
                /// <summary>
                /// Sets the underlying value for the next component.
                /// </summary>
                /// <param name="v">The type of the underlying value.</param>
                /// <param name="value">The underlying value for the next component.</param>
                /// <returns>A new builder with an added component.</returns>
                public GeneralBuilder Value(Type v, object value)
                    => _builder.Add(_type, v, value);
            }
            /// <summary>
            /// Helper struct that represents an identity value with a next component entity type.
            /// </summary>
            /// <typeparam name="T">The next component entity type.</typeparam>
            public struct Odd<T>
            {
                private readonly GeneralBuilder _builder;

                internal Odd(GeneralBuilder builder)
                {
                    _builder = builder;
                }
                /// <summary>
                /// Sets the underlying value for the next component.
                /// </summary>
                /// <typeparam name="K">The type of the underlying value.</typeparam>
                /// <param name="value">The underlying value.</param>
                /// <returns>A new builder with the added component.</returns>
                public Even<T> Value<K>(K value)
                    => new Even<T>(_builder._provider, _builder._components.Add((typeof(T), typeof(K), value)));
                /// <summary>
                /// Sets the underlying value for the next component.
                /// </summary>
                /// <param name="v">The type of the underlying value.</param>
                /// <param name="value">The underlying value.</param>
                /// <returns>A new builder with the added component.</returns>
                public Even<T> Value(Type v, object value)
                    => new Even<T>(_builder._provider, _builder._components.Add((typeof(T), v, value)));
            }
            /// <summary>
            /// A builder representing a typed identity value.
            /// </summary>
            /// <typeparam name="T">The entity type for the identity value.</typeparam>
            public struct Even<T>
            {
                private readonly GeneralBuilder _builder;
                internal Even(IIdentityProvider provider, ImmutableList<(Type, Type, object)> components = null)
                {
                    _builder = new GeneralBuilder(provider, components);

                }
                /// <summary>
                /// Adds a component to the Builder.
                /// </summary>
                /// <param name="t">The entity type.</param>
                /// <param name="v">The underlying value type.</param>
                /// <param name="value">The underlying value.</param>
                /// <returns>A new Builder.</returns>
                public GeneralBuilder Add(Type t, Type v, object value)
                    => _builder.Add(t, v, value);
                /// <summary>
                /// Adds a number of components to the Builder.
                /// </summary>
                /// <param name="items">A collection of components.</param>
                /// <returns>A new builder.</returns>
                public GeneralBuilder AddRange(IEnumerable<(Type, Type, object)> items)
                    => _builder.AddRange(items);
                /// <summary>
                /// Adds a number of components to the Builder.
                /// </summary>
                /// <param name="items">A collection of components.</param>
                /// <returns>A new builder.</returns>
                public GeneralBuilder Add(params (Type, Type, object)[] items)
                    => _builder.Add(items);
                /// <summary>
                /// Converts the typed builder to an untyped builder.
                /// </summary>
                /// <returns>An untyped builder.</returns>
                public GeneralBuilder Builder()
                    => _builder;
                /// <summary>
                /// Sets the entity type for the next component.
                /// </summary>
                /// <typeparam name="U">The entity type for the next component.</typeparam>
                /// <returns>A new builder context based on the current and the type of the next component.</returns>
                public Odd<U> For<U>()
                    => new Odd<U>(_builder);
                /// <summary>
                /// Sets the entity type for the next component.
                /// </summary>
                /// <param name="t">The entity type for the next component.</param>
                /// <returns>A new builder context based on the current and the type of the next component.</returns>
                public Odd For(Type t)
                    => new Odd(_builder, t);
                /// <summary>
                /// Constructs the actual identity value for this builder.
                /// </summary>
                /// <returns>An identity value.</returns>
                public IIdentity<T> Id()
                    => (IIdentity<T>)_builder.Id();
            }
            /// <summary>
            /// Constructs the actual identity value for this builder.
            /// </summary>
            /// <returns>An identity value.</returns>
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
