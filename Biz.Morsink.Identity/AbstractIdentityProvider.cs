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

        private Dictionary<Type, IValueToIdentity> _idCreators;
        private Dictionary<Type, IValueToIdentity> idCreators => _idCreators = _idCreators ??
            (from mi in this.GetType().GetTypeInfo().DeclaredMethods
             where mi.ReturnType.GenericTypeArguments.Length == 1 && mi.ReturnType.GetGenericTypeDefinition() == typeof(IIdentity<>)
               && mi.GetGenericArguments().Length == 1
               && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType == mi.GetGenericArguments()[0]
             select new
             {
                 Key = mi.ReturnType.GenericTypeArguments[0],
                 Value = (IValueToIdentity)Activator.CreateInstance(
                     typeof(ValueToIdentity<>).MakeGenericType(mi.ReturnType.GenericTypeArguments[0]), this, mi)
             }).ToDictionary(x => x.Key, x => x.Value);
        private Func<object, IIdentity> createFunc(MethodInfo mi)
        {
            var input = Ex.Parameter(typeof(object), "input");
            var lambda = Ex.Lambda(Ex.Call(Ex.Constant(this), mi, input), input);
            return (Func<object, IIdentity>)lambda.Compile();
        }

        protected IValueToIdentity GetCreator(Type t)
        {
            return idCreators.TryGetValue(t, out var creator) ? creator : null;
        }
        protected IValueToIdentity<T> GetCreator<T>()
        {
            return idCreators.TryGetValue(typeof(T), out var creator) ? creator as IValueToIdentity<T> : null;
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
                && object.Equals(x.Value, GetConverter(x.ForType, true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result);

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
