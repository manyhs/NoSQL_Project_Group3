// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using MongoDB.Bson;
using MongoDB.Driver;

namespace NoSQL_Project_Tests
{
    [TestFixture]
    public class MongoConnectivityTest
    {
        [Test]
        public void Should_Connect_To_Atlas_And_Ping()
        {
            DotNetEnv.Env.TraversePath().Load();
            var conn = Environment.GetEnvironmentVariable("Mongo__ConnectionString") ?? "";
            if (string.IsNullOrWhiteSpace(conn))
                Assert.Ignore("Mongo__ConnectionString not set. Did you create .env?");

            var settings = MongoClientSettings.FromConnectionString(conn);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            var result = new MongoClient(settings).GetDatabase("admin")
                .RunCommand<BsonDocument>(new BsonDocument("ping", 1));

            Assert.That(result["ok"].ToInt32(), Is.EqualTo(1));
        }
    }
}
