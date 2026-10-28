// Author: Mariana da Costa Reis & Cristhel Mae Solomon
namespace NoSQL_Project.Models
{
    public class HomeViewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public List<TicketWithReporter> Tickets { get; set; } = new();
        public int EmployeeCount { get; set; }
    }
}
