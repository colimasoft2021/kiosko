﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SLE_System.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
