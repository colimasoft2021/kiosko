﻿using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Progresos = new HashSet<Progreso>();
        }

        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string? Clave { get; set; }
        public string? Rol { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Progreso> Progresos { get; set; }
    }
}
