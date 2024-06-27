using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalaryCalculator.Infrastructure.Data;
using SalaryCalculator.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApiDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApiDbContext>>()))
            {
                // Check if there is already data in the database
                if (context.Pensions.Any())
                {
                    return; // Database has been seeded
                }
                else
                {
                    // Seed pension tiers data
                    var pensionTiers = new Pension[]
                    {
                     new Pension { Id = 1, TierName = "Tier1", EmployeeRate = 0, EmployerRate = 13 },
                     new Pension { Id = 2, TierName = "Tier2", EmployeeRate = 5.5, EmployerRate = 0 },
                     new Pension { Id = 3, TierName = "Tier3", EmployeeRate = 5, EmployerRate = 5 }
                    };
                    context.Pensions.AddRange(pensionTiers);
                }

                if (context.IncomeTaxRates.Any())
                {
                    return;
                }
                else
                {
                    // Seed pension tiers data
                    var incomeTaxRates = new IncomeTaxRate[]
                    {
                     new IncomeTaxRate { Id = 1, Year = "First", ChargeableIncome = 490, Rate = 0, TaxPayable = 0, CumulativeIncome = 490, CumulativeTax = 0 },
                     new IncomeTaxRate { Id = 2, Year = "Next", ChargeableIncome = 110, Rate = 5, TaxPayable = 5.5, CumulativeIncome = 600, CumulativeTax = 5.5 },
                     new IncomeTaxRate { Id = 3, Year = "Next", ChargeableIncome = 130, Rate = 13, TaxPayable = 13.00, CumulativeIncome = 730, CumulativeTax = 18.5 },
                     new IncomeTaxRate { Id = 4, Year = "Next", ChargeableIncome = 3166.67, Rate = 17.5, TaxPayable = 554.17, CumulativeIncome = 3896.67, CumulativeTax = 572.67 },
                     new IncomeTaxRate { Id = 5, Year = "Next", ChargeableIncome = 16000, Rate = 25, TaxPayable = 4000.00, CumulativeIncome = 19896.67, CumulativeTax = 4572.67 },
                     new IncomeTaxRate { Id = 6, Year = "Next", ChargeableIncome = 30520, Rate = 30, TaxPayable = 9156.00, CumulativeIncome = 50416.67, CumulativeTax = 13728.67 },
                     new IncomeTaxRate { Id = 7, Year = "Exceeding", ChargeableIncome = 50000.00, Rate = 35, TaxPayable = null, CumulativeIncome = null, CumulativeTax = null }
                    };
                    context.IncomeTaxRates.AddRange(incomeTaxRates);
                }

                if (context.Users.Any())
                {
                    return; // Database has been seeded
                }
                else
                {
                    // Seed pension tiers data
                    var users = new User[]
                    {
                     new User { Id = 1, Username = "admin", Password = "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8", DateCreated = Convert.ToDateTime("06-26-2024") },
                    };
                    context.Users.AddRange(users);
                }

                context.SaveChanges();
            }
        }

    }
}
