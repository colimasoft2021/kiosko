﻿using System;
using System.Collections.Generic;

namespace kiosko.Models
{
    public partial class Modulo
    {
        public Modulo()
        {
            Componentes = new HashSet<Componente>();
        }

        public int Id { get; set; }
        public string? Titulo { get; set; }
        public int? AccesoDirecto { get; set; }
        public int? Orden { get; set; }
        public int? Desplegable { get; set; }
        public string? IdModulo { get; set; }
        public string? Padre { get; set; }
        public int? TiempoInactividad { get; set; }

        public virtual ICollection<Componente> Componentes { get; set; }
    }
}
