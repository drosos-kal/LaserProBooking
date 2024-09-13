using BeautySalonBookingSystem.Models.Entities;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System;
using Newtonsoft.Json;

namespace BeautySalonBookingSystem.Converters
{
    public class GenderJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CustomerGender);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumValue = (CustomerGender)value;
            var description = GetEnumDescription(enumValue);
            writer.WriteValue(description);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private string GetEnumDescription(CustomerGender value)
        {
            if ((int)value < 1) { return ""; }
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
