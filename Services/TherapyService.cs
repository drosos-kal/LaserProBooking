using BeautySalonBookingSystem.Data;
using BeautySalonBookingSystem.Models.DTO;
using BeautySalonBookingSystem.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalonBookingSystem.Services
{
    // Not used
    public class TherapyService
    {
       /* private readonly IMongoCollection<Therapy> _therapyCollection;

        public TherapyService(IDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _therapyCollection = database.GetCollection<Therapy>("Therapies");
        }

        public async Task<List<TherapyDTO>> GetTherapiesAsync()
        {
            var therapies = await _therapyCollection.Find(x => true).ToListAsync();
            var therapiesDto = new List<TherapyDTO>();
            foreach (var therapy in therapies)
            {
                var therapyDto = MapToDTO(therapy);
                therapiesDto.Add(therapyDto);
            }
            return therapiesDto;
        }

        *//*public async Task<TherapyDTO> GetTherapyAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            var idToSearch = new ObjectId(id);
            var filter = Builders<Therapy>.Filter.Eq("_id", idToSearch);
            var therapy = await _therapyCollection.Find(filter).FirstOrDefaultAsync();

            if (therapy == null) { return null; }
            var therapyToReturn = MapToDTO(therapy);
            return therapyToReturn;
        }*//*

        public async Task CreateTherapyAsync(TherapyDTO therapy)
        {
            //DateTime localStartDate = DateTime.SpecifyKind(therapy.StartDate, DateTimeKind.Local);
            //DateTime utcStartDate = TimeZoneInfo.ConvertTimeToUtc(localStartDate, TimeZoneInfo.Local);

            var therapyToCreate = MapToEntity(therapy);
            await _therapyCollection.InsertOneAsync(therapyToCreate);
        }

        public async Task DeleteTherapyAsync(string id)
        {
            var idToSearch = new ObjectId(id);
            var filter = Builders<Therapy>.Filter.Eq("_id", idToSearch);
            var result = await _therapyCollection.DeleteOneAsync(filter);
        }

        public async Task UpdateTherapyAsync(TherapyDTO therapyDto)
        {
            DateTime localStartDate = DateTime.SpecifyKind(therapyDto.StartDate, DateTimeKind.Local);
            DateTime utcStartDate = TimeZoneInfo.ConvertTimeToUtc(localStartDate, TimeZoneInfo.Local);

            if (therapyDto == null) { return; }
            var filter = Builders<Therapy>.Filter.Eq("_id", new ObjectId(therapyDto.Id));
            var update = Builders<Therapy>.Update
                .Set(t => t.Title, therapyDto.Title)
                .Set(t => t.Energy, therapyDto.Energy)
                .Set(t => t.Pulses, therapyDto.Pulses)
                .Set(t => t.BeamDiameter, therapyDto.BeamDiameter)
                .Set(t => t.StartDate, utcStartDate)
                .Set(t => t.EndDate, localStartDate.AddMinutes(30))
                .Set(t => t.AdditionalComments, therapyDto.AdditionalComments);

            var result = await _therapyCollection.UpdateOneAsync(filter, update);
        }

        private TherapyDTO MapToDTO(Therapy therapy)
        {
            if (therapy == null) { return null; }
            var therapyDto = new TherapyDTO
            {

                Id = therapy.Id.ToString(),
                Title = therapy.Title,
                Energy = therapy.Energy,
                Pulses = therapy.Pulses,
                BeamDiameter = therapy.BeamDiameter,
                StartDate = therapy.StartDate.ToLocalTime(),
                EndDate = therapy.EndDate.ToLocalTime().AddMinutes(30),
                AdditionalComments = therapy.AdditionalComments
            };
            return therapyDto;
        }

        private Therapy MapToEntity(TherapyDTO therapyDto)
        {
            if (therapyDto == null) { return null; }
            var therapy = new Therapy
            {
                Id = string.IsNullOrEmpty(therapyDto.Id) ? ObjectId.GenerateNewId() : new ObjectId(therapyDto.Id),
                Title = therapyDto.Title,
                Energy = therapyDto.Energy,
                Pulses = therapyDto.Pulses,
                BeamDiameter = therapyDto.BeamDiameter,
                StartDate = therapyDto.StartDate,
                EndDate = therapyDto.EndDate,
                AdditionalComments = therapyDto.AdditionalComments
            };
            return therapy;
        }*/
    }
}