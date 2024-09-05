using System.ComponentModel.DataAnnotations;

namespace BusTicketHub.Models
{
    public class Login
    {
        [Required]
        public string phone { get; set; }
        [Required]
        public string password { get; set; }
    }
}
