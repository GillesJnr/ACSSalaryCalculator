using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Entities
{
    public class Pension
    {
        [Key]
        public int Id { get; set; }
        public string? TierName { get; set; }
        public double? EmployeeRate { get; set; }
        public double? EmployerRate { get; set;}
    }
}
