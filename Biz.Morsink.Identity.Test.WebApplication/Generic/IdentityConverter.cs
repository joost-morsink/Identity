using System;
using Newtonsoft.Json;
using System.Reflection;
using Biz.Morsink.Identity.Test.WebApplication.IdentityProvider;

namespace Biz.Morsink.Identity.Test.WebApplication.Generic
{
    internal class IdentityConverter : JsonConverter
    {
        public static IdentityConverter Instance { get; } = new IdentityConverter();
        public override bool CanConvert(Type objectType)
            => typeof(IIdentity).GetTypeInfo().IsAssignableFrom(objectType);

        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var id = (IIdentity)value;
            var path = ApiIdentityProvider.Instance.ToPath(id);
            writer.WriteValue(path);
        }
    }
}