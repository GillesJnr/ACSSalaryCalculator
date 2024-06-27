using Microsoft.EntityFrameworkCore;
using SalaryCalculator.Infrastructure.Data;
using SalaryCalculator.Infrastructure.Entities;
using SalaryCalculator.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiDbContext _context;
        public UserRepository( ApiDbContext context)
        {
           _context = context; 
        }

        public async Task<User> Get(string username, string password)
        {
             return await _context.Users.FirstOrDefaultAsync(s=> s.Username == username && s.Password == password);
        }

    }
}
