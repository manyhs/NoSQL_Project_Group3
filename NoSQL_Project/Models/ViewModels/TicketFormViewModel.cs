// Author: Kavan Kamangar
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoSQL_Project.Models
{
    public class TicketFormViewModel
    {
        public static readonly int[] DeadlineChoices = { 1, 3, 7, 14 };

        public string? Id { get; set; }

        [Required(ErrorMessage = "Please enter the subject of the incident.")]
        [StringLength(100, ErrorMessage = "The subject can be at most 100 characters.")]
        public string Subject { get; set; } = "";

        [Required(ErrorMessage = "Please describe the incident.")]
        public string Description { get; set; } = "";

        public TicketType Type { get; set; } = TicketType.Software;

        public TicketPriority Priority { get; set; } = TicketPriority.Normal;

        public string? EmployeeId { get; set; }

        public DateTime ReportedAt { get; set; }

        public int DeadlineDays { get; set; } = 7;

        public DateTime Deadline { get; set; }

        public TicketStatus Status { get; set; }

        public string? HandledByName { get; set; }

        public string? HandledByDescription { get; set; }

        public DateTime? HandledAt { get; set; }

        public bool IsEscalated { get; set; }

        public DateTime? EscalationDate { get; set; }

        public string? EscalationReason { get; set; }

        public bool EscalationRecommended { get; set; }

        [ValidateNever]
        public List<Employee> Employees { get; set; } = new();

        [ValidateNever]
        public IEnumerable<SelectListItem> EmployeeOptions =>
            Employees.Select(e => new SelectListItem($"{e.FullName} ({e.Email})", e.Id ?? ""));

        [ValidateNever]
        public IEnumerable<SelectListItem> ServiceDeskOptions =>
            Employees.Where(e => e.UserType == UserTypes.ServiceDesk)
                .Select(e => new SelectListItem($"{e.FullName} ({e.Email})", e.Id ?? ""));
    }
}
