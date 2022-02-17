using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace kiosko.Models
{
    [Keyless]
    public class LoginModel
    {
        [Required]
        public string vchEmail { get; set; }
        [Required]
        public string vchPass { get; set; }
    }
}
