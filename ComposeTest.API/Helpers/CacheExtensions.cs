using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

namespace ComposeTest.API.Helpers {
    public static class CacheExtensions {
        public async static Task<T?> GetFromJsonAsync<T>(this IDistributedCache cache, string key) {
            var cacheContent = await cache.GetAsync(key);
            if (cacheContent is null) {
                return await Task.FromResult<T?>(default);
            }

            return JsonSerializer.Deserialize<T>(cacheContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="item">must be json serializable</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async static Task SetAsJsonAsync<T>(this IDistributedCache cache, string key, T item) {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(item));
        }
    }
}
