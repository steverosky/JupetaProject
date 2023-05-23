namespace Jupeta.Models.DBModels
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionURI { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
    }
}
