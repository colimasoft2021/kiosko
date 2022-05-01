using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Error
    {
        public int Id { get; set; }
        public string? Bloque { get; set; }
        public string? Controlador { get; set; }
        public string? Metodo { get; set; }
        public string? Mensaje { get; set; }
    }
}
