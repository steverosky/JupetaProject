namespace Jupeta.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expireTime);
        object RemoveData(string key);
    }
}
