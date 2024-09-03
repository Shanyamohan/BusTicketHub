using System.ComponentModel.DataAnnotations;

namespace Demo2.Models
{
    public class Customer
    {
        [Required]
        public int id {  get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string phone { get; set; }
        [Required]
        public string password {  get; set; }
    }
}
