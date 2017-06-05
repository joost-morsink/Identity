using System;
using Newtonsoft.Json;
using System.Reflection;
using Biz.Morsink.Identity.Test.WebApplication.IdentityProvider;

namespace Biz.Morsink.Identity.Test.WebApplication.Generic
{
    internal class IdentityConverter : JsonConverter
    {
        public static IdentityConverter Instance { get; } = new IdentityConverter();
        /// <summary>
        /// This Converter converts properties of type IIdentity.
        /// </summary>
        /// <param name="objectType">The type to convert.</param>
        /// <returns>True if the type to convert implements IIdentity.</returns>
        public override bool CanConvert(Type objectType)
            => typeof(IIdentity).GetTypeInfo().IsAssignableFrom(objectType);

        /// <summary>
        /// This example component has no read implementation.
        /// </summary>
        public override bool CanRead => false;
        /// <summary>
        /// True.
        /// </summary>
        public override bool CanWrite => true;
        /// <summary>
        /// Throws a NotImplementedException.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Writes a value to a JsonWriter.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The IIdentity value to write.</param>
        /// <param name="serializer">The serializer used to write to the JsonWriter.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var id = (IIdentity)value;
            var path = ApiIdentityProvider.Instance.ToPath(id);
            writer.WriteValue(path);
        }
    }
}