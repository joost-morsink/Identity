using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biz.Morsink.DataConvert;
using System.Reflection;
using Ex = System.Linq.Expressions.Expression;
using Biz.Morsink.DataConvert.Converters;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Base class for identity providers.
    /// Forwards actual creation of identity values to two methods called 'GetCreator'.
    /// </summary>
    public abstract class AbstractIdentityProvider : IIdentityProvider
    {
        /// <summary>
        /// Gets an IIdentityCreator instance for some type.
        /// </summary>
        /// <param name="type">The type to get an IIdentityCreator for.</param>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected abstract IIdentityCreator GetCreator(Type type);
        /// <summary>
        /// Gets an IIdentityCreator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">The type to get an IIdentityCreator for.</typeparam>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected abstract IIdentityCreator<T> GetCreator<T>();

        /// <summary>
        /// Gets an IIdentityGenerator instance for some type.
        /// </summary>
        /// <param name="type">The type to get an IIdentityGenerator for.</param>
        /// <returns>An IIdentityGenerator for the specified type.</returns>
        protected virtual IIdentityGenerator GetGenerator(Type type)
        {
            throw new InvalidOperationException();
        }
        /// <summary>
        /// Gets an IIdentityGenerator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">he type to get an IIdentityGenerator for.</typeparam>
        /// <returns>An IIdentityGenerator for the specified type.</returns>
        protected virtual IIdentityGenerator<T> GetGenerator<T>()
        {
            throw new InvalidOperationException();
        }
        /// <summary>
        /// Creates an identity value for a certain type of a certain value type.
        /// </summary>
        /// <typeparam name="K">The type of the underlying value.</typeparam>
        /// <param name="forType">The type of object the identity value refers to.</param>
        /// <param name="value">The underlying value.</param>
        /// <returns>If the provider is able to construct an identity value for the specified parameters, the identity value. Null otherwise.</returns>
        public virtual IIdentity Create<K>(Type t, K value)
        {
            var creator = GetCreator(t);
            return creator == null ? null : creator.Create(value);
        }
        /// <summary>
        /// Creates an identity value for a certain type of a certain value type.
        /// </summary>
        /// <typeparam name="T">The type of object the identity value refers to.</typeparam>
        /// <typeparam name="K">The type of the underlying value.</typeparam>
        /// <param name="value">The underlying value.</param>
        /// <returns>If the provider is able to construct an identity value for the specified parameters, the identity value. Null otherwise.</returns>
        public virtual IIdentity<T> Create<T, K>(K value)
        {
            var creator = GetCreator<T>();
            return creator == null ? null : creator.Create(value);
        }
        /// <summary>
        /// Creates a new identity value for some entity of some type.
        /// </summary>
        /// <param name="forType">The type of entity the identity value is created for.</param>
        /// <param name="entity">The entity the identity value is created for.</param>
        /// <returns>A new and unique identity value for the entity.</returns>
        /// <exception cref="InvalidOperationException">When SupportsNewIdentities is false.</exception>
        public virtual IIdentity New(Type forType, object entity)
        {
            var generator = GetGenerator(forType);
            return generator == null ? null : generator.New(entity);
        }
        /// <summary>
        /// Creates a new identity value for some entity of some type.
        /// </summary>
        /// <typeparam name="T">The type of entity the identity value is created for.</typeparam>
        /// <param name="entity">The entity the identity value is created for.</param>
        /// <returns>A new and unique identity value for the entity.</returns>
        /// <exception cref="InvalidOperationException">When SupportsNewIdentities is false.</exception>
        public virtual IIdentity<T> New<T>(T entity)
        {
            var generator = GetGenerator<T>();
            return generator == null ? null : generator.New(entity);
        }
        /// <summary>
        /// Indicates whether this identity provider supports the generation of new identity values for new entities.
        /// Default implementation returns false.
        /// </summary>
        public virtual bool SupportsNewIdentities => false;

        /// <summary>
        /// Evaluates equality for to IIdentity values.
        /// Assumes the first has this as provider.
        /// The second may be translated by this to align underlying value types.
        /// </summary>
        /// <param name="x">The first operand of equality</param>
        /// <param name="y">The second operand of equality</param>
        /// <returns>Whether the two identity values are considered equal by this provider.</returns>
        public virtual bool Equals(IIdentity x, IIdentity y)
            => object.ReferenceEquals(x, y)
            || x.ForType == y.ForType
                && (object.ReferenceEquals(x.Value, y.Value)
                    || x.Provider == y.Provider && object.Equals(x.Value, y.Value)
                    || x.Value != null && object.Equals(x.Value, GetConverter(x.ForType, true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result));
        /// <summary>
        /// Evaluates equality for to IIdentity values.
        /// Assumes the first has this as provider.
        /// The second may be translated by this to align underlying value types.
        /// This is a shortcut for equality when the identity type is known statically.
        /// </summary>
        /// <typeparam name="T">The type of object the identity values refer to.</typeparam>
        /// <param name="x">The first operand of equality</param>
        /// <param name="y">The second operand of equality</param>
        /// <returns>Whether the two identity values are considered equal by this provider.</returns>
        public virtual bool Equals<T>(IIdentity<T> x, IIdentity<T> y)
            => object.ReferenceEquals(x, y)
                || object.ReferenceEquals(x.Value, y.Value)
                    || x.Provider == y.Provider && object.Equals(x.Value, y.Value)
                    || x.Value != null && object.Equals(x.Value, GetConverter(typeof(T), true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result);
        /// <summary>
        /// Gets a hashcode for an identity value.
        /// </summary>
        /// <param name="obj">The identity value to get the hashcode for.</param>
        /// <returns>A hashcode for the specified identity value.</returns>
        public int GetHashCode(IIdentity obj)
            => obj.ForType.GetHashCode() ^ (obj.Value?.GetHashCode() ?? 0);

        /// <summary>
        /// Gets a data converter for converting underlying identity values.
        /// </summary>
        /// <param name="t">The type of object the identity value refers to.</param>
        /// <param name="incoming">Indicates if the converter should handle incoming or outgoing conversions.</param>
        /// <returns>An IDataConverter that is able to make conversions between different types of values.</returns>
        public virtual IDataConverter GetConverter(Type t, bool incoming)
            => Converters.DefaultPipeline;
        /// <summary>
        /// Tries to translate some identity value to one that is owned by this identity provider.
        /// </summary>
        /// <param name="id">The externally owned identity value.</param>
        /// <returns>If the provider is able to understand the incoming identity value, a newly constructed identity value.
        /// If the identity value cannot be translated, null.</returns>
        public virtual IIdentity Translate(IIdentity id)
            => Create(id.ForType, id.Value);
        /// <summary>
        /// Tries to translate some identity value to one that is owned by this identity provider.
        /// </summary>
        /// <typeparam name="T">The type the identity value refers to.</typeparam>
        /// <param name="id">The externally owned identity value.</param>
        /// <returns>If the provider is able to understand the incoming identity value, a newly constructed identity value.
        /// If the identity value cannot be translated, null.</returns>
        public virtual IIdentity<T> Translate<T>(IIdentity<T> id)
            => Create<T, object>(id.Value);

        /// <summary>
        /// Contains convenience methods and properties for data conversion pipeline construction.
        /// </summary>
        public static class Converters
        {
            /// <summary>
            /// Contains short circuit converters (Identity)
            /// </summary>
            public static IEnumerable<IConverter> ShortCircuit => new IConverter[] { IdentityConverter.Instance };
            /// <summary>
            /// Contains high priority converters (IsoDateTime, Base64, ToString, TryParse)
            /// </summary>
            public static IEnumerable<IConverter> HighPriority => new IConverter[]
            {
                IsoDateTimeConverter.Instance,
                Base64Converter.Instance,
                new ToStringConverter(true)
                    .Restrict((from, to) => !from.Name.Contains("Tuple")), // Exclude tuples.
                new TryParseConverter()
            };
            /// <summary>
            /// Contains regular converters (EnumToNumeric, SimpleNumeric, NumericToEnum, EnumParse, ToNullable, Tuple, EnumerableToTuple)
            /// </summary>
            public static IEnumerable<IConverter> Regular => new IConverter[]
            {
                EnumToNumericConverter.Instance,
                SimpleNumericConverter.Instance,
                new NumericToEnumConverter(),
                EnumParseConverter.CaseInsensitive,
                new ToNullableConverter(),
                TupleConverter.Instance,
                EnumerableToTupleConverter.Instance,
                TupleToArrayConverter.Instance
            };
            /// <summary>
            /// Contains fallback converters (FromStringRepresentation, Dynamic)
            /// </summary>
            public static IEnumerable<IConverter> Fallback => new IConverter[]
            {
                new FromStringRepresentationConverter()
                    .Restrict((from, to) => from != typeof(Version)) // Version could conflict with numeric types' syntaxes.
                    .Restrict((from, to) => !from.Name.Contains("Tuple")), // Exclude tuples.
                new DynamicConverter()
            };
            /// <summary>
            /// Contains the default pipeline of short circuit, high priority, regular and fallback converters.
            /// </summary>
            public static DataConverter DefaultPipeline { get; } =
                 new DataConverter(
                     ShortCircuit 
                        .Concat(HighPriority)
                        .Concat(Regular)
                        .Concat(Fallback));
            /// <summary>
            /// Creates a new pipeline of converters, allowing overrides for each of the four stages.
            /// </summary>
            /// <param name="shortCircuit">Optional override for short circuit converters.</param>
            /// <param name="highPriority">Optional override for high priority converters.</param>
            /// <param name="regular">Optional override for regular converters.</param>
            /// <param name="fallback">Optional override for fallback converters.</param>
            /// <returns>A DataConverter pipeline.</returns>
            public static DataConverter CreatePipeline(
                IEnumerable<IConverter> shortCircuit = null,
                IEnumerable<IConverter> highPriority = null,
                IEnumerable<IConverter> regular = null,
                IEnumerable<IConverter> fallback = null)
            {
                return new DataConverter(
                    (shortCircuit ?? ShortCircuit)
                    .Concat(highPriority ?? HighPriority)
                    .Concat(regular ?? Regular)
                    .Concat(fallback ?? Fallback));
            }
            /// <summary>
            /// Creates a default pipeline, extended with separated strings.
            /// </summary>
            /// <param name="separator">The separator character.</param>
            /// <returns>A DataConverter pipeline.</returns>
            public static DataConverter WithSeparator(char separator) => CreatePipeline(
            regular: Regular.Concat(new[] {
                new LosslessStringToTupleConverter(separator),
                new SeparatedStringConverter(separator).Restrict((f, t) => f != typeof(string))
            }));
        }
    }
}
