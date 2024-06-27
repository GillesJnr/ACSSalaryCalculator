using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string cacheKey);
        void Set<T>(string cacheKey, T item, MemoryCacheEntryOptions options);
    }
}
