namespace SpendWiseAPI.Config
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}
// This file defines the MongoDB settings used by the application.  
// It includes properties for the connection string and database name, which are essential for connecting to a MongoDB instance. 
// The settings are typically loaded from configuration files like appsettings.json or environment variables, allowing for flexible deployment configurations.                   