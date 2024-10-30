using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeautySalonBookingSystem.Models.Entities
{
    public class Customer
    {
        public ObjectId Id { get; set; } 

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Gender { get; set; }
        public DateTime? Age { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Medication { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string AdditionalComments { get; set; }
        public string TherapyPlan { get; set; }
        public string SkinPhototype { get; set; }
        public List<Therapy> Therapies { get; set; }


    }

    public enum CustomerGender
    {
        [Description("Άντρας")]
        Male = 1,
        [Description("Γυναίκα")]
        Female = 2,
        [Description("Άλλο")]
        Other = 3,
    }

    public static class MyEnumExtensions
    {
        public static string ToDescriptionString(this CustomerGender val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
