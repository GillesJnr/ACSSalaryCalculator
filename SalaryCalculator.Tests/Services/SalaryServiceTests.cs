using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SalaryCalculator.Core.Interfaces;
using SalaryCalculator.Core.Models;
using SalaryCalculator.Core.Services;
using SalaryCalculator.Infrastructure.Entities;
using SalaryCalculator.Infrastructure.Interfaces;
using SalaryCalculator.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Tests.Services
{
    public class SalaryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<Salary>> _mockSalaryRepository;
        private readonly Mock<IRepository<Pension>> _mockPensionRepository;
        private readonly Mock<IRepository<IncomeTaxRate>> _mockTaxRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly SalaryService _salaryService;

        public SalaryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCacheService = new Mock<ICacheService>();
            _mockSalaryRepository = new Mock<IRepository<Salary>>();
            _mockPensionRepository = new Mock<IRepository<Pension>>();
            _mockTaxRepository = new Mock<IRepository<IncomeTaxRate>>();
            _mockUnitOfWork.Setup(uow => uow.Salaries).Returns(_mockSalaryRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.Pensions).Returns(_mockPensionRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.IncomeTaxRates).Returns(_mockTaxRepository.Object);
            _salaryService = new SalaryService(_mockUnitOfWork.Object, _mockCacheService.Object, _mockPensionRepository.Object, _mockSalaryRepository.Object, _mockTaxRepository.Object);
        }

        [Fact]
        public async Task CalculateGrossSalary_ShouldReturnExpectedResult()
        {
            var request = SalaryFixture.salaryRequestDto();
            var cacheKey = $"{request.DesiredNetSalary}_{request.TotalAllowances}";
            _mockCacheService.Setup(cs => cs.Get<SalaryResponseDto>(cacheKey)).Returns((SalaryResponseDto)null);

            // Act
            var result = await _salaryService.CalculateGrossSalaryAsync(request);

            // Assert
            result.GrossSalary.Should().BeGreaterThan(0);
            result.BasicSalary.Should().BeGreaterThan(0);
            result.TotalPAYETax.Should().BeGreaterThanOrEqualTo(0);
            result.EmployeePensionContribution.Should().BeGreaterThanOrEqualTo(0);
            result.EmployerPensionContribution.Should().BeGreaterThanOrEqualTo(0);

            // Verifying that the AddAsync method was called once
            _mockSalaryRepository.Verify(repo => repo.AddAsync(It.IsAny<Salary>()), Times.Once);

            // Verifying that the CompleteAsync method was called once
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);

            // Verifying that cache set method was called once
            _mockCacheService.Verify(mc => mc.Set(cacheKey, It.IsAny<SalaryResponseDto>(), It.IsAny<MemoryCacheEntryOptions>()), Times.Once);

        }

        [Fact]
        public async Task CalculateGrossSalaryAsync_ShouldReturnCachedResult_WhenCalledTwiceWithSameRequest()
        {
            // Arrange
            var request = SalaryFixture.salaryRequestDto();

            var cacheKey = $"{request.DesiredNetSalary}_{request.TotalAllowances}";
            var cachedResponse = SalaryFixture.salaryResponseDto();

            _mockCacheService.Setup(cs => cs.Get<SalaryResponseDto>(cacheKey)).Returns(cachedResponse);


            // Act
            var firstResult = await _salaryService.CalculateGrossSalaryAsync(request);
            var secondResult = await _salaryService.CalculateGrossSalaryAsync(request);

            // Assert
            firstResult.Should().BeEquivalentTo(secondResult);

            // Verifying that the AddAsync method was never called
            _mockSalaryRepository.Verify(repo => repo.AddAsync(It.IsAny<Salary>()), Times.Never);

            // Verifying that the CompleteAsync method was never called
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }
    }
}
