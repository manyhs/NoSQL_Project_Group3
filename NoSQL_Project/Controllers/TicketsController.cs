// Author: Mani Hassani
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Auth;
using NoSQL_Project.Features;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketRepository _tickets;
        private readonly IEmployeeRepository _employees;

        public TicketsController(ITicketRepository tickets, IEmployeeRepository employees)
        {
            _tickets = tickets;
            _employees = employees;
        }

        public IActionResult Index(string? email, string? keywords, string? sort)
        {
            var model = new TicketListViewModel { Email = email, Keywords = keywords, Sort = sort };
            var tickets = LoadTickets(email, model);

            tickets = TicketFilter.ByKeywords(tickets, keywords);
            model.Tickets = TicketSorter.Sort(tickets, sort);

            return View(model);
        }

        private List<TicketWithReporter> LoadTickets(string? email, TicketListViewModel model)
        {
            if (!User.IsServiceDesk())
                return _tickets.GetTickets(User.GetEmployeeId());

            if (string.IsNullOrWhiteSpace(email))
                return _tickets.GetTickets();

            var employee = _employees.GetByEmail(email.Trim().ToLowerInvariant());
            if (employee == null)
            {
                model.Message = $"There is no employee with the email address {email}.";
                return new List<TicketWithReporter>();
            }

            return _tickets.GetTickets(employee.Id);
        }
    }
}
