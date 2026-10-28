// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using MongoDB.Driver;
using NoSQL_Project.Models;

namespace NoSQL_Project.Database
{
    public static class MongoIndexes
    {
        public static void CreateAll(IMongoDatabase db)
        {
            var employees = db.GetCollection<Employee>("Employees");
            employees.Indexes.CreateOne(new CreateIndexModel<Employee>(
                Builders<Employee>.IndexKeys.Ascending(e => e.Email),
                new CreateIndexOptions { Unique = true }));

            var tickets = db.GetCollection<Ticket>("Tickets");
            tickets.Indexes.CreateOne(new CreateIndexModel<Ticket>(
                Builders<Ticket>.IndexKeys.Ascending(t => t.EmployeeId).Ascending(t => t.Status)));
        }
    }
}
