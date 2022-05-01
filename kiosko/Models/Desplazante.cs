using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Desplazante
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Titulo { get; set; }
        public string? Texto { get; set; }
        public int IdComponente { get; set; }
        public string? BackgroundColor { get; set; }

        public virtual Componente IdComponenteNavigation { get; set; } = null!;
    }
}
