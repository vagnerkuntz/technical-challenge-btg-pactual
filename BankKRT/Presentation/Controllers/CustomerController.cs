using BankKRT.Application.Services;
using BankKRT.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BankKRT.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CustomerController(CustomerService customerService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                await customerService.CreateCustomer(customer);
                return Ok("Cliente criado com sucesso.");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
