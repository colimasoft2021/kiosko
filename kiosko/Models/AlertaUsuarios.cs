namespace kiosko.Models
{
    public class AlertaUsuarios
    {
    }
    public class ComisionistasApi
    {
        public ComisionistasApi()
        {
            empleados = new HashSet<ArrayEmpleado>();
        }
        public string correo { get; set; }
        public string tienda { get; set; }
        public string iD_Comisionista { get; set; }
        public string nombre_Comisionista { get; set; }
        public virtual ICollection<ArrayEmpleado> empleados { get; set; }
    }
    public class ArrayEmpleado
    {
        public int id_Empleado { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }
    public class Comisionistas
    {
        public Comisionistas()
        {
            DataComisionistas = new HashSet<DataComisionista>();
        }
        public virtual ICollection<DataComisionista> DataComisionistas { get; set; }
    }
    public class DataComisionista
    {
        public DataComisionista()
        {
            Empleados = new HashSet<UsuarioAlerta>();
        }
        public string EmailComisionista { get; set; }
        public string NombreComisionista { set; get; }
        public virtual ICollection<UsuarioAlerta> Empleados { get; set; }

    }

    public class DataUsuario
    {
        public DataUsuario()
        {
            UsuariosAlertas = new HashSet<UsuarioAlerta>();
        }
        public virtual ICollection<UsuarioAlerta> UsuariosAlertas { get; set; }

    }
    public class UsuarioAlerta
    {
        public UsuarioAlerta()
        {
            ModulosInactivos = new HashSet<ModuloInactivo>();
        }
        public string Nombre { get; set; }
        public int IdUsuarioKiosko { get; set; }
        public virtual ICollection<ModuloInactivo> ModulosInactivos { get; set; }
    }

    public class ModuloInactivo
    {
        public string Modulo { get; set; }
        public double? Porcentaje { get; set; }
        public int TiempoInactividad { get; set; }
    }

    public class DataModulos
    {
        public DataModulos()
        {
            CustomModulos = new HashSet<CustomModulo>();
        }
        public virtual ICollection<CustomModulo> CustomModulos { get; set; }
    }

    public class CustomModulo
    {
        public CustomModulo()
        {
            Componentes = new HashSet<Componente>();
            Submodulos = new HashSet<CustomModulo>();
        }

        public int Id { get; set; }
        public int IdProgreso { get; set; }
        public double? Porcentaje { get; set; }
        public bool? finalizado { get; set; }
        public string? Titulo { get; set; }
        public int? AccesoDirecto { get; set; }
        public int? Orden { get; set; }
        public int? Desplegable { get; set; }
        public string? IdModulo { get; set; }
        public string? Padre { get; set; }
        public int? TiempoInactividad { get; set; }
        public int? NumeroHijos { get; set; }
        public string? Url { get; set; }
        public string? UrlFondo { get; set; }
        public string? BackgroundColor { get; set; }

        public virtual ICollection<Componente> Componentes { get; set; }
        public virtual ICollection<CustomModulo> Submodulos { get; set; }
    }
}
