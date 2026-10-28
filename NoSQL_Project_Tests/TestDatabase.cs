// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Database;

namespace NoSQL_Project_Tests
{
    public static class TestDatabase
    {
        public static IMongoDatabase GetCleanDatabase()
        {
            DotNetEnv.Env.TraversePath().Load();

            var conn = Environment.GetEnvironmentVariable("Mongo__ConnectionString") ?? "";
            if (string.IsNullOrWhiteSpace(conn))
                Assert.Ignore("Mongo__ConnectionString not set. Did you create .env?");

            var db = new MongoClient(conn).GetDatabase("garden_group_test");
            db.GetCollection<BsonDocument>("Employees").DeleteMany(FilterDefinition<BsonDocument>.Empty);
            db.GetCollection<BsonDocument>("Tickets").DeleteMany(FilterDefinition<BsonDocument>.Empty);
            MongoIndexes.CreateAll(db);
            return db;
        }
    }
}
