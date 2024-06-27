﻿using SalaryCalculator.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Get (string username, string password);
    }
}
