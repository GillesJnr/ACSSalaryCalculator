using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using SalaryCalculator.Core.Interfaces;
using SalaryCalculator.Core.Models;
using SalaryCalculator.Core.Services;
using SalaryCalculator.Infrastructure.Entities;
using SalaryCalculator.Infrastructure.Interfaces;

namespace SalaryCalculator.API.Controllers
{
    [ApiController]
    public class CalculatorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CalculatorController> _logger;
        private readonly IConfiguration _config;
        private readonly SalaryService _salaryService;     
        private readonly IUtilityRepository _utility;     

        public CalculatorController(IUnitOfWork unitOfWork, ICacheService cacheService,
             ILogger<CalculatorController> logger, IConfiguration config, SalaryService salaryService, IUserRepository userRepo,
             IUtilityRepository utility)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
            _config = config;
            _salaryService = salaryService;
            _userRepo = userRepo;
            _utility = utility;
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token([FromBody] LoginDto request)
        {
            string ip = await _utility.GetClientIPAddress(); //Getting the IP Address of the user making this request
            _logger.LogInformation($"========== Request initiated from {ip}========= at {DateTime.UtcNow}");            
            if (!ModelState.IsValid) return BadRequest();
            string password = await _utility.ComputeSha256(request.Password!); //computing the hash of the password which will be used to check the db
            var user = await _userRepo.Get(request.Username!, password);

            if (user == null) return Unauthorized();

            string key = _utility.GenerateJwtToken(request.Username!, out DateTime expires, out DateTime issued);
            TokenResponse response = new TokenResponse
            { 
                Token = key,
                Username = request.Username,
                Created_At = issued,
                Expires_At = expires
            };
            return Ok(response);
        }

        [Authorize]
        [Route("[controller]/")]
        [HttpPost]
        public async Task<IActionResult> Get(SalaryRequestDto request)
        {           
            _logger.LogInformation($"================request {JsonConvert.SerializeObject(request)} initiated at {DateTime.UtcNow}  ");
            if (!ModelState.IsValid) return BadRequest();
            SalaryResponseDto response = await _salaryService.CalculateGrossSalaryAsync(request);
            
            return Ok(response);
        }
    }
}
