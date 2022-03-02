﻿using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario1 { get; set; } = null!;
        public string? Clave { get; set; }
        public string? Rol { get; set; }
        public byte[]? Email { get; set; }
    }
}
