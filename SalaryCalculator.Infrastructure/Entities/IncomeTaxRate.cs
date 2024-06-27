using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Entities
{
    public class IncomeTaxRate
    {
        [Key]
        public int Id { get; set; }
        public string? Year { get; set; }
        public double? ChargeableIncome { get; set; }
        public double? Rate { get; set; }
        public double? TaxPayable { get; set; }
        public double? CumulativeIncome { get; set; }
        public double? CumulativeTax { get; set; }
    }
}
