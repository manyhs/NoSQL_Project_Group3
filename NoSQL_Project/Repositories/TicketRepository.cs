// Author: Mani Hassani & Kavan Kamangar
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Tickets");
        }

        public List<TicketWithReporter> GetTickets(string? employeeId = null)
        {
            if (employeeId != null && !ObjectId.TryParse(employeeId, out _))
                return new List<TicketWithReporter>();

            var pipeline = new List<BsonDocument>();

            if (employeeId != null)
                pipeline.Add(MatchEmployee(employeeId));

            pipeline.Add(BsonDocument.Parse("{ $lookup: { from: 'Employees', localField: 'Employee_ID', foreignField: '_id', as: 'Reporter' } }"));
            pipeline.Add(BsonDocument.Parse("{ $unwind: { path: '$Reporter', preserveNullAndEmptyArrays: true } }"));
            pipeline.Add(BsonDocument.Parse("{ $project: { 'Reporter.PasswordHash': 0 } }"));
            pipeline.Add(BsonDocument.Parse("{ $sort: { Creation_Date: -1 } }"));

            return _tickets.Aggregate<TicketWithReporter>(pipeline).ToList();
        }

        public Ticket? GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return null;

            return _tickets.Find(t => t.Id == id).FirstOrDefault();
        }

        public void Create(Ticket ticket)
        {
            ticket.Status = TicketStatus.Open;
            ticket.SolutionDate = null;
            ticket.HandledBy = null;

            if (ticket.CreationDate == default)
                ticket.CreationDate = DateTime.UtcNow;

            if (ticket.Deadline == default)
                ticket.Deadline = ticket.CreationDate.AddDays(7);

            _tickets.InsertOne(ticket);
        }

        public bool Update(Ticket ticket)
        {
            var result = _tickets.ReplaceOne(t => t.Id == ticket.Id, ticket);
            return result.MatchedCount > 0;
        }

        public bool ChangeStatus(string ticketId, TicketStatus status, string handlerId, string description)
        {
            if (!ObjectId.TryParse(ticketId, out _))
                return false;

            var now = DateTime.UtcNow;
            DateTime? solutionDate = status == TicketStatus.Open ? null : now;
            var handledBy = new HandledBy { EmployeeId = handlerId, DateTime = now, Description = description };

            var update = Builders<Ticket>.Update
                .Set(t => t.Status, status)
                .Set(t => t.SolutionDate, solutionDate)
                .Set(t => t.HandledBy, handledBy);

            return _tickets.UpdateOne(t => t.Id == ticketId, update).MatchedCount > 0;
        }

        public bool Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return false;

            return _tickets.DeleteOne(t => t.Id == id).DeletedCount > 0;
        }

        public DashboardStats GetDashboardStats(string? employeeId = null)
        {
            if (employeeId != null && !ObjectId.TryParse(employeeId, out _))
                return new DashboardStats();

            var pipeline = new List<BsonDocument>();

            if (employeeId != null)
                pipeline.Add(MatchEmployee(employeeId));

            pipeline.Add(BsonDocument.Parse("{ $group: { _id: '$Status', Count: { $sum: 1 } } }"));

            var counts = _tickets.Aggregate<BsonDocument>(pipeline).ToList()
                .ToDictionary(row => row["_id"].AsString, row => row["Count"].AsInt32);

            return new DashboardStats
            {
                Open = counts.GetValueOrDefault("Open"),
                Resolved = counts.GetValueOrDefault("Resolved"),
                Closed = counts.GetValueOrDefault("Closed"),
                PastDeadline = CountPastDeadline(employeeId)
            };
        }

        private int CountPastDeadline(string? employeeId)
        {
            var now = DateTime.UtcNow;

            if (employeeId == null)
                return (int)_tickets.CountDocuments(t => t.Status == TicketStatus.Open && t.Deadline < now);

            return (int)_tickets.CountDocuments(t => t.Status == TicketStatus.Open && t.Deadline < now && t.EmployeeId == employeeId);
        }

        private static BsonDocument MatchEmployee(string employeeId)
        {
            return new BsonDocument("$match", new BsonDocument("Employee_ID", ObjectId.Parse(employeeId)));
        }
    }
}
