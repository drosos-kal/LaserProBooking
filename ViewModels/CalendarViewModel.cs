using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BeautySalonBookingSystem.Models.DTO;
using BeautySalonBookingSystem.Models.Entities;
using BeautySalonBookingSystem.Services;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;
using MongoDB.Bson;
using MongoDB.Driver;
using static DotVVM.Framework.Compilation.Javascript.MethodFindingHelper.Generic;
using Enum = System.Enum;

namespace BeautySalonBookingSystem.ViewModels
{
    public class CalendarViewModel : MasterPageViewModel
    {
        public TherapyDTO ProjectedTherapy { get; set; } = new TherapyDTO();
        public Customer ProjectedCustomer { get; set; } = new Customer();
        public List<string> AppointmentHours { get; set; } = new List<string>();
        public string AppointmentTime { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TherapyIdToPopulate { get; set; } = "";
        public string CustomerIdToAppoint { get; set; } = "";
        public string CustomerToAppoint { get; set; }
        public List<GenderEnumProjection> ComboBoxGenders { get; set; } = new List<GenderEnumProjection>();
        public List<string> TherapistList { get; set; } = new List<string> { "Ηλιάνα", "Μελίνα" };
        public List<string> TherapyTypes { get; set; }
        public List<TherapyArea> TherapyTypeBuilderList { get; set; } = new List<TherapyArea>();

        // DI
        private readonly CustomerService _customerService;

        public CalendarViewModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public override Task Init()
        {
            Context.Authorize();
            if (!Context.IsPostBack)
            {
                TherapyTypes = new List<string>
                {
                    "Μουστάκι", "Πρόσωπο", "Μασχάλες", "Στήθος", "Χέρια", "Κοιλιά", "Πλάτη", "Ώμοι", "Μπικίνι", "Γλουτοί", "Κνήμες", "Γάμπες", "Πόδια", "Αυτιά", "Αυχένας", "Μούσι", "Full Body"
                }.OrderBy(x => x).ToList();
                ComboBoxGenders = GenderEnumProjection.GetGenderList();
                WorkingHours();
            }
            return base.Init();
        }

        #region Customer
        public async Task CreateCustomer()
        {
            /*if (ProjectedCustomer.Age == null || ProjectedCustomer.Age.Value.Year < 1900 || ProjectedCustomer.Age.Value.Year > DateTime.Now.Year)
            {
                ProjectedCustomer.Age = null;
            }*/
            var newCustomer = new CustomerDTO
            {
                Firstname = ProjectedCustomer.Firstname,
                Lastname = ProjectedCustomer.Lastname,
                Gender = ProjectedCustomer.Gender,
                Age = ProjectedCustomer.Age,
                MobileNumber = ProjectedCustomer.MobileNumber,
                Email = ProjectedCustomer.Email,
                Address = ProjectedCustomer.Address,
                City = ProjectedCustomer.City,
                PostalCode = ProjectedCustomer.PostalCode,
                Medication = ProjectedCustomer.Medication,
                Therapies = new List<TherapyDTO>()
            };

            var insertedCustomer = await _customerService.CreateCustomerAsync(newCustomer);

            CustomerToAppoint = $"{newCustomer.Firstname} {newCustomer.Lastname}";
            CustomerIdToAppoint = insertedCustomer.Id.ToString();
            Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#customerToAppoint').show()");
            Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#selectClientBtn').hide()");
            Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#addCustomerModal').modal('toggle')");
        }

        public async Task SelectExistingCustomer()
        {
            var customerFromDb = await _customerService.GetCustomer(CustomerIdToAppoint);
            if (customerFromDb == null) { return; }
            CustomerToAppoint = $"{customerFromDb.Firstname} {customerFromDb.Lastname}";
            Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#customerToAppoint').show()");
            Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#selectClientBtn').hide()");
        }

        #endregion Customer

        #region Therapy

        public void TherapyTypeBuilder(string type, bool isAdded)
        {
            if (isAdded)
            {
                TherapyTypes.Remove(type);
                ProjectedTherapy.TherapyAreas.Add(new TherapyAreaDTO
                {
                    AreaName = type
                });
            }
            else
            {
                TherapyTypes.Add(type);
                var areaToRemove = ProjectedTherapy.TherapyAreas.Where(x => x.AreaName.Equals(type)).FirstOrDefault();
                ProjectedTherapy.TherapyAreas.Remove(areaToRemove);
                TherapyTypes = TherapyTypes.OrderBy(x => x).ToList();
            }
            ProjectedTherapy.Title = string.Join(", ", ProjectedTherapy.TherapyAreas);

        }

        public async Task CreateTherapy()
        {
            var customerToAssign = await _customerService.GetCustomer(CustomerIdToAppoint);
            if (customerToAssign == null) { return; }
            if (customerToAssign.Therapies == null)
            {
                customerToAssign.Therapies = new List<TherapyDTO>();
            }

            customerToAssign.Therapies.Add(ProjectedTherapy);
            await _customerService.UpdateCustomer(customerToAssign);
            Context.RedirectToRoute("Calendar");
        }

        public async Task EditTherapy()
        {
            if (string.IsNullOrEmpty(TherapyIdToPopulate)) { return; }
            var dictionaryFromDb = await _customerService.GetTherapyAsync(TherapyIdToPopulate);
            if (dictionaryFromDb == null) { return; }

            var customerId = dictionaryFromDb.Keys.FirstOrDefault();

            var therapyFromDb = dictionaryFromDb.Values.FirstOrDefault();
            therapyFromDb.Title = ProjectedTherapy.Title;
            therapyFromDb.TherapistName = ProjectedTherapy.TherapistName;
            therapyFromDb.Energy = ProjectedTherapy.Energy;
            therapyFromDb.Pulses = ProjectedTherapy.Pulses;
            therapyFromDb.BeamDiameter = ProjectedTherapy.BeamDiameter;
            therapyFromDb.StartDate = ProjectedTherapy.StartDate;
            therapyFromDb.AdditionalComments = ProjectedTherapy.AdditionalComments;
            therapyFromDb.TherapyAreas = ProjectedTherapy.TherapyAreas;

            await _customerService.UpdateTherapyAsync(customerId, therapyFromDb);
            Context.RedirectToRoute("Calendar");
        }

        public async Task DeleteTherapy()
        {
            if (string.IsNullOrEmpty(TherapyIdToPopulate)) { return; }
            await _customerService.DeleteTherapyAsync(CustomerIdToAppoint, TherapyIdToPopulate);
            Context.RedirectToRoute("Calendar");
        }

        #endregion Therapy

        #region HTML Functionality

        public async Task PopulateEditTherapyForm()
        {
            if (string.IsNullOrEmpty(TherapyIdToPopulate)) { return; }
            var dictionaryFromDb = await _customerService.GetTherapyAsync(TherapyIdToPopulate);

            if (dictionaryFromDb == null) { return; }

            var customerId = dictionaryFromDb.Keys.FirstOrDefault();

            var customerFromDb = await _customerService.GetCustomer(customerId);
            CustomerToAppoint = $"{customerFromDb.Firstname} {customerFromDb.Lastname}";

            var therapyFromDictionary = dictionaryFromDb.Values.FirstOrDefault();
            ProjectedTherapy.Title = therapyFromDictionary.Title;
            if (therapyFromDictionary.TherapyAreas != null)
            {
                    ProjectedTherapy.TherapyAreas = therapyFromDictionary.TherapyAreas
                .Select(dto => new TherapyAreaDTO
                {
                    AreaName = dto.AreaName,
                    BeamDiameter = dto.BeamDiameter
                }).ToList();
            }
            TherapyTypes = new List<string>
            {
                "Μουστάκι", "Πρόσωπο", "Μασχάλες", "Στήθος", "Χέρια", "Κοιλιά", "Πλάτη", "Ώμοι", "Μπικίνι", "Γλουτοί", "Κνήμες", "Γάμπες", "Πόδια", "Αυτιά", "Αυχένας", "Μούσι", "Full Body"
            }
            .OrderBy(x => x)
            .Except(ProjectedTherapy.TherapyAreas.Select(t => t.AreaName))
            .ToList();
            ProjectedTherapy.TherapistName = therapyFromDictionary.TherapistName;
            ProjectedTherapy.Energy = therapyFromDictionary.Energy;
            ProjectedTherapy.Pulses = therapyFromDictionary.Pulses;
            ProjectedTherapy.BeamDiameter = therapyFromDictionary.BeamDiameter;
            ProjectedTherapy.AdditionalComments = therapyFromDictionary.AdditionalComments;
            ProjectedTherapy.StartDate = therapyFromDictionary.StartDate;
            AppointmentDate = therapyFromDictionary.StartDate;
            AppointmentTime = therapyFromDictionary.StartDate.ToString("HH:mm");
            CustomerIdToAppoint = customerId;
        }

        public void ClearForm()
        {
            TherapyTypeBuilderList = new List<TherapyArea>();
            ProjectedTherapy = new TherapyDTO();
            ProjectedCustomer = new Customer();
            AppointmentTime = "";
            TherapyTypes = new List<string>
                {
                    "Μουστάκι", "Πρόσωπο", "Μασχάλες", "Στήθος", "Χέρια", "Κοιλιά", "Πλάτη", "Ώμοι", "Μπικίνι", "Γλουτοί", "Κνήμες", "Γάμπες", "Πόδια", "Αυτιά", "Αυχένας", "Μούσι", "Full Body"
                }.OrderBy(x => x).ToList();
        }

        public void SetAppointmentTime()
        {
            if (string.IsNullOrEmpty(AppointmentTime)) { return; }
            ProjectedTherapy.StartDate = AppointmentDate;
            DateTime parsedTime = DateTime.ParseExact(AppointmentTime, "HH:mm", CultureInfo.InvariantCulture);

            ProjectedTherapy.StartDate = ProjectedTherapy.StartDate.Date
            .AddHours(parsedTime.Hour)
            .AddMinutes(parsedTime.Minute);

            ProjectedTherapy.EndDate = ProjectedTherapy.StartDate.AddMinutes(30);
        }

        public void WorkingHours()
        {
            DateTime startTime = DateTime.Today.AddHours(11);
            DateTime endTime = DateTime.Today.AddHours(22);
            TimeSpan interval = TimeSpan.FromMinutes(30);

            while (startTime <= endTime)
            {
                AppointmentHours.Add(startTime.ToString("HH:mm"));
                startTime = startTime.Add(interval);
            }
        }

        #endregion HTML Functionality


    }
}