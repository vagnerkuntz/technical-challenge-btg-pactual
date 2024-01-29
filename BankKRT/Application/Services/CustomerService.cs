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

        private async Task<Customer?> GetCustomerByDocumentAndNumberAccount(string document, int numberAccount)
        {
            var customers = await customerRepository.GetCustomersByNumberAccount(numberAccount);
            return customers.FirstOrDefault(c => c.Document == document);
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

            try
            {
                customer.Document = FormatCPF(customer.Document);
                await customerRepository.CreateCustomer(customer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomerByDocument(string document)
        {
            if (!CpfValidator.Validate(document))
            {
                throw new ArgumentException("CPF inválido. Por favor, verifique o número do CPF e tente novamente.");
            }

            try
            {
                document = FormatCPF(document);
                var customer = await customerRepository.GetCustomerByDocument(document);
                return customer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByNumberAccount(int numberAccount)
        {
            if (numberAccount <= 0)
            {
                throw new ArgumentException("O número da conta deve ser um valor positivo maior que zero.");
            }

            try
            {
                var customers = await customerRepository.GetCustomersByNumberAccount(numberAccount);
                return customers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task UpdatePixLimitAsync(string document, int numberAccount, decimal newLimit)
        {
            if (numberAccount <= 0)
            {
                throw new ArgumentException("O número da conta deve ser um valor positivo maior que zero.");
            }

            if (newLimit < 0)
            {
                throw new ArgumentException("O novo limite não pode ser menor que zero.");
            }

            try
            {
                var customer = await GetCustomerByDocument(document);
                var firstCustomer = customer.FirstOrDefault();

                if (firstCustomer != null)
                {
                    if (firstCustomer.Document != document || firstCustomer.NumberAccount != numberAccount)
                    {
                        throw new ArgumentException("Informações incompatíveis!");
                    }

                    await customerRepository.UpdateLimitPix(document, numberAccount, newLimit);
                }
                else
                {
                    throw new ArgumentException("Cliente não encontrado.");
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
            if (!CpfValidator.Validate(document))
            {
                throw new ArgumentException("CPF inválido. Por favor, verifique o número do CPF e tente novamente.");
            }

            try
            {
                document = FormatCPF(document);
                var customer = await GetCustomerByDocument(document);
                _ = customer.FirstOrDefault() ?? throw new InvalidOperationException($"Cliente com o CPF {document} não encontrado.");
                await customerRepository.DeleteCustomerByDocument(document);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<bool> UpdateTargetAccountLimit(string document, int destinationAccount, decimal transactionValue)
        {
            
            try
            {
                var customer = await customerRepository.GetCustomersByNumberAccount(destinationAccount) ?? throw new InvalidOperationException("Conta não encontrada.");
                var firstCustomer = customer.FirstOrDefault();

                if (firstCustomer == null)
                {
                    throw new InvalidOperationException("Conta de destino não encontrada!");
                }

                var currentLimit = firstCustomer.LimitPix += transactionValue;
                await UpdatePixLimitAsync(document, destinationAccount, currentLimit);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<decimal> ConsumeLimitForTransaction(string document, int numberAccount, decimal transactionValue)
        {
            var customer = await GetCustomerByDocumentAndNumberAccount(document, numberAccount) ?? throw new ArgumentException("Conta não encontrada.");
            if (customer.LimitPix < transactionValue)
            {
                throw new InvalidOperationException("Saldo da conta insuficiente.");
            }

            try
            {
                customer.LimitPix -= transactionValue;
                await UpdatePixLimitAsync(customer.Document, customer.NumberAccount, customer.LimitPix);
                return customer.LimitPix;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



    }
}
