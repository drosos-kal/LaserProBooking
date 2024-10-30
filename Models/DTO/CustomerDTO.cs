using BeautySalonBookingSystem.Converters;
using BeautySalonBookingSystem.Models.Entities;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeautySalonBookingSystem.Models.DTO
{
    public class CustomerDTO
    {
        public string Id { get; set; }  // Parse ID as string
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [JsonConverter(typeof(GenderJsonConverter))]
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
        public List<TherapyDTO> Therapies { get; set; }
    }

    public class GenderEnumProjection
    {
        public int Id { get; set; }
        public string Gender { get; set; }

        public static List<GenderEnumProjection> GetGenderList()
        {
            List<GenderEnumProjection> customerGenders = new List<GenderEnumProjection>();
            foreach (CustomerGender gender in Enum.GetValues(typeof(CustomerGender)))
            {
                customerGenders.Add(new GenderEnumProjection
                {
                    Id = (int)gender,
                    Gender = gender.ToDescriptionString()
                });
            }

            return customerGenders;
        }
    }

    
}
