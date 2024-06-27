using SalaryCalculator.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SalaryCalculator.Infrastructure.Entities;

namespace SalaryCalculator.Infrastructure.Repositories
{
    public class UtilityRepository : IUtilityRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UtilityRepository> _logger;
        private readonly IConfiguration _config;

        public UtilityRepository(IHttpContextAccessor httpContextAccessor, ILogger<UtilityRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }
        public async Task<string> ComputeSha256(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString().ToUpper();
            }
        }

        public async Task<string> GetClientIPAddress()
        {
            string ipAddress = string.Empty;
            try
            {
                // Retrieve the HttpContext from the IHttpContextAccessor
                var httpContext = _httpContextAccessor.HttpContext;

                // Retrieve the remote IP address of the client making the request
                ipAddress = httpContext.Connection.RemoteIpAddress!.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));
            }

            return ipAddress;
        }

        public string GenerateJwtToken(string username, out DateTime expires, out DateTime issued)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value!.ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("Username", username)
            };

            var token = new JwtSecurityToken(
              issuer: _config["Jwt:Issuer"],
              audience: _config["Jwt:Audience"],
              claims: claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: creds
              );

            issued = DateTime.Now;
            expires = DateTime.Now.AddHours(1);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
