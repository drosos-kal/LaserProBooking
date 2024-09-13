using BeautySalonBookingSystem.Converters;
using BeautySalonBookingSystem.Models.Entities;
using BeautySalonBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BeautySalonBookingSystem.Controllers
{

    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [Route("get-customers")]
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomer()
        {
            var customerList = await _customerService.GetCustomers();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new GenderJsonConverter());

            var json = JsonConvert.SerializeObject(customerList, settings);

            return Ok(json);
        }
    }
}
