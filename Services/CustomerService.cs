using BeautySalonBookingSystem.Data;
using BeautySalonBookingSystem.Models.DTO;
using BeautySalonBookingSystem.Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalonBookingSystem.Services
{
    public class CustomerService
    {
        private readonly IMongoCollection<Customer> _customersCollection;

        public CustomerService(IDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _customersCollection = database.GetCollection<Customer>("Customers");

        }

        public async Task<List<TherapyWithCustomerInfoDTO>> GetTherapiesWithCustomerInfoAsync()
        {
            var pipeline = new BsonDocument[]
            {
                BsonDocument.Parse("{ $unwind: '$Therapies' }"), // Unwind the Therapies array
                BsonDocument.Parse("{ $project: { _id: 0, Therapy: '$Therapies', Firstname: 1, Lastname: 1 } }") // Project the unwound Therapies with Firstname and Lastname
            };

            // Execute the aggregation pipeline
            var cursor = await _customersCollection.AggregateAsync<BsonDocument>(pipeline);
            if (cursor == null) { return null; }

            // Deserialize each document in the cursor into a TherapyWithCustomerInfoDTO object
            var therapiesWithCustomerInfo = new List<TherapyWithCustomerInfoDTO>();
            await cursor.ForEachAsync(document =>
            {
                var therapyDocument = document["Therapy"].AsBsonDocument;
                if (therapyDocument == null) { return; }

                var therapy = BsonSerializer.Deserialize<Therapy>(therapyDocument);
                var firstnameBsonValue = document.GetValue("Firstname", BsonNull.Value);
                var lastnameBsonValue = document.GetValue("Lastname", BsonNull.Value);

                var therapyWithCustomerInfo = new TherapyWithCustomerInfoDTO()
                {
                    TherapyDto = TherapyToDTO(therapy),
                    Firstname = firstnameBsonValue.IsBsonNull ? string.Empty : firstnameBsonValue.AsString,
                    Lastname = lastnameBsonValue.IsBsonNull ? string.Empty : lastnameBsonValue.AsString
                };

                therapiesWithCustomerInfo.Add(therapyWithCustomerInfo);
            });

            return therapiesWithCustomerInfo;
        }

        public async Task<List<TherapyDTO>> GetTherapiesAsync()
        {
            var pipeline = new BsonDocument[]
                {
                    BsonDocument.Parse("{ $unwind: '$Therapies' }"), // Unwind the Therapies array
                    BsonDocument.Parse("{ $project: { _id: 0, Therapy: '$Therapies' } }") // Project the unwound Therapies
                };

            // Execute the aggregation pipeline
            var cursor = await _customersCollection.AggregateAsync<BsonDocument>(pipeline);
            if (cursor == null) { return null; }
            // Deserialize each document in the cursor into a Therapy object
            var therapies = new List<TherapyDTO>();
            await cursor.ForEachAsync(document =>
            {
                var therapyDocument = document["Therapy"].AsBsonDocument;
                if (therapyDocument == null) { return; }
                var therapy = BsonSerializer.Deserialize<Therapy>(therapyDocument);
                therapies.Add(TherapyToDTO(therapy));
            });

            return therapies;

        }

        public async Task UpdateTherapyAsync(string customerId, TherapyDTO therapyDto)
        {
            if (therapyDto == null) { return; }

            var filter = Builders<Customer>.Filter.And(
                Builders<Customer>.Filter.Eq("_id", new ObjectId(customerId)),
                Builders<Customer>.Filter.ElemMatch("Therapies", Builders<Therapy>.Filter.Eq("_id", new ObjectId(therapyDto.Id)))
            );

            var update = Builders<Customer>.Update
                .Set("Therapies.$.Title", therapyDto.Title)
                .Set("Therapies.$.TherapistName", therapyDto.TherapistName)
                .Set("Therapies.$.Energy", therapyDto.Energy)
                .Set("Therapies.$.Pulses", therapyDto.Pulses)
                .Set("Therapies.$.BeamDiameter", therapyDto.BeamDiameter)
                .Set("Therapies.$.StartDate", therapyDto.StartDate.ToUniversalTime())
                .Set("Therapies.$.EndDate", therapyDto.StartDate.AddMinutes(30))
                .Set("Therapies.$.AdditionalComments", therapyDto.AdditionalComments)
                .Set("Therapies.$.TherapyAreas", therapyDto.TherapyAreas.Select(area => new
                {
                    AreaName = area.AreaName,
                    BeamDiameter = area.BeamDiameter,
                    Pulses = area.Pulses,
                    Price = area.Price,
                    Energy = area.Energy
                }).ToList());

            var result = await _customersCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Dictionary<string, TherapyDTO>> GetTherapyAsync(string id)
        {

            if (string.IsNullOrEmpty(id)) return null;
            var resultDictionary = new Dictionary<string, TherapyDTO>();
            var pipeline = new[]
            {
            new BsonDocument("$unwind", "$Therapies"),
            new BsonDocument("$match", new BsonDocument("Therapies._id", new ObjectId(id))),
            new BsonDocument("$project", new BsonDocument
            {
                { "Therapies", 1 },
                { "customerId", "$_id" },
                { "_id", 0 }
            })
            };

            // Execute the aggregation
            var result = await _customersCollection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            if (result == null || result["Therapies"] == null || result["customerId"] == null) return null;
            var therapyDocument = result["Therapies"].AsBsonDocument;
            var customerId = result.GetValue("customerId").ToString();
            Therapy therapy = BsonSerializer.Deserialize<Therapy>(therapyDocument);
            var therapyToReturn = TherapyToDTO(therapy);
            resultDictionary.Add(customerId, therapyToReturn);
            return resultDictionary;
        }

        public async Task DeleteTherapyAsync(string customerId, string therapyId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(therapyId)) return;

            var filter = Builders<Customer>.Filter.And(
                Builders<Customer>.Filter.Eq("_id", new ObjectId(customerId)),
                Builders<Customer>.Filter.ElemMatch(c => c.Therapies, t => t.Id == new ObjectId(therapyId))
            );

            var update = Builders<Customer>.Update.PullFilter(c => c.Therapies, t => t.Id == new ObjectId(therapyId));

            var result = await _customersCollection.UpdateOneAsync(filter, update);
        }

        public async Task<List<CustomerDTO>> GetCustomers()
        {
            var customers = await _customersCollection.Find(customer => true).ToListAsync();
            var customersDto = new List<CustomerDTO>();
            foreach (var customer in customers)
            {
                var customerDto = CustomerToDTO(customer);
                customersDto.Add(customerDto);
            }
            return customersDto;
        }

        public async Task<CustomerDTO> GetCustomer(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var idToSearch = new ObjectId(id);

            var filter = Builders<Customer>.Filter.Eq("_id", idToSearch);
            var customer = await _customersCollection.Find(filter).FirstOrDefaultAsync();

            if (customer == null) { return null; }
            var customerToReturn = CustomerToDTO(customer);
            return customerToReturn;
        }

        public async Task<Customer> CreateCustomerAsync(CustomerDTO customerDto)
        {
            var customerToCreate = CustomerToModel(customerDto);
            await _customersCollection.InsertOneAsync(customerToCreate);
            return customerToCreate;
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var idToSearch = new ObjectId(id);
            var filter = Builders<Customer>.Filter.Eq("_id", idToSearch);
            var result = await _customersCollection.DeleteOneAsync(filter);
        }

        public async Task UpdateCustomer(CustomerDTO customerDto)
        {
            if (customerDto == null) { return; }
            if (customerDto.Therapies == null)
            {
                customerDto.Therapies = new List<TherapyDTO>();
            }

            List<Therapy> therapies = new List<Therapy>();
            foreach (var therapyDto in customerDto.Therapies)
            {
                therapies.Add(TherapyToModel(therapyDto));
            }

            var filter = Builders<Customer>.Filter.Eq("_id", new ObjectId(customerDto.Id));
            var update = Builders<Customer>.Update
                .Set(c => c.Firstname, customerDto.Firstname)
                .Set(c => c.Lastname, customerDto.Lastname)
                .Set(c => c.Gender, customerDto.Gender)
                .Set(c => c.Age, customerDto.Age)
                .Set(c => c.MobileNumber, customerDto.MobileNumber)
                .Set(c => c.Email, customerDto.Email)
                .Set(c => c.Address, customerDto.Address)
                .Set(c => c.City, customerDto.City)
                .Set(c => c.PostalCode, customerDto.PostalCode)
                .Set(c => c.AdditionalComments, customerDto.AdditionalComments)
                .Set(c => c.Medication, customerDto.Medication)
                .Set(c => c.Therapies, therapies);
            var result = await _customersCollection.UpdateOneAsync(filter, update);
        }

        #region Mappers


        public CustomerDTO CustomerToDTO(Customer customer)
        {
            if (customer == null) { return null; }
            List<TherapyDTO> therapies = new List<TherapyDTO>();
            foreach (var therapy in customer.Therapies)
            {
                var therapyDto = TherapyToDTO(therapy);
                therapies.Add(therapyDto);
            }

            var customerDto = new CustomerDTO
            {
                Id = customer.Id.ToString(),
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Age = customer.Age,
                Gender = customer.Gender,
                MobileNumber = customer.MobileNumber,
                Email = customer.Email,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Medication = customer.Medication,
                AdditionalComments = customer.AdditionalComments,
                Therapies = therapies
            };

            return customerDto;
        }

        public Customer CustomerToModel(CustomerDTO customerDto)
        {
            if (customerDto == null) { return null; }
            if (customerDto.Therapies == null)
            {
                customerDto.Therapies = new List<TherapyDTO>();
            }
            List<Therapy> therapies = new List<Therapy>();
            foreach (var therapyDto in customerDto.Therapies)
            {
                therapies.Add(TherapyToModel(therapyDto));
            }
            return new Customer
            {
                Id = string.IsNullOrEmpty(customerDto.Id) ? ObjectId.GenerateNewId() : new ObjectId(customerDto.Id),
                Firstname = customerDto.Firstname,
                Lastname = customerDto.Lastname,
                Age = customerDto.Age,
                Gender = customerDto.Gender,
                MobileNumber = customerDto.MobileNumber,
                Email = customerDto.Email,
                Address = customerDto.Address,
                City = customerDto.City,
                PostalCode = customerDto.PostalCode,
                Medication = customerDto.Medication,
                AdditionalComments = customerDto.AdditionalComments,
                Therapies = therapies
            };
        }

        private TherapyDTO TherapyToDTO(Therapy therapy)
        {
            if (therapy == null) { return null; }
            var therapyDto = new TherapyDTO
            {

                Id = therapy.Id.ToString(),
                Title = therapy.Title,
                TherapistName = therapy.TherapistName,
                Energy = therapy.Energy,
                Pulses = therapy.Pulses,
                BeamDiameter = therapy.BeamDiameter,
                StartDate = therapy.StartDate.ToLocalTime(),
                EndDate = therapy.EndDate.ToLocalTime().AddMinutes(30),
                AdditionalComments = therapy.AdditionalComments,
                TherapyAreas = therapy.TherapyAreas?.Select(area => new TherapyAreaDTO
                {
                    AreaName = area.AreaName,
                    BeamDiameter = area.BeamDiameter,
                    Pulses = area.Pulses,
                    Energy = area.Energy,
                    Price = area.Price
                }).ToList()
            };
            return therapyDto;
        }

        private Therapy TherapyToModel(TherapyDTO therapyDto)
        {
            if (therapyDto == null) { return null; }

            var therapy = new Therapy
            {
                Id = string.IsNullOrEmpty(therapyDto.Id) ? ObjectId.GenerateNewId() : new ObjectId(therapyDto.Id),
                Title = therapyDto.Title,
                TherapistName = therapyDto.TherapistName,
                Energy = therapyDto.Energy,
                Pulses = therapyDto.Pulses,
                BeamDiameter = therapyDto.BeamDiameter,
                StartDate = therapyDto.StartDate,
                EndDate = therapyDto.EndDate,
                AdditionalComments = therapyDto.AdditionalComments,
                TherapyAreas = therapyDto.TherapyAreas?.Select(areaDto => new TherapyArea
                {
                    AreaName = areaDto.AreaName,
                    BeamDiameter = areaDto.BeamDiameter,
                    Pulses = areaDto.Pulses,
                    Energy = areaDto.Energy,
                    Price = areaDto.Price
                }).ToList()
            };
            return therapy;
        }
        #endregion
    }

    public class TherapyWithCustomerInfoDTO
    {
        public TherapyDTO TherapyDto { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
