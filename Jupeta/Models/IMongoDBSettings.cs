namespace Jupeta.Models
{
    public interface IMongoDBSettings
    {
        string ConnectionURI { get; set; } 
        string DatabaseName { get; set; }
        string CollectionName { get; set; }

    }
}
