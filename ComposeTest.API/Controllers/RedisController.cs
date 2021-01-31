using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ComposeTest.API.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ComposeTest.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class RedisController : ControllerBase {
        private const string CACHE_KEY = "forecasts";

        private readonly ILogger<RedisController> _logger;

        public IDistributedCache Cache { get; }

        public RedisController(ILogger<RedisController> logger, IDistributedCache cache) {
            _logger = logger;
            Cache = cache;
        }

        [HttpGet]
        public async Task<IEnumerable<CacheEntry>?> Get() {
            try {
                var result = await Cache.GetFromJsonAsync<List<CacheEntry>>(CACHE_KEY);
                return result;
            }
            catch {
                _logger.LogError("Could not parse entry in redis");
                return new List<CacheEntry>();
            }
        }

        [HttpPost]
        public async Task Post(CacheEntry item) {
            try {
                var items = await Cache.GetFromJsonAsync<List<CacheEntry>>(CACHE_KEY) ?? new List<CacheEntry>();
                items.Add(item);
                await Cache.SetAsJsonAsync(CACHE_KEY, items);
            }
            catch {
                _logger.LogError("Failed to parse existing cache items");
                var items = new List<CacheEntry> {
                    item
                };
                await Cache.SetAsJsonAsync(CACHE_KEY, items);
            }
        }

        [HttpDelete]
        public async Task Clear() {
            var items = new List<CacheEntry>();
            await Cache.SetAsJsonAsync(CACHE_KEY, items);
        }
    }

    public record CacheEntry(int A, string B);
}
