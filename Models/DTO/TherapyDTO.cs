using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace BeautySalonBookingSystem.Models.DTO
{
    public class TherapyDTO
    {
        public string Id { get; set; } 
        public string Title { get; set; }
        public string TherapistName { get; set; }
        public string Energy { get; set; }
        public string Pulses { get; set; }
        public string BeamDiameter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdditionalComments { get; set; }
    }
}
