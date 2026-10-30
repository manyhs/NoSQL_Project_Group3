// Author: Cristhel Mae Solomon
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Features
{
    public class TicketEscalationService
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly ITicketRepository _ticketRepository;

        public TicketEscalationService(IMongoDatabase db, ITicketRepository ticketRepository)
        {
            _tickets = db.GetCollection<Ticket>("Tickets");
            _ticketRepository = ticketRepository;
        }

        public bool ShouldEscalate(Ticket ticket)
        {
            return ticket.Status == TicketStatus.Open
                && !ticket.IsEscalated
                && ticket.Priority == TicketPriority.High
                && ticket.Deadline < DateTime.UtcNow;
        }

        public bool Escalate(string ticketId, string reason)
        {
            if (!ObjectId.TryParse(ticketId, out _))
                return false;

            var update = Builders<Ticket>.Update
                .Set(t => t.IsEscalated, true)
                .Set(t => t.EscalationDate, DateTime.UtcNow)
                .Set(t => t.EscalationReason, reason);

            var filter = Builders<Ticket>.Filter.Where(t => t.Id == ticketId && t.Status == TicketStatus.Open && !t.IsEscalated);

            return _tickets.UpdateOne(filter, update).ModifiedCount > 0;
        }

        public bool Close(string ticketId, string handlerId, string reason)
        {
            return _ticketRepository.ChangeStatus(ticketId, TicketStatus.Closed, handlerId, reason);
        }
    }
}
