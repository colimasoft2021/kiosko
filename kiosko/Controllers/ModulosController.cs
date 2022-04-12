#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kiosko.Models;
using Microsoft.AspNetCore.Authorization;
using kiosko.Helpers;
using System.Text.RegularExpressions;
using System.Text;

namespace kiosko.Controllers
{
    public class ModulosController : Controller
    {
        private readonly KioskoCmsContext _context;
        MailService _mailService;
        AuthorizationService _authorizationService;

        public ModulosController(KioskoCmsContext context, MailService mailService, AuthorizationService authorizationService)
        {
            _context = context;
            _mailService = mailService;
            _authorizationService = authorizationService;
        }

        // GET: Modulos
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Modulos.ToListAsync());
        }

        [Authorize]
        // GET: Modulos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.Modulos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public IActionResult GetAllModulos()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try { 
                var modulos = _context.Modulos.Where(m => m.Id > 1).OrderBy(m => m.Orden);
                ret = StatusCode(StatusCodes.Status200OK, modulos);
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        [HttpPost()]
        public IActionResult GetModulosAndComponentsForApp([FromBody] Usuario usuario)
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }

            var exist = UsuarioExists(usuario.Id);

            if (!exist) {
                message = new { status = "error", message = "No existe el usuario" };
                return StatusCode(StatusCodes.Status200OK, message);
            }

            try { 
                var modulos = _context.Modulos.OrderBy(c => c.Orden)
                    .Include(m => m.Componentes)
                        .ThenInclude(m => m.Desplazantes)
                        .ToList();
                var dataModulos = new DataModulos();
                foreach (var modulo in modulos)
                {
                    var progresos = _context.Progresos.Where(p => p.IdModulo == modulo.Id).Where(p => p.IdUsuario == usuario.Id).FirstOrDefault();
                    var dataModulo = new CustomModulo();
                    dataModulo.Id = modulo.Id;
                    dataModulo.Titulo = modulo.Titulo;
                    dataModulo.AccesoDirecto = modulo.AccesoDirecto;
                    dataModulo.Orden = modulo.Orden;
                    dataModulo.Desplegable = modulo.Desplegable;
                    dataModulo.IdModulo = modulo.IdModulo;
                    dataModulo.Padre = modulo.Padre;
                    dataModulo.TiempoInactividad = modulo.TiempoInactividad;
                    dataModulo.Componentes = modulo.Componentes;
                    if (modulo.Padre == null)
                    {
                    
                        dataModulo.IdProgreso = progresos.Id;
                        dataModulo.Porcentaje = progresos.Porcentaje;
                        dataModulos.CustomModulos.Add(dataModulo);
                    }
                    else
                    {
                        foreach (var custom in dataModulos.CustomModulos)
                        {
                            if (custom.IdModulo == modulo.Padre)
                            {
                                custom.Submodulos.Add(dataModulo);
                            }
                            else
                            {
                                foreach(var sub in custom.Submodulos)
                                {
                                    if (sub.IdModulo == modulo.Padre)
                                    {
                                        sub.Submodulos.Add(dataModulo);
                                    }
                                }
                            }
                        }
                    }
                }

                ret = StatusCode(StatusCodes.Status200OK, dataModulos);
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        [HttpPost()]
        public IActionResult SaveMenuModulo(Modulo modulo)
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try { 
                modulo.TiempoInactividad = 5;
                _context.Add(modulo);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status201Created, modulo);
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateModulo()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try {
                var idModulo = Int32.Parse(Request.Form["idModulo"]);
                var modulo = await _context.Modulos
                    .FirstOrDefaultAsync(m => m.Id == idModulo);
                modulo.Titulo = Request.Form["tituloModulo"];
                modulo.TiempoInactividad = Int32.Parse(Request.Form["tiempoInactividad"]);

                _context.Update(modulo);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status200OK, modulo);
            
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult deleteModulo(int id)
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try
            {
                var modulo = _context.Modulos.Find(id);
                _context.Modulos.Remove(modulo);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status201Created, modulo);
            
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        public IActionResult EnviarAlertas()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            try
            {
                var usuarios = _context.Usuarios
                .Include(u => u.Progresos)
                .ThenInclude(p => p.IdModuloNavigation).AsNoTracking();

                var alertas = new DataUsuario();

                foreach (var user in usuarios)
                {
                    var infoUsuario = new UsuarioAlerta();
                    infoUsuario.Nombre = user.NombreUsuario;
                    foreach (var p in user.Progresos.ToList())
                    {
                        var inactividadModulo = p.IdModuloNavigation.TiempoInactividad;
                        DateTime fechaHoy = DateTime.Now;
                        var fechaActualizacion = p.FechaActualizacion;
                        TimeSpan diferenciaDias = fechaHoy.Subtract((DateTime)fechaActualizacion);
                        var inactividad = diferenciaDias.Days;
                        if (inactividad > inactividadModulo)
                        {
                            var infoModulo = new ModuloInactivo();
                            infoModulo.Modulo = p.IdModuloNavigation.Titulo;
                            infoModulo.Porcentaje = p.Porcentaje;
                            infoModulo.TiempoInactividad = inactividad;
                            infoUsuario.ModulosInactivos.Add(infoModulo);
                            alertas.UsuariosAlertas.Add(infoUsuario);
                        }
                    }
                }
                string cuerpoMensaje = "<h2>Los siguientes usuarios no han retomado su capacitacion</h2><br/><br/>";
                foreach (var usuario in alertas.UsuariosAlertas)
                {
                    cuerpoMensaje += "<div style='height: auto; border: 1px solid blue; padding: 10px 10px 10px 10px; margin-bottom: 20px;'>";
                    cuerpoMensaje += "<h4 style='margin-bottom: 10px;'>";
                    cuerpoMensaje += usuario.Nombre;
                    cuerpoMensaje += "</h4>";
                    foreach (var modulo in usuario.ModulosInactivos)
                    {
                        cuerpoMensaje += "<h5>Modulo Inactivo: " + modulo.Modulo + "</h5>";
                        cuerpoMensaje += "<ul>";
                        cuerpoMensaje += "<li>Porcentaje de avance: " + modulo.Porcentaje + "%</li>";
                        cuerpoMensaje += "<li>Tiempo de inactividad: " + modulo.TiempoInactividad + " días</li>";
                        cuerpoMensaje += "</ul>";
                    }
                    cuerpoMensaje += "</div>";
                }
                try
                {
                    _mailService.SendEmailGmail("juan.rivera@colimasoft.com", "Alerta de capacitación", cuerpoMensaje);
                    message = new { status = "ok", message = "Email enviado" };
                    ret = StatusCode(StatusCodes.Status200OK, message);
                }
                catch (Exception ex)
                {
                    message = new { status = "error", message = ex.Message };
                    ret = StatusCode(StatusCodes.Status500InternalServerError, message);
                }
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return ret;
        }

        private bool UsuarioExists(int IdUsuario)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == IdUsuario);
        }

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

    public class CustomModulo {
        public CustomModulo()
        {
            Componentes = new HashSet<Componente>();
            Submodulos = new HashSet<CustomModulo>();
        }

        public int Id { get; set; }
        public int IdProgreso { get; set; }
        public double? Porcentaje { get; set; }
        public string? Titulo { get; set; }
        public int? AccesoDirecto { get; set; }
        public int? Orden { get; set; }
        public int? Desplegable { get; set; }
        public string? IdModulo { get; set; }
        public string? Padre { get; set; }
        public int? TiempoInactividad { get; set; }


        public virtual ICollection<Componente> Componentes { get; set; }
        public virtual ICollection<CustomModulo> Submodulos { get; set; }
    }

}
