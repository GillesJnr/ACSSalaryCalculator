using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Models
{
    public class SalaryRequestDto
    {
        [Required]
        public double DesiredNetSalary { get; set; }
        [Required]
        public double TotalAllowances { get; set; }
    }
}
