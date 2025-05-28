using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeautySalonBookingSystem.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class Therapy
    {
        public ObjectId Id { get; set; }  // This would be the MongoDB ObjectId
        public string Title { get; set; }
        public string AppointmentType { get; set; }
        public List<TherapyArea> TherapyAreas { get; set; }
        public string TherapistName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdditionalComments { get; set; }

    }

    [BsonIgnoreExtraElements]
    public class TherapyArea
    {
        public string AreaName { get; set; }
        public string BeamDiameter { get; set; }
        public string Pulses { get; set; }
        public string Energy { get; set; }
        public string Price { get; set; }
    }

    
}
