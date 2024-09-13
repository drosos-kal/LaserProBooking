using BeautySalonBookingSystem.Data;
using BeautySalonBookingSystem.Models.DTO;
using BeautySalonBookingSystem.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BeautySalonBookingSystem.Services
{
    public class UserService
    {

        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _usersCollection = database.GetCollection<User>("Users");
        }

        public async Task<UserDTO> GetUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) return null;
            var filter = Builders<User>.Filter.Eq("Username", username);
            var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            
            if (user == null) return null;
            var userToReturn = UserToDTO(user);
            return userToReturn;
        }

        public async Task<User> CreateUserAsync(UserDTO userDto)
        {
            if (userDto == null) { return null; }
            var userToCreate = UserToModel(userDto);
            await _usersCollection.InsertOneAsync(userToCreate);
            return userToCreate;
        }

        public UserDTO UserToDTO(User user)
        {
            if (user == null) return null;
            var userDTO = new UserDTO
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Password = user.Password,
                Role = user.Role,
            };
            return userDTO;
        }

        public User UserToModel(UserDTO userDto)
        {
            if (userDto == null) return null;
            var user = new User
            {
                Id = string.IsNullOrEmpty(userDto.Id) ? ObjectId.GenerateNewId() : new ObjectId(userDto.Id),
                Username = userDto.Username,
                Password = userDto.Password,
                Role = userDto.Role
            };

            return user;
        }


    }
}
