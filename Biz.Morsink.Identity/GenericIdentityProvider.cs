using System;
using System.Collections.Generic;
using System.Text;
using Biz.Morsink.DataConvert;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This identity provider supports identity value creation for all entity types based on a single key type.
    /// </summary>
    /// <typeparam name="KO">The type of the underlying values.</typeparam>
    public class GenericIdentityProvider<KO> : AbstractIdentityProvider
    {
        private readonly IDataConverter _converter;

        /// <summary>
        /// This creator support creating the actual identities for the GenericIdentityProvider.
        /// </summary>
        /// <typeparam name="T">The type the identity values refer to.</typeparam>
        protected class Creator<T> : IIdentityCreator<T>
        {
            private readonly GenericIdentityProvider<KO> _parent;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent">A reference to the parent class.</param>
            public Creator(GenericIdentityProvider<KO> parent)
            {
                _parent = parent;
            }
            /// <summary>
            /// Create an identity value for a type T.
            /// </summary>
            /// <typeparam name="KI">The underlying input type of the identity value.</typeparam>
            /// <param name="value">The underlying input value of the identity value.</param>
            /// <returns>A newly constructed identity value for type T with a converted underlying value.
            /// Null if conversion failed.</returns>
            public IIdentity<T> Create<KI>(KI value)
                => _parent._converter.Convert(value).TryTo(out KO x)
                    ? new Identity<T, KO>(_parent, x)
                    : null;

            IIdentity IIdentityCreator.Create<KI>(KI value)
                => Create(value);
        }
        /// <summary>
        /// Constructor.
        /// Takes a IDataConverter to take care of the data conversions necessary for the underlying values.
        /// </summary>
        /// <param name="converter"></param>
        public GenericIdentityProvider(IDataConverter converter = null)
        {
            _converter = converter ?? Converters.DefaultPipeline;
        }

        /// <summary>
        /// Gets a IIdentityCreator instance for some type.
        /// </summary>
        /// <param name="type">The type to get an IIdentityCreator for.</param>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected override IIdentityCreator GetCreator(Type type)
        {
            return (IIdentityCreator)Activator.CreateInstance(typeof(Creator<>).MakeGenericType(type), this);
        }

        /// <summary>
        /// Gets a IIdentityCreator&lt;T&gt; instance for some type.
        /// </summary>
        /// <typeparam name="T">The type to get an IIdentityCreator for.</typeparam>
        /// <returns>An IIdentityCreator for the specified type.</returns>
        protected override IIdentityCreator<T> GetCreator<T>()
            => new Creator<T>(this);

        /// <summary>
        /// This simple Identity Provider always returns the same IDataConverter.
        /// </summary>
        public override IDataConverter GetConverter(Type t, bool incoming)
            => _converter;
    }
}
