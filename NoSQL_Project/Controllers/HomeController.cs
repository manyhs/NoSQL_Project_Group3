// Author: Mariana da Costa Reis & Cristhel Mae Solomon
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employees;
        private readonly ITicketRepository _tickets;

        public HomeController(IEmployeeRepository employees, ITicketRepository tickets)
        {
            _employees = employees;
            _tickets = tickets;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Stats = _tickets.GetDashboardStats(),
                Tickets = _tickets.GetTickets(),
                EmployeeCount = _employees.GetAll().Count
            };
            return View(model);
        }
    }
}
