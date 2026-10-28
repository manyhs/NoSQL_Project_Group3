// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories;

namespace NoSQL_Project_Tests
{
    [TestFixture]
    public class EmployeeRepositoryTests
    {
        private EmployeeRepository _repo = null!;

        [SetUp]
        public void SetUp() => _repo = new EmployeeRepository(TestDatabase.GetCleanDatabase());

        private static Employee NewEmployee(string email) => new()
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PhoneNumber = "+31 6 00000000",
            UserType = UserTypes.Employee,
            Location = "Haarlem"
        };

        [Test]
        public void Create_SavesEmployee_WithHashedPassword()
        {
            var employee = NewEmployee("create@tgg.nl");
            _repo.Create(employee, "Secret123");

            var saved = _repo.GetById(employee.Id!);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved!.Email, Is.EqualTo("create@tgg.nl"));
            Assert.That(saved.PasswordHash, Is.Not.EqualTo("Secret123"));
            Assert.That(saved.PasswordHash, Does.StartWith("$2"));
        }

        [Test]
        public void Create_WithExistingEmail_Throws()
        {
            _repo.Create(NewEmployee("same@tgg.nl"), "a");
            Assert.Throws<MongoWriteException>(() => _repo.Create(NewEmployee("same@tgg.nl"), "b"));
        }

        [Test]
        public void GetAll_And_GetByEmail_ReturnCreatedEmployees()
        {
            _repo.Create(NewEmployee("one@tgg.nl"), "a");
            _repo.Create(NewEmployee("two@tgg.nl"), "b");

            Assert.That(_repo.GetAll(), Has.Count.EqualTo(2));
            Assert.That(_repo.GetByEmail("two@tgg.nl"), Is.Not.Null);
            Assert.That(_repo.GetByEmail("nobody@tgg.nl"), Is.Null);
        }

        [Test]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            Assert.That(_repo.GetById("not-an-id"), Is.Null);
        }

        [Test]
        public void Update_ChangesFields_AndOptionallyPassword()
        {
            var employee = NewEmployee("update@tgg.nl");
            _repo.Create(employee, "OldPass");

            employee.Location = "Amsterdam";
            employee.UserType = UserTypes.ServiceDesk;
            Assert.That(_repo.Update(employee), Is.True);

            var saved = _repo.GetById(employee.Id!)!;
            Assert.That(saved.Location, Is.EqualTo("Amsterdam"));
            Assert.That(saved.UserType, Is.EqualTo(UserTypes.ServiceDesk));
            Assert.That(_repo.Login("update@tgg.nl", "OldPass"), Is.Not.Null, "password should not change");

            _repo.Update(employee, "NewPass");
            Assert.That(_repo.Login("update@tgg.nl", "NewPass"), Is.Not.Null);
            Assert.That(_repo.Login("update@tgg.nl", "OldPass"), Is.Null);
        }

        [Test]
        public void Delete_RemovesEmployee()
        {
            var employee = NewEmployee("delete@tgg.nl");
            _repo.Create(employee, "a");

            Assert.That(_repo.Delete(employee.Id!), Is.True);
            Assert.That(_repo.GetById(employee.Id!), Is.Null);
            Assert.That(_repo.Delete(employee.Id!), Is.False);
        }

        [Test]
        public void Login_ChecksPassword()
        {
            _repo.Create(NewEmployee("login@tgg.nl"), "Correct1");

            Assert.That(_repo.Login("login@tgg.nl", "Correct1"), Is.Not.Null);
            Assert.That(_repo.Login("login@tgg.nl", "Wrong"), Is.Null);
            Assert.That(_repo.Login("unknown@tgg.nl", "Correct1"), Is.Null);
        }
    }
}
