using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biz.Morsink.DataConvert;
using System.Reflection;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Base class for identity providers.
    /// It supports creation of identities by method implementations in the derived class that adhere to one of two method signatures.
    /// Either the method returns an IIdentity&lt;T&gt; and accepts a generically defined parameter, 
    /// or it returns some IIdentity implementing type of arity n, and has n method parameters of any type.
    /// </summary>
    public class AbstractIdentityProvider : IIdentityProvider
    {
        private Dictionary<Type, IIdentityCreator> _idCreators;
        private Dictionary<Type, IIdentityCreator> idCreators => _idCreators = _idCreators ??
            // Select all the generic methods.
            (from mi in this.GetType().GetTypeInfo().DeclaredMethods
             where mi.ReturnType.GenericTypeArguments.Length == 1 && mi.ReturnType.GetGenericTypeDefinition() == typeof(IIdentity<>)
               && mi.GetGenericArguments().Length == 1
               && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType == mi.GetGenericArguments()[0]
             select new
             {
                 Key = mi.ReturnType.GenericTypeArguments[0],
                 Value = (IIdentityCreator)Activator.CreateInstance(
                     typeof(MethodInfoIdentityCreator<>).MakeGenericType(mi.ReturnType.GenericTypeArguments[0]), this, mi)
             }).Concat(
             // Select all the specific methods.
                from mi in this.GetType().GetTypeInfo().DeclaredMethods
                where typeof(IIdentity).GetTypeInfo().IsAssignableFrom(mi.ReturnType.GetTypeInfo())
                  && mi.GetGenericArguments().Length == 0
                from itfs in mi.ReturnType.GetTypeInfo().ImplementedInterfaces.Concat(new[] { mi.ReturnType })
                let iti = itfs.GetTypeInfo()
                where iti.IsInterface && iti.GenericTypeArguments.Length == 1 && iti.GetGenericTypeDefinition() == typeof(IIdentity<>)
                let idType = iti.GenericTypeArguments[0]
                select new
                {
                    Key = idType,
                    Value = (IIdentityCreator)Activator.CreateInstance(
                        typeof(MethodInfoIdentityCreator<>).MakeGenericType(idType), this, mi)
                }
              ).ToDictionary(x => x.Key, x => x.Value);
        private Func<object, IIdentity> createFunc(MethodInfo mi)
        {
            var input = Ex.Parameter(typeof(object), "input");
            var lambda = Ex.Lambda(Ex.Call(Ex.Constant(this), mi, input), input);
            return (Func<object, IIdentity>)lambda.Compile();
        }
        /// <summary>
        /// Gets a IIdentityCreator instance for some type.
        /// </summary>
        /// <param name="type">The type to get an IIdentityCreator for.</param>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected IIdentityCreator GetCreator(Type type)
        {
            return idCreators.TryGetValue(type, out var creator) ? creator : null;
        }
        /// <summary>
        /// Gets a IIdentityCreator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">The type to get an IIdentityCreator for.</typeparam>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected IIdentityCreator<T> GetCreator<T>()
        {
            return idCreators.TryGetValue(typeof(T), out var creator) ? creator as IIdentityCreator<T> : null;
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
                    || x.Value != null
                        && object.Equals(x.Value, GetConverter(x.ForType, true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result));
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
                || x.Value != null
                    && object.Equals(x.Value, GetConverter(typeof(T), true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result);
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
            => DataConverter.Default;
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
    }
}
