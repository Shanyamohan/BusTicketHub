using System.ComponentModel.DataAnnotations;

namespace BusTicketHub.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits and numeric.")]

        public string phone { get; set; }
        [Required]
        public string password { get; set; }
    }
}
