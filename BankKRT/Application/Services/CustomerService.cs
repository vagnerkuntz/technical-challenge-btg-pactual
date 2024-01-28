using BankKRT.Domain.Entities;
using BankKRT.Domain.Repositories;
using BankKRT.Domain.Validation;

namespace BankKRT.Application.Services
{
    public class CustomerService(ICustomerRepository customerRepository)
    {
        private string FormatCPF(string cpf)
        {
            return cpf.Replace(" ", "").Replace("-", "").Replace(".", "");
        }

        public async Task CreateCustomer(Customer customer)
        {
            if (!CpfValidator.Validate(customer.Document))
            {
                throw new ArgumentException("CPF inválido. Por favor, verifique o número do CPF e tente novamente.");
            }

            if (customer.NumberAgency <= 0) throw new ArgumentException("O número da agência deve ser um valor positivo maior que zero.");
            if (customer.NumberAccount <= 0) throw new ArgumentException("O número da conta deve ser um valor positivo maior que zero.");
            if (customer.LimitPix < 0) throw new ArgumentException("O limite para transações não pode ser menor que 0");

            customer.Document = FormatCPF(customer.Document);
            await customerRepository.CreateCustomer(customer);
        }

        public async Task<Customer?> GetCustomerByDocument(string document)
        {
            document = FormatCPF(document);
            return await customerRepository.GetCustomerByDocument(document);
        }

        public async Task<Customer?> GetCustomerByNumberAccountAsync(int numberAccount)
        {
            if (numberAccount <= 0)
            {
                throw new ArgumentException("O número da conta deve ser um valor positivo maior que zero.");
            }

            return await customerRepository.GetCustomerByNumberAccount(numberAccount);
        }

        public async Task UpdatePixLimitAsync(int numberAccount, decimal newLimit)
        {
            if (numberAccount <= 0)
            {
                throw new ArgumentException("O número da conta deve ser um valor positivo maior que zero.");
            }

            await customerRepository.UpdateLimitPix(numberAccount, newLimit);
        }
    }
}
