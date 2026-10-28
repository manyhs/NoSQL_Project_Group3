// Author: Mani Hassani & Kavan Kamangar
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    [BsonIgnoreExtraElements]
    public class TicketWithReporter : Ticket
    {
        public Employee? Reporter { get; set; }
    }
}
