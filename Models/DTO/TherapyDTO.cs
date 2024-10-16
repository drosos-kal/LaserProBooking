using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace BeautySalonBookingSystem.Models.DTO
{
    public class TherapyDTO
    {
        public string Id { get; set; }
        public List<TherapyAreaDTO> TherapyAreas { get; set; } = new List<TherapyAreaDTO>();

        public string Title { get; set; }
        public string TherapistName { get; set; }
        public string Energy { get; set; }
        public string Pulses { get; set; }
        public string BeamDiameter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdditionalComments { get; set; }
    }

    public class TherapyAreaDTO
    {
        public string AreaName { get; set; }
        public string BeamDiameter { get; set; }
        public string Pulses { get; set; }
        public string Energy { get; set; }
        public string Price { get; set; }
    }
}
