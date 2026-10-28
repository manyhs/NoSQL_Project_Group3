// Author: Mani Hassani & Kavan Kamangar
using NoSQL_Project.Models;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        List<TicketWithReporter> GetTickets(string? employeeId = null);

        Ticket? GetById(string id);

        void Create(Ticket ticket);

        bool Update(Ticket ticket);

        bool ChangeStatus(string ticketId, TicketStatus status, string handlerId, string description);

        bool Delete(string id);

        DashboardStats GetDashboardStats(string? employeeId = null);
    }
}
