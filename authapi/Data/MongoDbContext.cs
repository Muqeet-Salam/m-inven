using MongoDB.Driver;
using MongoDB.Bson;
using authapi.Models;
using SharpCompress.Compressors.ZStandard.Unsafe;

namespace authapi.Data;

public class MongoDbContext {
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration) {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];
        var settings = MongoClientSettings.FromConnectionString(connectionString!);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        _database = client.GetDatabase(databaseName);
    }
    
    public async Task<bool> PingAsync() {
        try {
            await _database
                .RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1)
                );
            return true;
        } catch {
            return false;
        }
    }

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");

    public IMongoCollection<Product> Products =>
        _database.GetCollection<Product>("Products");

    public IMongoCollection<InventoryTransaction> InventoryTransactions =>
        _database.GetCollection<InventoryTransaction>("InventoryTransactions");
}