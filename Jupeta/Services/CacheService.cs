using StackExchange.Redis;
using System.Text.Json;

namespace Jupeta.Services
{
    public class CacheService : ICacheService
    {
        IDatabase _cachedb;
        IConfiguration _config;
        public CacheService(IConfiguration config)
        {
            _config = config;
            var redisConn = _config.GetValue<string>("redis");
            var redis = ConnectionMultiplexer.Connect(redisConn);
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
