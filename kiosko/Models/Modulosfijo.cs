using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Modulosfijo
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Desplegable { get; set; }
        public string? IdModulo { get; set; }
        public string? Padre { get; set; }
    }
}
