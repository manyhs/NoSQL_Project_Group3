// Author: Cristhel Mae Solomon
using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.Models
{
    public class UserFormViewModel
    {
        public static readonly string[] Locations = { "Haarlem", "Amsterdam", "Knuppeldam", "HQ" };

        public string? Id { get; set; }

        [Required(ErrorMessage = "Please enter a first name.")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Please enter a last name.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Please choose the type of user.")]
        public string UserType { get; set; } = UserTypes.Employee;

        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "This is not a valid email address.")]
        public string Email { get; set; } = "";

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please choose a location.")]
        public string Location { get; set; } = "Haarlem";

        [MinLength(6, ErrorMessage = "The password must have at least 6 characters.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
