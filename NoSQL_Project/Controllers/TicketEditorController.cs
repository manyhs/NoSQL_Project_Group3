// Author: Kavan Kamangar
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Auth;
using NoSQL_Project.Features;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Controllers
{
    [Authorize]
    public class TicketEditorController : Controller
    {
        private readonly ITicketRepository _tickets;
        private readonly IEmployeeRepository _employees;
        private readonly TicketTransferService _transfer;
        private readonly TicketEscalationService _escalation;

        public TicketEditorController(ITicketRepository tickets, IEmployeeRepository employees,
            TicketTransferService transfer, TicketEscalationService escalation)
        {
            _tickets = tickets;
            _employees = employees;
            _transfer = transfer;
            _escalation = escalation;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new TicketFormViewModel
            {
                ReportedAt = RoundToMinutes(DateTime.Now),
                EmployeeId = User.GetEmployeeId(),
                Employees = _employees.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TicketFormViewModel model)
        {
            if (!User.IsServiceDesk())
                model.EmployeeId = User.GetEmployeeId();

            CheckReporter(model);
            if (!TicketFormViewModel.DeadlineChoices.Contains(model.DeadlineDays))
                ModelState.AddModelError(nameof(model.DeadlineDays), "Please choose a deadline.");

            if (!ModelState.IsValid)
            {
                model.Employees = _employees.GetAll();
                return View(model);
            }

            var reportedAt = model.ReportedAt.ToUniversalTime();
            var ticket = new Ticket
            {
                EmployeeId = model.EmployeeId!,
                Subject = model.Subject.Trim(),
                Description = model.Description.Trim(),
                Type = model.Type,
                Priority = model.Priority,
                CreationDate = reportedAt,
                Deadline = reportedAt.AddDays(model.DeadlineDays)
            };
            _tickets.Create(ticket);

            TempData["Message"] = $"Ticket \"{ticket.Subject}\" was created.";
            return RedirectToAction("Index", "Tickets");
        }

        [HttpGet]
        [Authorize(Roles = UserTypes.ServiceDesk)]
        public IActionResult Edit(string id)
        {
            var ticket = _tickets.GetById(id);
            if (ticket == null)
                return NotFound();

            return View(BuildEditModel(ticket));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserTypes.ServiceDesk)]
        public IActionResult Edit(TicketFormViewModel form)
        {
            var ticket = _tickets.GetById(form.Id ?? "");
            if (ticket == null)
                return NotFound();

            CheckReporter(form);
            if (form.Deadline < form.ReportedAt)
                ModelState.AddModelError(nameof(form.Deadline), "The deadline cannot be before the date the incident was reported.");

            if (!ModelState.IsValid)
            {
                var model = BuildEditModel(ticket);
                model.Subject = form.Subject;
                model.Description = form.Description;
                model.Type = form.Type;
                model.Priority = form.Priority;
                model.EmployeeId = form.EmployeeId;
                model.ReportedAt = form.ReportedAt;
                model.Deadline = form.Deadline;
                return View(model);
            }

            ticket.Subject = form.Subject.Trim();
            ticket.Description = form.Description.Trim();
            ticket.Type = form.Type;
            ticket.Priority = form.Priority;
            ticket.EmployeeId = form.EmployeeId!;
            ticket.CreationDate = form.ReportedAt.ToUniversalTime();
            ticket.Deadline = form.Deadline.ToUniversalTime();
            _tickets.Update(ticket);

            TempData["Message"] = "The ticket was saved.";
            return RedirectToAction(nameof(Edit), new { id = ticket.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserTypes.ServiceDesk)]
        public IActionResult ChangeStatus(string id, TicketStatus status, string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Please describe what was done.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            _tickets.ChangeStatus(id, status, User.GetEmployeeId(), description.Trim());

            TempData["Message"] = $"The status was changed to {status}.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserTypes.ServiceDesk)]
        public IActionResult Transfer(string id, string? newHandlerId)
        {
            if (string.IsNullOrEmpty(newHandlerId))
            {
                TempData["Error"] = "Please choose who should handle the ticket.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var (success, message) = _transfer.Transfer(id, newHandlerId);
            TempData[success ? "Message" : "Error"] = message;
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserTypes.ServiceDesk)]
        public IActionResult Delete(string id)
        {
            if (_tickets.Delete(id))
                TempData["Message"] = "The ticket was deleted.";
            else
                TempData["Error"] = "The ticket could not be found.";

            return RedirectToAction("Index", "Tickets");
        }

        private void CheckReporter(TicketFormViewModel model)
        {
            if (string.IsNullOrEmpty(model.EmployeeId) || _employees.GetById(model.EmployeeId) == null)
                ModelState.AddModelError(nameof(model.EmployeeId), "Please choose the employee who reported the incident.");
        }

        private TicketFormViewModel BuildEditModel(Ticket ticket)
        {
            var employees = _employees.GetAll();
            var handler = ticket.HandledBy == null
                ? null
                : employees.FirstOrDefault(e => e.Id == ticket.HandledBy.EmployeeId);

            return new TicketFormViewModel
            {
                Id = ticket.Id,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Type = ticket.Type,
                Priority = ticket.Priority,
                EmployeeId = ticket.EmployeeId,
                ReportedAt = RoundToMinutes(ticket.CreationDate.ToLocalTime()),
                Deadline = RoundToMinutes(ticket.Deadline.ToLocalTime()),
                Status = ticket.Status,
                HandledByName = ticket.HandledBy == null ? null : handler?.FullName ?? "(deleted employee)",
                HandledByDescription = ticket.HandledBy?.Description,
                HandledAt = ticket.HandledBy?.DateTime.ToLocalTime(),
                IsEscalated = ticket.IsEscalated,
                EscalationDate = ticket.EscalationDate?.ToLocalTime(),
                EscalationReason = ticket.EscalationReason,
                EscalationRecommended = _escalation.ShouldEscalate(ticket),
                Employees = employees
            };
        }

        private static DateTime RoundToMinutes(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);
        }
    }
}
