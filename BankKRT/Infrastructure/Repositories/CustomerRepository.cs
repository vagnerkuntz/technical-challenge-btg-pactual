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
    }
}
