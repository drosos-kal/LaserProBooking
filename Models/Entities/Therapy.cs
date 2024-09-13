using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;

namespace BeautySalonBookingSystem.Models.Entities
{
    public class Therapy
    {
        public ObjectId Id { get; set; }  // This would be the MongoDB ObjectId

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
