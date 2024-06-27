using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Interfaces
{
    public interface IUtilityRepository
    {
        Task<string> ComputeSha256(string rawData);
        Task<string> GetClientIPAddress();
        string GenerateJwtToken(string username, out DateTime expires, out DateTime issued);
    }
}
