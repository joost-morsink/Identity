using System;
using System.Linq;
using System.Reflection;

namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// Helper class that for ValueTuples recursively uses equality comparers from the instance to implement the equality comparer for the ValueTuple.
    /// </summary>
    public class RecursiveValueTupleEqualityComparers : EqualityComparers
    {
        private static Type valueTupleComparers(int arity)
        {
            switch (arity)
            {
                case 2:
                    return typeof(ValueTupleComparer<,>);
                case 3:
                    return typeof(ValueTupleComparer<,,>);
                case 4:
                    return typeof(ValueTupleComparer<,,,>);
                case 5:
                    return typeof(ValueTupleComparer<,,,,>);
                default:
                    throw new ArgumentOutOfRangeException(nameof(arity), "Supported tuple sizes are only in the range [2, 5]");
            }
        }
        /// <summary>
        /// RecursiveValueTupleEqualityComparer recursively uses comparers from the current instance to implement equality comparison for the component values.
        /// For other types it uses the default implementation.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected override object Create(Type t)
        {
            if (t.Namespace == nameof(System) && t.Name.StartsWith(nameof(ValueTuple) + "`"))
            {
                var generics = t.GetTypeInfo().GenericTypeArguments;
                var type = valueTupleComparers(generics.Length).MakeGenericType(generics);
                return Activator.CreateInstance(type, generics.Select(Get).ToArray());
            }
            else
                return base.Create(t);
        }
    }
}
