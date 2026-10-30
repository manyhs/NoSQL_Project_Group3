// Author: Cristhel Mae Solomon
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    [BsonIgnoreExtraElements]
    public class EmployeeWithTicketCount : Employee
    {
        public int TicketCount { get; set; }
    }
}
