using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SLE_System.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La clave {0} debe contener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Clave")]
        public string Clave { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public string Role { get; set; }
    }
}
