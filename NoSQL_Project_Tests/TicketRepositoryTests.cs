// Author: Mani Hassani & Kavan Kamangar
using NoSQL_Project.Models;
using NoSQL_Project.Repositories;

namespace NoSQL_Project_Tests
{
    [TestFixture]
    public class TicketRepositoryTests
    {
        private TicketRepository _tickets = null!;
        private Employee _reporter = null!;
        private Employee _serviceDesk = null!;

        [SetUp]
        public void SetUp()
        {
            var db = TestDatabase.GetCleanDatabase();
            _tickets = new TicketRepository(db);

            var employees = new EmployeeRepository(db);
            _reporter = new Employee { FirstName = "Anna", LastName = "Jansen", Email = "anna@tgg.nl", UserType = UserTypes.Employee };
            _serviceDesk = new Employee { FirstName = "Mark", LastName = "de Vries", Email = "mark@tgg.nl", UserType = UserTypes.ServiceDesk };
            employees.Create(_reporter, "a");
            employees.Create(_serviceDesk, "b");
        }

        private Ticket NewTicket(string employeeId, string subject = "Printer broken") => new()
        {
            EmployeeId = employeeId,
            Subject = subject,
            Description = "Test ticket",
            Priority = TicketPriority.High,
            Type = TicketType.Hardware
        };

        [Test]
        public void Create_StartsAsOpen_WithoutHandler()
        {
            var ticket = NewTicket(_reporter.Id!);
            ticket.Status = TicketStatus.Closed;
            _tickets.Create(ticket);

            var saved = _tickets.GetById(ticket.Id!)!;
            Assert.That(saved.Status, Is.EqualTo(TicketStatus.Open));
            Assert.That(saved.HandledBy, Is.Null);
            Assert.That(saved.SolutionDate, Is.Null);
            Assert.That(saved.Priority, Is.EqualTo(TicketPriority.High));
            Assert.That(saved.Deadline, Is.EqualTo(saved.CreationDate.AddDays(7)).Within(TimeSpan.FromSeconds(1)),
                "a ticket without deadline gets 7 days");
        }

        [Test]
        public void GetTickets_IncludesReporter_AndFiltersByEmployee()
        {
            _tickets.Create(NewTicket(_reporter.Id!, "From Anna"));
            _tickets.Create(NewTicket(_serviceDesk.Id!, "From Mark"));

            var all = _tickets.GetTickets();
            Assert.That(all, Has.Count.EqualTo(2));

            var annas = _tickets.GetTickets(_reporter.Id);
            Assert.That(annas, Has.Count.EqualTo(1));
            Assert.That(annas[0].Subject, Is.EqualTo("From Anna"));
            Assert.That(annas[0].Reporter!.FullName, Is.EqualTo("Anna Jansen"));
            Assert.That(annas[0].Reporter!.PasswordHash, Is.Empty, "password hash must not be returned");
        }

        [Test]
        public void Update_SavesAllFields()
        {
            var ticket = NewTicket(_reporter.Id!);
            _tickets.Create(ticket);

            ticket.Subject = "Printer still broken";
            ticket.Priority = TicketPriority.Low;
            Assert.That(_tickets.Update(ticket), Is.True);

            var saved = _tickets.GetById(ticket.Id!)!;
            Assert.That(saved.Subject, Is.EqualTo("Printer still broken"));
            Assert.That(saved.Priority, Is.EqualTo(TicketPriority.Low));
        }

        [Test]
        public void ChangeStatus_SetsHandlerAndSolutionDate()
        {
            var ticket = NewTicket(_reporter.Id!);
            _tickets.Create(ticket);

            Assert.That(_tickets.ChangeStatus(ticket.Id!, TicketStatus.Resolved, _serviceDesk.Id!, "Replaced toner"), Is.True);

            var saved = _tickets.GetById(ticket.Id!)!;
            Assert.That(saved.Status, Is.EqualTo(TicketStatus.Resolved));
            Assert.That(saved.SolutionDate, Is.Not.Null);
            Assert.That(saved.HandledBy!.EmployeeId, Is.EqualTo(_serviceDesk.Id));
            Assert.That(saved.HandledBy.Description, Is.EqualTo("Replaced toner"));
        }

        [Test]
        public void Delete_RemovesTicket()
        {
            var ticket = NewTicket(_reporter.Id!);
            _tickets.Create(ticket);

            Assert.That(_tickets.Delete(ticket.Id!), Is.True);
            Assert.That(_tickets.GetById(ticket.Id!), Is.Null);
        }

        [Test]
        public void GetDashboardStats_CountsPerStatus_ForAllAndForOneEmployee()
        {
            var t1 = NewTicket(_reporter.Id!);
            var t2 = NewTicket(_reporter.Id!);
            var t3 = NewTicket(_reporter.Id!);
            var t4 = NewTicket(_serviceDesk.Id!);
            foreach (var t in new[] { t1, t2, t3, t4 }) _tickets.Create(t);

            _tickets.ChangeStatus(t2.Id!, TicketStatus.Resolved, _serviceDesk.Id!, "Fixed");
            _tickets.ChangeStatus(t3.Id!, TicketStatus.Closed, _serviceDesk.Id!, "Duplicate");

            var all = _tickets.GetDashboardStats();
            Assert.That(all.Total, Is.EqualTo(4));
            Assert.That(all.Open, Is.EqualTo(2));
            Assert.That(all.OpenPercentage, Is.EqualTo(50.0));

            var annas = _tickets.GetDashboardStats(_reporter.Id);
            Assert.That(annas.Open, Is.EqualTo(1));
            Assert.That(annas.Resolved, Is.EqualTo(1));
            Assert.That(annas.Closed, Is.EqualTo(1));
            Assert.That(annas.ResolvedPercentage, Is.EqualTo(33.3));
        }

        [Test]
        public void GetDashboardStats_CountsOpenTicketsPastDeadline()
        {
            var late = NewTicket(_reporter.Id!);
            var onTime = NewTicket(_reporter.Id!);
            var lateButResolved = NewTicket(_reporter.Id!);
            late.Deadline = DateTime.UtcNow.AddDays(-1);
            lateButResolved.Deadline = DateTime.UtcNow.AddDays(-1);
            foreach (var t in new[] { late, onTime, lateButResolved }) _tickets.Create(t);
            _tickets.ChangeStatus(lateButResolved.Id!, TicketStatus.Resolved, _serviceDesk.Id!, "Fixed");

            Assert.That(_tickets.GetDashboardStats().PastDeadline, Is.EqualTo(1));
        }
    }
}
