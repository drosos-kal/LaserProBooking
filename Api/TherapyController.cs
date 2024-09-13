using BeautySalonBookingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BeautySalonBookingSystem.Api
{
    [Route("api/therapies")]
    [ApiController]
    public class TherapyController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public TherapyController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [Route("get-therapies")]
        [HttpGet("therapies")]
        public async Task<IActionResult> GetTherapies()
        {
            var therapiesList = await _customerService.GetTherapiesWithCustomerInfoAsync();
            return Ok(therapiesList);
        }
    }
}