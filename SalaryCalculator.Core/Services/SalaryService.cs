using Microsoft.Extensions.Caching.Memory;
using SalaryCalculator.Core.Interfaces;
using SalaryCalculator.Core.Models;
using SalaryCalculator.Infrastructure.Entities;
using SalaryCalculator.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Services
{
    public class SalaryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Pension> _pensionRepo;
        private readonly IRepository<Salary> _salaryRepo;
        private readonly IRepository<IncomeTaxRate> _taxRepo;
        private readonly ICacheService _cacheService;


        public SalaryService(IUnitOfWork unitOfWork, ICacheService cacheService,
            IRepository<Pension> pensionRepo, IRepository<Salary> salaryRepo, IRepository<IncomeTaxRate> taxRepo)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _pensionRepo = pensionRepo;
            _salaryRepo = salaryRepo;
            _taxRepo = taxRepo;
        }
        public async Task<SalaryResponseDto> CalculateGrossSalaryAsync(SalaryRequestDto request)
        {
            string cacheKey = $"{request.DesiredNetSalary}_{request.TotalAllowances}";
            var cachedResponse = _cacheService.Get<SalaryResponseDto>(cacheKey);
            if (cachedResponse == null)
            {

                double desiredNetSalary = request.DesiredNetSalary;
                double allowances = request.TotalAllowances;

                //get the basic which is net + allowances
                double basicSalary = desiredNetSalary + allowances;

                //pensions deductions from the basic with remainder being taxable amount
                (double EmployerTierOnePayment, double EmployeeTierTwoPayment,
                    double EmployeeTierThreePayment, double EmployerTierThreePayment) = await CalculatePensionTiers(basicSalary);

                double TotalEmployerDeductions = EmployerTierOnePayment + EmployerTierThreePayment;
                double TotalEmployeeDeductions = EmployeeTierTwoPayment + EmployeeTierThreePayment;


                //taxed amounts based on taxable amount
                double TaxableIncome = basicSalary - TotalEmployeeDeductions;
                double PAYETax = await CalculatePAYETax(TaxableIncome);

                //gross salary 

                double grossSalary = basicSalary + PAYETax + allowances + TotalEmployeeDeductions;

                var salary = new Salary()
                {
                    GrossSalary = Math.Round(grossSalary, 2),
                    BasicSalary = Math.Round(basicSalary, 2),
                    TotalPAYETax = Math.Round(PAYETax, 2),
                    EmployeePensionContribution = Math.Round(TotalEmployeeDeductions, 2),
                    EmployerPensionContribution = Math.Round(TotalEmployerDeductions, 2)
                };

                await _unitOfWork.Salaries.AddAsync(salary);
                await _unitOfWork.CompleteAsync();



                var response = new SalaryResponseDto
                {
                    GrossSalary = Math.Round(grossSalary, 2) ,
                    BasicSalary = Math.Round(basicSalary, 2),
                    TotalPAYETax = Math.Round(PAYETax, 2),
                    EmployeePensionContribution = Math.Round(TotalEmployeeDeductions, 2),
                    EmployerPensionContribution = Math.Round(TotalEmployerDeductions, 2)
                };

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };
             
                _cacheService.Set(cacheKey, response, cacheOptions);

                return response;
            }

            return cachedResponse;
        }

        private async Task<double> CalculatePAYETax(double taxableIncome)
        {
            string cacheKey = $"taxableIncome_{taxableIncome}";

            List<IncomeTaxRate> incomeTaxRates = _cacheService.Get<List<IncomeTaxRate>>(cacheKey);
            if (incomeTaxRates == null)
            {
                IEnumerable<IncomeTaxRate> incomeTaxEnumerable = await _unitOfWork.IncomeTaxRates.GetAllAsync();
                incomeTaxRates = incomeTaxEnumerable.ToList();

                if (incomeTaxRates == null) return 0;
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                _cacheService.Set(cacheKey, incomeTaxRates, cacheOptions);
            }

            if (incomeTaxRates.Count == 0) return 0;

            for (int i = 0; i < incomeTaxRates.Count; i++)
            {
                if (taxableIncome > incomeTaxRates[i].CumulativeIncome && taxableIncome < incomeTaxRates[i + 1].CumulativeIncome)
                {
                    //this checks for especially the last salary bound which is not capped.
                    double cumulativeIncome = incomeTaxRates[i].CumulativeIncome!.Value == null ? incomeTaxRates[i - 1].CumulativeIncome!.Value : incomeTaxRates[i].CumulativeIncome!.Value;
                    double cumulativeTax = incomeTaxRates[i].CumulativeTax!.Value == null ? incomeTaxRates[i - 1].CumulativeTax!.Value : incomeTaxRates[i].CumulativeTax!.Value;
                    double taxPayable = incomeTaxRates[i].TaxPayable!.Value == null ? incomeTaxRates[i - 1].TaxPayable!.Value : incomeTaxRates[i].TaxPayable!.Value;
                    double rate = incomeTaxRates[i].Rate!.Value;

                    double tax = (rate / 100) * (taxableIncome - cumulativeIncome);
                    double totalTax = tax + cumulativeTax;
                    return totalTax;
                }
            }

            return 0;
            
        }

        private async Task<(double, double, double, double)> CalculatePensionTiers(double basicSalary)
        {
            string cacheKey = $"basicSalary_{basicSalary}";
            List<Pension> pensionTiers = _cacheService.Get<List<Pension>>(cacheKey);
            if (pensionTiers == null)
            {
                IEnumerable<Pension> pensionEnumerable = await _unitOfWork.Pensions.GetAllAsync();
                pensionTiers = pensionEnumerable.ToList();
                if (pensionTiers == null) return (0, 0, 0, 0);
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };

                _cacheService.Set(cacheKey, pensionTiers, cacheOptions);
            }

            if (pensionTiers.Count == 0) return (0, 0, 0, 0);  

            double EmployerTierOnePayment = (pensionTiers.FirstOrDefault(s => s.TierName == "Tier1").EmployerRate!.Value / 100) * basicSalary;
            double EmployeeTierTwoPayment = (pensionTiers.FirstOrDefault(s => s.TierName == "Tier2").EmployeeRate!.Value / 100) * basicSalary;
            double EmployeeTierThreePayment = (pensionTiers.FirstOrDefault(s => s.TierName == "Tier3").EmployeeRate!.Value / 100) * basicSalary;
            double EmployerTierThreePayment = (pensionTiers.FirstOrDefault(s => s.TierName == "Tier3").EmployerRate!.Value / 100) * basicSalary;

            return (EmployerTierOnePayment, EmployeeTierTwoPayment, EmployeeTierThreePayment, EmployerTierThreePayment);
        }
    }
}
