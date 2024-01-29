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
            try
            {
                var customer = await customerService.GetCustomerByDocument(document);
                if (!customer.Any())
                {
                    return NotFound();
                }

                var responseDtoArray = customer.Select(customer => new CustomerResponseByDocumentDTO
                {
                    Document = customer.Document,
                    LimitPix = customer.LimitPix
                }).ToArray();

                return Ok(responseDtoArray);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByNumberAccount/{numberAccount:int}")]
        public async Task<IActionResult> GetByNumberAccount(int numberAccount)
        {
            try
            {
                var customers = await customerService.GetCustomersByNumberAccount(numberAccount);
                if (!customers.Any())
                {
                    return NotFound("Nenhum cliente encontrado para a conta especificada.");
                }

                var responseDtoArray = customers.Select(customer => new CustomerResponseByNumberAccountDTO
                {
                    Document = customer.Document,
                    NumberAccount = customer.NumberAccount,
                    LimitPix = customer.LimitPix
                }).ToArray();

                return Ok(responseDtoArray);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdatePixLimit")]
        public async Task<IActionResult> UpdatePixLimit([FromBody] CustomerResponseByNumberAccountDTO request)
        {
            try
            {
                await customerService.UpdatePixLimitAsync(request.Document, request.NumberAccount, request.LimitPix);
                return Ok("Limite PIX atualizado com sucesso.");
            }
            catch (System.Exception ex)
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

        [HttpPost("StartPixTransaction")]
        public async Task<IActionResult> StartPixTransaction([FromBody] CustomerTransactionDTO request)
        {
            try
            {
                if (request.DocumentSourceAccount == request.DocumentDestinationAccount || request.SourceAccount == request.DestinationAccount)
                {
                    return BadRequest("Você não pode realizar uma transferência para a mesma conta.");
                }

                var sourceAccount = await customerService.GetCustomersByNumberAccount(request.SourceAccount);
                if (!sourceAccount.Any())
                {
                    return BadRequest("Conta de origem não encontrada.");
                }

                var destinationAccount = await customerService.GetCustomersByNumberAccount(request.DestinationAccount);
                if (!destinationAccount.Any())
                {
                    return BadRequest("Conta de destino não encontrada.");
                }

                if (request.TransactionValue == 0)
                {
                    return BadRequest("O valor transacionado precisa ser maior que zero");
                }

                var newLimit =
                    await customerService.ConsumeLimitForTransaction(request.DocumentSourceAccount, request.SourceAccount, request.TransactionValue);
                await customerService.UpdateTargetAccountLimit(request.DocumentDestinationAccount, request.DestinationAccount, request.TransactionValue);

                return Ok(new { Status = true, Message = "Transação PIX aprovada.", CurrentLimitSourceAccount = newLimit });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
