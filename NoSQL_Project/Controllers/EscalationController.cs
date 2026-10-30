// Author: Cristhel Mae Solomon
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Auth;
using NoSQL_Project.Features;
using NoSQL_Project.Models;

namespace NoSQL_Project.Controllers
{
    [Authorize(Roles = UserTypes.ServiceDesk)]
    public class EscalationController : Controller
    {
        private readonly TicketEscalationService _escalation;

        public EscalationController(TicketEscalationService escalation)
        {
            _escalation = escalation;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Escalate(string id, string? reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Please give a reason why the ticket should go to management.";
                return RedirectToAction("Edit", "TicketEditor", new { id });
            }

            if (_escalation.Escalate(id, reason.Trim()))
                TempData["Message"] = "The ticket was escalated to management.";
            else
                TempData["Error"] = "Only open tickets that are not escalated yet can be escalated.";

            return RedirectToAction("Edit", "TicketEditor", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Close(string id, string? reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Please give a reason why the ticket is closed without being solved.";
                return RedirectToAction("Edit", "TicketEditor", new { id });
            }

            _escalation.Close(id, User.GetEmployeeId(), reason.Trim());
            TempData["Message"] = "The ticket was closed without resolution.";

            return RedirectToAction("Edit", "TicketEditor", new { id });
        }
    }
}
