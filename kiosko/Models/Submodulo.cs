using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Submodulo
    {
        public int Id { get; set; }
        public int? IdModulo { get; set; }
        public string? IdSubmodulo { get; set; }
        public string? Titulo { get; set; }
        public string? Padre { get; set; }
        public int? AccesoDirecto { get; set; }
        public int? Nivel { get; set; }
        public int? Desplegable { get; set; }
    }
}
