// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    [BsonIgnoreExtraElements]
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public string UserType { get; set; } = UserTypes.Employee;

        public string Location { get; set; } = "";

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
