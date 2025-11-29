
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SimpleRedis.Services
{
    public class RedisCacheService : IRedisCacheService
    {

        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            _logger.LogInformation("Getting value from Redis cache for key: {Key}", key);
            var json = await _cache.GetStringAsync(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json!);
        }



        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            _logger.LogInformation("Setting value in Redis cache for key: {Key}", key);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
            };

            var json = JsonSerializer.Serialize(value);  // Convert T to JSON string
            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveAsync(string key)
        {
            _logger.LogInformation("Removing value from Redis cache for key: {Key}", key);
            await _cache.RemoveAsync(key);
        }
    }
}
