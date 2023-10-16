using StackExchange.Redis;
using System.Text.Json;

namespace Jupeta.Services
{
    public class CacheService : ICacheService
    {
        IDatabase _cachedb;
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");
            _cachedb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _cachedb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);

            return default;
        }

        public object RemoveData(string key)
        {
            var exist = _cachedb.KeyExists(key);
            if(exist)
                return _cachedb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expireTime)
        {
            var expiryDateTime = expireTime.DateTime.Subtract(DateTime.Now);
            return _cachedb.StringSet(key, JsonSerializer.Serialize(value), expiryDateTime);
        }
    }
}
