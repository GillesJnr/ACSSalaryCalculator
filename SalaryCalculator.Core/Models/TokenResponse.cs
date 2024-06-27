using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Models
{
    public class TokenResponse
    {
        public string? Username { get; set; }
        public string? Token { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Expires_At { get; set; }
    }
}
