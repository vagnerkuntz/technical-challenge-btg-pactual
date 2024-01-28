﻿using BankKRT.Domain.Entities;

namespace BankKRT.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task CreateCustomer(Customer customer);
    }
}