// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using NoSQL_Project.Models;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();

        Employee? GetById(string id);

        Employee? GetByEmail(string email);

        void Create(Employee employee, string password);

        bool Update(Employee employee, string? newPassword = null);

        bool Delete(string id);

        Employee? Login(string email, string password);
    }
}
