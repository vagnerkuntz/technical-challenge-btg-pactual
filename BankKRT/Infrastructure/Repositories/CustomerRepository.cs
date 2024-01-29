using Amazon.DynamoDBv2.DataModel;
using BankKRT.Domain.Entities;
using BankKRT.Domain.Repositories;
using BankKRT.Infrastructure.Persistence.Models;

namespace BankKRT.Infrastructure.Repositories
{
    public class CustomerRepository(IDynamoDBContext context) : ICustomerRepository
    {
        public async Task CreateCustomer(Customer customer)
        {
            var existingCustomer = await context.LoadAsync<CustomerDynamoDBModel>(customer.Document);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException($"Cliente com o CPF {customer.Document}, já existe.");
            }

            var customerDynamoDbModel = new CustomerDynamoDBModel
            {
                Document = customer.Document,
                NumberAgency = customer.NumberAgency,
                NumberAccount = customer.NumberAccount,
                LimitPix = customer.LimitPix
            };

            await context.SaveAsync(customerDynamoDbModel);
        }

        public async Task<IEnumerable<Customer?>> GetCustomerByDocument(string document)
        {
            try
            {
                var customerDynamoDbModel = await context.LoadAsync<CustomerDynamoDBModel>(document) ?? throw new InvalidOperationException(
                        $"Cliente com o documento {document} não encontrado.");

                var customer = new Customer
                {
                    Document = customerDynamoDbModel.Document,
                    NumberAccount = customerDynamoDbModel.NumberAccount,
                    LimitPix = customerDynamoDbModel.LimitPix
                };

                return new[] { customer };

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByNumberAccount(int numberAccount)
        {
            try
            {
                var queryConfig = new DynamoDBOperationConfig
                {
                    IndexName = "NumberAccountIndex"
                };

                var search = context.QueryAsync<CustomerDynamoDBModel>(numberAccount, queryConfig);
                var results = await search.GetRemainingAsync();

                return results.Select(result => new Customer
                {
                    Document = result.Document,
                    NumberAccount = result.NumberAccount,
                    LimitPix = result.LimitPix
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task UpdateLimitPix(string document, int numberAccount, decimal newLimit)
        {
            try
            {
                var queryConfig = new DynamoDBOperationConfig
                {
                    IndexName = "NumberAccountIndex"
                };

                var customerAccount = context.QueryAsync<CustomerDynamoDBModel>(numberAccount, queryConfig);
                var results = await customerAccount.GetRemainingAsync();

                if (results == null || results.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Cliente com o número da conta {numberAccount} não encontrado.");
                }

                foreach (var customer in results.Where(customer => customer.Document == document))
                {
                    customer.LimitPix = newLimit;
                    await context.SaveAsync(customer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task DeleteCustomerByDocument(string document)
        {
            try
            {
                var customerDynamoDbModel = await context.LoadAsync<CustomerDynamoDBModel>(document);
                if (customerDynamoDbModel != null)
                    await context.DeleteAsync(customerDynamoDbModel);
                else
                    throw new InvalidOperationException($"Cliente com o CPF {document} não encontrado.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
