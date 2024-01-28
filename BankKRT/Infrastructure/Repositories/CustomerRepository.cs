using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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

        public async Task<Customer?> GetCustomerByDocument(string document)
        {
            var customerDynamoDbModel = await context.LoadAsync<CustomerDynamoDBModel>(document);
            if (customerDynamoDbModel == null)
            {
                return null;
            }

            return new Customer
            {
                Document = customerDynamoDbModel.Document,
                LimitPix = customerDynamoDbModel.LimitPix
            };
        }

        public async Task<Customer?> GetCustomerByNumberAccount(int numberAccount)
        {
            var queryConfig = new DynamoDBOperationConfig
            {
                IndexName = "NumberAccountIndex"
            };
                
            var search = context.QueryAsync<CustomerDynamoDBModel>(numberAccount, queryConfig);
            var results = await search.GetRemainingAsync();

            var firstResult = results.FirstOrDefault();
            if (firstResult == null)
            {
                return null;
            }

            return new Customer
            {
                NumberAccount = firstResult.NumberAccount,
                LimitPix = firstResult.LimitPix
            };
        }

        public async Task UpdateLimitPix(int numberAccount, decimal newLimit)
        {
            var queryConfig = new DynamoDBOperationConfig
            {
                IndexName = "NumberAccountIndex"
            };

            var customerAccount = context.QueryAsync<CustomerDynamoDBModel>(numberAccount, queryConfig);
            var results = await customerAccount.GetRemainingAsync();

            if (results == null || results.Count == 0)
            {
                throw new InvalidOperationException($"Cliente com o número da conta {numberAccount} não encontrado.");
            }

            foreach (var customer in results)
            {
                customer.LimitPix = newLimit;
                await context.SaveAsync(customer);
            }
        }

    }
}
