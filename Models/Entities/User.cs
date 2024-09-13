using MongoDB.Bson;

namespace BeautySalonBookingSystem.Models.Entities
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
