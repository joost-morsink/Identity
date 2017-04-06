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
        private static MethodInfo createMethod = (from m in typeof(AbstractIdentityProvider).GetTypeInfo().GetDeclaredMethods(nameof(Create))
                                                  where m.IsPublic && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 1
                                                  && m.GetParameters()[0].ParameterType == typeof(object)
                                                  select m).Single();
        private Dictionary<Type, Func<object, IIdentity>> _idCreators;
        private Dictionary<Type, Func<object, IIdentity>> idCreators => _idCreators = _idCreators ??
            (from mi in this.GetType().GetTypeInfo().DeclaredMethods
             where mi.ReturnType.GenericTypeArguments.Length == 1 && mi.ReturnType.GetGenericTypeDefinition() == typeof(IIdentity<>)
               && mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType == typeof(object)
             select new
             {
                 Key = mi.ReturnType.GenericTypeArguments[0],
                 Value = createFunc(mi) //(Func<object, IIdentity>)mi.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(object), mi.ReturnType))
             }).ToDictionary(x => x.Key, x => x.Value);
        private Func<object, IIdentity> createFunc(MethodInfo mi)
        {
            var input = Ex.Parameter(typeof(object), "input");
            var lambda = Ex.Lambda(Ex.Call(Ex.Constant(this), mi, input), input);
            return (Func<object, IIdentity>)lambda.Compile();
        }

        protected Func<object, IIdentity> GetCreator(Type t)
        {
            return idCreators.TryGetValue(t, out var creator) ? creator : null;
        }
        protected Func<object, IIdentity<T>> GetCreator<T>()
        {
            return idCreators.TryGetValue(typeof(T), out var creator) ? (Func<object, IIdentity<T>>)creator : null;
        }

        public virtual IIdentity Create(Type t, object value)
        {
            var creator = GetCreator(t);
            return creator == null ? null : creator(value);
        }
        public virtual IIdentity<T> Create<T>(object value)
        {
            var creator = GetCreator<T>();
            return creator == null ? null : creator(value);
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
        {
            throw new NotImplementedException();
        }
    }
}
