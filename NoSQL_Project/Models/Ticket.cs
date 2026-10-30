// Author: Mani Hassani & Kavan Kamangar
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    [BsonIgnoreExtraElements]
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Employee_ID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; } = "";

        public string Subject { get; set; } = "";

        [BsonRepresentation(BsonType.String)]
        public TicketPriority Priority { get; set; } = TicketPriority.Normal;

        [BsonRepresentation(BsonType.String)]
        public TicketType Type { get; set; } = TicketType.Software;

        public string Description { get; set; } = "";

        [BsonRepresentation(BsonType.String)]
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        [BsonElement("Creation_Date")]
        public DateTime CreationDate { get; set; }

        public DateTime Deadline { get; set; }

        [BsonElement("Solution_Date")]
        public DateTime? SolutionDate { get; set; }

        public HandledBy? HandledBy { get; set; }

        public bool IsEscalated { get; set; }

        [BsonElement("Escalation_Date")]
        public DateTime? EscalationDate { get; set; }

        [BsonElement("Escalation_Reason")]
        public string? EscalationReason { get; set; }
    }
}
