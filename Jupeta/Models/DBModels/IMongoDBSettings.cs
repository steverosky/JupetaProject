namespace Jupeta.Models.DBModels
{
    public interface IMongoDBSettings
    {
        string ConnectionURI { get; set; }
        string DatabaseName { get; set; }
        string CollectionName { get; set; }

    }
}
