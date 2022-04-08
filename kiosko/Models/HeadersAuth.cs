using System.ComponentModel.DataAnnotations;

namespace kiosko.Models
{
    public class HeadersAuth
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
