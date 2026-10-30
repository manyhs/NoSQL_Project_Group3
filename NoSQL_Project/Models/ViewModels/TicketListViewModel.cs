// Author: Mani Hassani
namespace NoSQL_Project.Models
{
    public class TicketListViewModel
    {
        public List<TicketWithReporter> Tickets { get; set; } = new();
        public string? Email { get; set; }
        public string? Keywords { get; set; }
        public string? Sort { get; set; }
        public string? Message { get; set; }
    }
}
