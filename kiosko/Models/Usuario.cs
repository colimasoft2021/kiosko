using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string? Clave { get; set; }
        public string? Rol { get; set; }
        public string? Email { get; set; }
    }
}
