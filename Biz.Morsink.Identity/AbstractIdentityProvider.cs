using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biz.Morsink.DataConvert;
using System.Reflection;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity
{
    public class AbstractIdentityProvider : IIdentityProvider
    {

        private Dictionary<Type, IIdentityCreator> _idCreators;
        private Dictionary<Type, IIdentityCreator> idCreators => _idCreators = _idCreators ??
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

        protected IIdentityCreator GetCreator(Type t)
        {
            return idCreators.TryGetValue(t, out var creator) ? creator : null;
        }
        protected IIdentityCreator<T> GetCreator<T>()
        {
            return idCreators.TryGetValue(typeof(T), out var creator) ? creator as IIdentityCreator<T> : null;
        }

        public virtual IIdentity Create<K>(Type t, K value)
        {
            var creator = GetCreator(t);
            return creator == null ? null : creator.Create(value);
        }
        public virtual IIdentity<T> Create<T, K>(K value)
        {
            var creator = GetCreator<T>();
            return creator == null ? null : creator.Create(value);
        }
        public virtual bool Equals(IIdentity x, IIdentity y)
            => object.ReferenceEquals(x, y)
            || x.ForType == y.ForType
                && (object.ReferenceEquals(x.Value, y.Value)
                    || x.Provider == y.Provider && object.Equals(x.Value, y.Value)
                    || x.Value != null
                        && object.Equals(x.Value, GetConverter(x.ForType, true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result));



        public virtual bool Equals<T>(IIdentity<T> x, IIdentity<T> y)
            => object.ReferenceEquals(x, y)
                || object.ReferenceEquals(x.Value, y.Value)
                || x.Provider == y.Provider && object.Equals(x.Value, y.Value)
                || x.Value != null
                    && object.Equals(x.Value, GetConverter(typeof(T), true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result);


        public virtual IDataConverter GetConverter(Type t, bool incoming)
            => DataConverter.Default;

        public int GetHashCode(IIdentity obj)
            => obj.ForType.GetHashCode() ^ (obj.Value?.GetHashCode() ?? 0);

        public virtual IIdentity Translate(IIdentity id)
            => Create(id.ForType, id.Value);
        public virtual IIdentity<T> Translate<T>(IIdentity<T> id)
            => Create<T, object>(id.Value);

    }
}
