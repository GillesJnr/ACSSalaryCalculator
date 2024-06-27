using Microsoft.Extensions.Caching.Memory;
using SalaryCalculator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalculator.Core.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string cacheKey)
        {
            _memoryCache.TryGetValue(cacheKey, out T value);
            return value;
        }

        public void Set<T>(string cacheKey, T item, MemoryCacheEntryOptions options)
        {
            _memoryCache.Set(cacheKey, item, options);
        }
    }
}
