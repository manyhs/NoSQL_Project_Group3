// Author: Mani Hassani
namespace NoSQL_Project.Models
{
    public static class StatusBadge
    {
        public static string CssClass(TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Open => "bg-warning text-dark",
                TicketStatus.Resolved => "bg-success",
                _ => "bg-secondary"
            };
        }
    }
}
