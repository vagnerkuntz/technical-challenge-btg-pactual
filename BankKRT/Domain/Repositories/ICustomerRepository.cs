using BankKRT.Domain.Entities;

namespace BankKRT.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task CreateCustomer(Customer customer);
        Task<IEnumerable<Customer?>> GetCustomerByDocument(string document);
        Task<IEnumerable<Customer>> GetCustomersByNumberAccount(int numberAccount);
        Task UpdateLimitPix(string document, int numberAccount, decimal newLimit);
        Task DeleteCustomerByDocument(string document);
    }
}
