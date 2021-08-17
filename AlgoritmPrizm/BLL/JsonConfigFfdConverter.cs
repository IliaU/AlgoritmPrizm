using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.BLL
{
    public class JsonConfigFfdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            FfdEn marking_type = (FfdEn)value;
            writer.WriteValue(marking_type.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;

            return Enum.Parse(typeof(FfdEn), enumString, true);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
