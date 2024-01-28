using BankKRT.Domain.Entities;

namespace BankKRT.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task CreateCustomer(Customer customer);
        Task<Customer?> GetCustomerByDocument(string document);
        Task<Customer?> GetCustomerByNumberAccount(int numberAccount);
        Task UpdateLimitPix(int numberAccount, decimal newLimit);
    }
}
