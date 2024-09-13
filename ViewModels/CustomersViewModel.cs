using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using BeautySalonBookingSystem.Models.Entities;
using BeautySalonBookingSystem.Services;
using BeautySalonBookingSystem.Models.DTO;

namespace BeautySalonBookingSystem.ViewModels
{

    public class CustomersViewModel : MasterPageViewModel
    {
        public CustomerDTO ProjectedCustomer { get; set; } = new CustomerDTO();
        public List<TherapyDTO> ProjectedTherapyList { get; set; } = new List<TherapyDTO>();
        public string IdToPopulate { get; set; } = "";
        public List<GenderEnumProjection> ComboBoxGenders { get; set; } = new List<GenderEnumProjection>();

        private readonly CustomerService _customerService;
        public CustomersViewModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public override Task Init()
        {
            Context.Authorize();
            if (!Context.IsPostBack)
            {
                ComboBoxGenders = GenderEnumProjection.GetGenderList();
            }
            return base.Init();
        }

        public async Task DeleteCustomer()
        {
            if (string.IsNullOrEmpty(IdToPopulate)) { return; }
            await _customerService.DeleteCustomerAsync(IdToPopulate);
            Context.RedirectToRoute("Customers");
        }

        public async Task CreateCustomer()
        {
            /*if (ProjectedCustomer.Age.Value.Year < 1900 || ProjectedCustomer.Age.Value.Year > DateTime.Now.Year)
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
                Therapies = ProjectedCustomer.Therapies
            };

            await _customerService.CreateCustomerAsync(newCustomer);
            Context.RedirectToRoute("Customers");
        }

        public async Task EditCustomer()
        {
            if (string.IsNullOrEmpty(IdToPopulate)) { return; }
            var customerFromDb = await _customerService.GetCustomer(IdToPopulate);
            if (customerFromDb != null)
            {
                customerFromDb.Firstname = ProjectedCustomer.Firstname;
                customerFromDb.Lastname = ProjectedCustomer.Lastname;
                customerFromDb.Gender = ProjectedCustomer.Gender;
                customerFromDb.Age = ProjectedCustomer.Age;
                customerFromDb.MobileNumber = ProjectedCustomer.MobileNumber;
                customerFromDb.Email = ProjectedCustomer.Email;
                customerFromDb.Address = ProjectedCustomer.Address;
                customerFromDb.City = ProjectedCustomer.City;
                customerFromDb.PostalCode = ProjectedCustomer.PostalCode;
                customerFromDb.AdditionalComments = ProjectedCustomer.AdditionalComments;
                customerFromDb.Medication = ProjectedCustomer.Medication;
                customerFromDb.Therapies = ProjectedCustomer.Therapies;
            }

           await _customerService.UpdateCustomer(customerFromDb);
        }

        public async Task PopulateModal()
        {
            if (string.IsNullOrEmpty(IdToPopulate)) { return; }
            var customerFromDb = await _customerService.GetCustomer(IdToPopulate);
            if (customerFromDb != null)
            {
                ProjectedCustomer.Firstname = customerFromDb.Firstname;
                ProjectedCustomer.Lastname = customerFromDb.Lastname;
                ProjectedCustomer.Gender = customerFromDb.Gender;
                ProjectedCustomer.Age = customerFromDb.Age;
                ProjectedCustomer.MobileNumber = customerFromDb.MobileNumber;
                ProjectedCustomer.Email = customerFromDb.Email;
                ProjectedCustomer.Address = customerFromDb.Address;
                ProjectedCustomer.City = customerFromDb.City;
                ProjectedCustomer.PostalCode = customerFromDb.PostalCode;
                ProjectedCustomer.AdditionalComments = customerFromDb.AdditionalComments;
                ProjectedCustomer.Medication = customerFromDb.Medication;
                ProjectedCustomer.Therapies = customerFromDb.Therapies;
                Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#editModal').modal('toggle')");
            }
        }

        public void ClearForm()
        {
            ProjectedCustomer = new CustomerDTO();
        }


    }
}

