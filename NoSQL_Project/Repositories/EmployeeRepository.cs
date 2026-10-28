// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMongoCollection<Employee> _employees;

        public EmployeeRepository(IMongoDatabase db)
        {
            _employees = db.GetCollection<Employee>("Employees");
        }

        public List<Employee> GetAll() =>
            _employees.Find(_ => true).SortBy(e => e.LastName).ToList();

        public Employee? GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return null;

            return _employees.Find(e => e.Id == id).FirstOrDefault();
        }

        public Employee? GetByEmail(string email) =>
            _employees.Find(e => e.Email == email).FirstOrDefault();

        public void Create(Employee employee, string password)
        {
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _employees.InsertOne(employee);
        }

        public bool Update(Employee employee, string? newPassword = null)
        {
            var update = Builders<Employee>.Update
                .Set(e => e.FirstName, employee.FirstName)
                .Set(e => e.LastName, employee.LastName)
                .Set(e => e.Email, employee.Email)
                .Set(e => e.PhoneNumber, employee.PhoneNumber)
                .Set(e => e.UserType, employee.UserType)
                .Set(e => e.Location, employee.Location);

            if (newPassword != null)
                update = update.Set(e => e.PasswordHash, BCrypt.Net.BCrypt.HashPassword(newPassword));

            var result = _employees.UpdateOne(e => e.Id == employee.Id, update);
            return result.MatchedCount > 0;
        }

        public bool Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return false;

            return _employees.DeleteOne(e => e.Id == id).DeletedCount > 0;
        }

        public Employee? Login(string email, string password)
        {
            var employee = GetByEmail(email);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
                return null;

            return employee;
        }
    }
}
