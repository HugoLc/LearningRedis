


using Microsoft.Extensions.Caching.Distributed;

namespace LearningRedis.Infrastructure.Caching
{
    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;

        public CachingService(IDistributedCache cache)
        {
            _cache = cache;
            _options = new DistributedCacheEntryOptions {
                //Tempo maximo absoluto que o item vai permanecer no cache
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                //O item será removido do cache se não for acessado por X minutos consecutivos. No entanto, toda vez que o item for acessado, o período de X minutos será reiniciado. Irá respeitar o tempo máximo de permanencia definido acima.
                SlidingExpiration = TimeSpan.FromSeconds(1200)
            };
        }
        public async Task<string> GetAsync(string key)
        {
            return await _cache.GetStringAsync(key);
        }

        public async Task SetAsync(string key, string value)
        {
            await _cache.SetStringAsync(key, value, _options);

        }
    }
}