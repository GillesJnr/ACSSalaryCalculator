using SalaryCalculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Tests.Fixtures
{
    public class SalaryFixture
    {
        public static List<SalaryRequestDto> salaryRequestDtos()
        {
            return new List<SalaryRequestDto>()
            {
                new SalaryRequestDto()
                {
                    DesiredNetSalary = 3000,
                    TotalAllowances =  500
                },
                new SalaryRequestDto()
                {
                    DesiredNetSalary = 12000,
                    TotalAllowances =  5000
                }
            };
        }

        public static SalaryRequestDto salaryRequestDto()
        {
            return new SalaryRequestDto()
            {
                DesiredNetSalary = 3000,
                TotalAllowances = 500
            };
        }

        public static SalaryResponseDto salaryResponseDto()
        {
            return new SalaryResponseDto()
            {
                GrossSalary = 4000,
                BasicSalary = 3500,
                TotalPAYETax = 500,
                EmployeePensionContribution = 350,
                EmployerPensionContribution = 450
            };
        }
    }
}
