using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Componente
    {
        public Componente()
        {
            Desplazantes = new HashSet<Desplazante>();
        }

        public int Id { get; set; }
        public string? Padre { get; set; }
        public string? TipoComponente { get; set; }
        public string? Url { get; set; }
        public string? Descripcion { get; set; }
        public string? BackgroundColor { get; set; }
        public int? AgregarFondo { get; set; }
        public string? Titulo { get; set; }
        public string? Subtitulo { get; set; }
        public int? Orden { get; set; }
        public int IdModulo { get; set; }

        public virtual Modulo IdModuloNavigation { get; set; } = null!;
        public virtual ICollection<Desplazante> Desplazantes { get; set; }
    }
}
