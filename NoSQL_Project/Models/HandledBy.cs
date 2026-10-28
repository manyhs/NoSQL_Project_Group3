// Author: Mani Hassani & Kavan Kamangar
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    public class HandledBy
    {
        [BsonElement("Employee_ID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; } = "";

        [BsonElement("Date_Time")]
        public DateTime DateTime { get; set; }

        public string Description { get; set; } = "";
    }
}
