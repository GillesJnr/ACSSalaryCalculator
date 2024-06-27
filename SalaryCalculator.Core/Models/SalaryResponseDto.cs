using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Models
{
    public class SalaryResponseDto
    {
        public double GrossSalary { get; set; }
        public double BasicSalary { get; set; }
        public double TotalPAYETax { get; set; }
        public double EmployeePensionContribution { get; set; }
        public double EmployerPensionContribution { get; set; }
    }
}
