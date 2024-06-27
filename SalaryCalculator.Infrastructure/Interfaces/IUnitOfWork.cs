using SalaryCalculator.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Salary> Salaries { get; }
        IRepository<Pension> Pensions { get; }
        IRepository<IncomeTaxRate> IncomeTaxRates { get; }
        IRepository<User> Users { get; }
        Task<int> CompleteAsync();
    }
}
