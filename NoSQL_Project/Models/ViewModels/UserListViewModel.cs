// Author: Cristhel Mae Solomon
namespace NoSQL_Project.Models
{
    public class UserListViewModel
    {
        public List<EmployeeWithTicketCount> Users { get; set; } = new();
        public string? Email { get; set; }
    }
}
