using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Progreso
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdModulo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool? Finalizado { get; set; }
        public double? Porcentaje { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public virtual Modulo IdModuloNavigation { get; set; } = null!;
    }
}
