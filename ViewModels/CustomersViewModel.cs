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
            if (ProjectedCustomer is null) { return; }
            await _customerService.CreateCustomerAsync(ProjectedCustomer);
            Context.RedirectToRoute("Customers");
        }

        public async Task EditCustomer()
        {
            if (string.IsNullOrEmpty(IdToPopulate)) { return; }
            var customerFromDb = await _customerService.GetCustomer(IdToPopulate);
            if (customerFromDb != null)
            {
                customerFromDb = ProjectedCustomer;
            }

           await _customerService.UpdateCustomer(customerFromDb);
        }

        public async Task PopulateModal()
        {
            if (string.IsNullOrEmpty(IdToPopulate)) { return; }
            ProjectedCustomer = await _customerService.GetCustomer(IdToPopulate);
            if (ProjectedCustomer != null)
            {
                Context.ResourceManager.AddStartupScript(Guid.NewGuid().ToString(), "$('#editModal').modal('toggle')");
            }
        }

        public void ClearForm()
        {
            ProjectedCustomer = new CustomerDTO();
        }


    }
}

