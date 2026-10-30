// Author: Kavan Kamangar
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Features
{
    public class TicketTransferService
    {
        private readonly ITicketRepository _tickets;
        private readonly IEmployeeRepository _employees;

        public TicketTransferService(ITicketRepository tickets, IEmployeeRepository employees)
        {
            _tickets = tickets;
            _employees = employees;
        }

        public (bool Success, string Message) Transfer(string ticketId, string newHandlerId)
        {
            var ticket = _tickets.GetById(ticketId);
            if (ticket == null)
                return (false, "This ticket does not exist anymore.");

            var newHandler = _employees.GetById(newHandlerId);
            if (newHandler == null || newHandler.UserType != UserTypes.ServiceDesk)
                return (false, "A ticket can only be transferred to a service desk employee.");

            if (ticket.HandledBy?.EmployeeId == newHandlerId)
                return (false, $"{newHandler.FullName} is already handling this ticket.");

            var previousHandler = CurrentHandlerName(ticket);

            ticket.HandledBy = new HandledBy
            {
                EmployeeId = newHandlerId,
                DateTime = DateTime.UtcNow,
                Description = $"Transferred from {previousHandler} to {newHandler.FullName}."
            };
            _tickets.Update(ticket);

            return (true, $"The ticket was transferred to {newHandler.FullName}.");
        }

        private string CurrentHandlerName(Ticket ticket)
        {
            if (ticket.HandledBy == null)
                return "nobody";

            return _employees.GetById(ticket.HandledBy.EmployeeId)?.FullName ?? "a deleted employee";
        }
    }
}
