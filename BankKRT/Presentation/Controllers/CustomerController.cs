using BankKRT.Application.Services;
using BankKRT.Domain.Entities;
using BankKRT.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankKRT.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CustomerController(CustomerService customerService) : ControllerBase
    {
        [HttpPost("Create")]
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

        [HttpGet("GetByDocument/{document}")]
        public async Task<IActionResult> GetByDocument(string document)
        {
            var customer = await customerService.GetCustomerByDocument(document);
            if (customer == null)
            {
                return NotFound();
            }

            var responseDto = new CustomerResponseByDocumentDTO
            {
                Document = customer.Document,
                LimitPix = customer.LimitPix
            };

            return Ok(responseDto);
        }

        [HttpGet("GetByNumberAccount/{numberAccount:int}")]
        public async Task<IActionResult> GetByNumberAccount(int numberAccount)
        {
            try
            {
                var customer = await customerService.GetCustomerByNumberAccountAsync(numberAccount);
                if (customer == null)
                {
                    return NotFound();
                }

                var responseDto = new CustomerResponseByNumberAccountDTO
                {
                    NumberAccount = customer.NumberAccount,
                    LimitPix = customer.LimitPix
                };

                return Ok(responseDto);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-pix-limit")]
        public async Task<IActionResult> UpdatePixLimit(int numberAccount, decimal newLimit)
        {
            try
            {
                await customerService.UpdatePixLimitAsync(numberAccount, newLimit);
                return Ok("Limite PIX atualizado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCustomer/{document}")]
        public async Task<IActionResult> DeleteCustomerByDocument(string document)
        {
            try
            {
                await customerService.DeleteCustomerByDocument(document);
                return Ok("Cliente apagado com sucesso.");
            } catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
