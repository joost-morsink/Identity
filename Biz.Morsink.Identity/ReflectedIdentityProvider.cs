using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Base class for an Identity Provider that uses reflection for actual identity value creation.
    /// It supports creation of identities by method implementations in the derived class that adhere to one of two method signatures.
    /// Either the method returns an IIdentity&lt;T&gt; and accepts a generically defined parameter, 
    /// or it returns some IIdentity implementing type of arity n, and has n method parameters of any type.
    /// </summary>
    public abstract class ReflectedIdentityProvider : AbstractIdentityProvider
    {
        /// <summary>
        /// Only methods attributed with the CreatorAttribute will be considered as Creators for identities
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class CreatorAttribute : Attribute { }

        /// <summary>
        /// Only methods attributed with the GeneratorAttribute will be considered as Generators for identities.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class GeneratorAttribute : Attribute { }

        private Dictionary<Type, IIdentityCreator> _idCreators;
        private Dictionary<Type, IIdentityCreator> idCreators => _idCreators = _idCreators ??
            // Select all the generic methods.
            (from mi in this.GetType().GetTypeInfo().DeclaredMethods
             where mi.GetCustomAttributes<CreatorAttribute>().Any()
               && mi.ReturnType.GenericTypeArguments.Length == 1 && mi.ReturnType.GetGenericTypeDefinition() == typeof(IIdentity<>)
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
                where mi.GetCustomAttributes<CreatorAttribute>().Any() 
                  && typeof(IIdentity).GetTypeInfo().IsAssignableFrom(mi.ReturnType.GetTypeInfo())
                  && mi.GetGenericArguments().Length == 0
                from itfs in mi.ReturnType.GetTypeInfo().ImplementedInterfaces.Concat(new[] { mi.ReturnType })
                let iti = itfs.GetTypeInfo()
                where iti.IsInterface && iti.GenericTypeArguments.Length == 1 && iti.GetGenericTypeDefinition() == typeof(IIdentity<>)
                let idType = iti.GenericTypeArguments[0]
                where mi.GetParameters().Length > 0 && mi.GetParameters()[0].ParameterType != idType
                select new
                {
                    Key = idType,
                    Value = (IIdentityCreator)Activator.CreateInstance(
                        typeof(MethodInfoIdentityCreator<>).MakeGenericType(idType), this, mi)
                }
              ).ToDictionary(x => x.Key, x => x.Value);
        private Dictionary<Type, IIdentityGenerator> _idGenerators;
        private Dictionary<Type, IIdentityGenerator> idGenerators => _idGenerators = _idGenerators ??
            (from mi in this.GetType().GetTypeInfo().DeclaredMethods
             where mi.GetCustomAttributes<GeneratorAttribute>().Any()
               && mi.ReturnType.GenericTypeArguments.Length == 1 && mi.ReturnType.GetGenericTypeDefinition() == typeof(IIdentity<>)
             let eType = mi.ReturnType.GenericTypeArguments[0]
             where mi.GetGenericArguments().Length == 0
               && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType == eType
             select new
             {
                 Key = eType,
                 Value = (IIdentityGenerator)Activator.CreateInstance(
                     typeof(MethodInfoIdentityGenerator<>).MakeGenericType(eType), this, mi)
             }).ToDictionary(x => x.Key, x => x.Value);

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
        protected override IIdentityCreator GetCreator(Type type)
        {
            return idCreators.TryGetValue(type, out var creator) ? creator : null;
        }
        /// <summary>
        /// Gets a IIdentityCreator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">The type to get an IIdentityCreator for.</typeparam>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected override IIdentityCreator<T> GetCreator<T>()
        {
            return idCreators.TryGetValue(typeof(T), out var creator) ? creator as IIdentityCreator<T> : null;
        }
        /// <summary>
        /// Returns true if the provider has any identity value creating methods.
        /// </summary>
        public override bool SupportsNewIdentities => idGenerators.Count > 0;
        /// <summary>
        /// Gets an IIdentityGenerator instance for some type.
        /// </summary>
        /// <param name="type">he type to get an IIdentityGenerator for.</param>
        /// <returns>An IIdentityGenerator for the specified type.</returns>
        protected override IIdentityGenerator GetGenerator(Type type)
        {
            return idGenerators.TryGetValue(type, out var res) ? res : null;
        }
        /// <summary>
        /// Gets an IIdentityGenerator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">he type to get an IIdentityGenerator for.</typeparam>
        /// <returns>An IIdentityGenerator for the specified type.</returns>
        protected override IIdentityGenerator<T> GetGenerator<T>()
        {
            return idGenerators.TryGetValue(typeof(T), out var res) ? (IIdentityGenerator<T>)res : null;
        }
    }
}
