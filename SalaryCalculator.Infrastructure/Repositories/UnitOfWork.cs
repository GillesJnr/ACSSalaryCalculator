using SalaryCalculator.Infrastructure.Entities;
using SalaryCalculator.Infrastructure.Data;
using SalaryCalculator.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiDbContext _context;
        public UnitOfWork(ApiDbContext context)
        {
            _context = context;
            Salaries = new Repository<Salary>(_context);
            Pensions = new Repository<Pension>(_context);
            IncomeTaxRates = new Repository<IncomeTaxRate>(_context);
            Users = new Repository<User>(_context);
        }

        public IRepository<Salary> Salaries { get; private set; }

        public IRepository<Pension> Pensions { get; private set; }

        public IRepository<IncomeTaxRate> IncomeTaxRates { get; private set; }

        public IRepository<User> Users { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
