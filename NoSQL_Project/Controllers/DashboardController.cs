// Author: Mani Hassani
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Auth;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITicketRepository _tickets;

        public DashboardController(ITicketRepository tickets)
        {
            _tickets = tickets;
        }

        public IActionResult Index()
        {
            var showsAllTickets = User.IsServiceDesk();
            var employeeId = showsAllTickets ? null : User.GetEmployeeId();

            var model = new DashboardViewModel
            {
                Stats = _tickets.GetDashboardStats(employeeId),
                ShowsAllTickets = showsAllTickets
            };

            return View(model);
        }
    }
}
