using Microsoft.EntityFrameworkCore;
using SalaryCalculator.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SalaryCalculator.Infrastructure.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {                
        }

        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Pension> Pensions { get; set; }
        public DbSet<IncomeTaxRate> IncomeTaxRates { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
