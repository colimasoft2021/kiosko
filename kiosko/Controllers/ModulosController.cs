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
using System.Net;
using Newtonsoft.Json;

namespace kiosko.Controllers
{
    public class ModulosController : Controller
    {
        private readonly KioskoCmsContext _context;
        MailService _mailService;
        AuthorizationService _authorizationService;
        ErrorService _errorService;
        private readonly IWebHostEnvironment _env;

        public ModulosController(KioskoCmsContext context, MailService mailService, AuthorizationService authorizationService, ErrorService errorService,
            IWebHostEnvironment env)
        {
            _context = context;
            _mailService = mailService;
            _authorizationService = authorizationService;
            _errorService = errorService;
            _env = env;

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
                var modulos = _context.Modulos.Where(m => m.Id > 2).OrderBy(m => m.Orden);
                ret = StatusCode(StatusCodes.Status200OK, modulos);
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Modulos", "ModulosController", "GetAllModulos", ex.Message);
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
                _errorService.SaveErrorMessage("_authorizationService.CheckAuthorization", "ModulosController", 
                    "GetModulosAndComponentsForApp", "Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }

            var exist = UsuarioExists(usuario.Id);

            if (!exist) {
                message = new { status = "error", message = "No existe el usuario" };
                return StatusCode(StatusCodes.Status200OK, message);
            }

            try { 
                var modulos = _context.Modulos.Where(c => c.Id > 3).OrderBy(c => c.Orden)
                    .Include(m => m.Componentes.OrderBy(m => m.Orden))
                        .ThenInclude(m => m.Desplazantes)
                        .ToList();
                var dataModulos = new DataModulos();
                var numeroHijos = 0;
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
                    dataModulo.NumeroHijos = 0;
                    dataModulo.Url = modulo.Url;
                    dataModulo.UrlFondo = modulo.UrlFondo;
                    dataModulo.BackgroundColor = modulo.BackgroundColor;
                    if (modulo.Padre == null)
                    {
                    
                        dataModulo.IdProgreso = progresos.Id;
                        dataModulo.Porcentaje = progresos.Porcentaje;
                        dataModulo.finalizado = progresos.Finalizado;
                        dataModulos.CustomModulos.Add(dataModulo);
                        dataModulo.NumeroHijos = 0;
                    }
                    else
                    {
                        foreach (var custom in dataModulos.CustomModulos)
                        {
                            if (custom.IdModulo == modulo.Padre)
                            {
                                custom.Submodulos.Add(dataModulo);
                                if(modulo.Desplegable == 0)
                                {
                                    var totalHijos = custom.NumeroHijos;
                                    totalHijos++;
                                    custom.NumeroHijos = totalHijos;
                                }
                            }
                            else
                            {
                                foreach(var sub in custom.Submodulos)
                                {
                                    if (sub.IdModulo == modulo.Padre)
                                    {
                                        sub.Submodulos.Add(dataModulo);
                                        if (modulo.Desplegable == 0)
                                        {
                                            var totalHijos = custom.NumeroHijos;
                                            totalHijos++;
                                            custom.NumeroHijos = totalHijos;
                                        }
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
                _errorService.SaveErrorMessage("_context.Modulos", "ModulosController", "GetModulosAndComponentsForApp", ex.Message);
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        [HttpGet]
        public IActionResult MessagesInitialsForApp()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try
            {
                List<AvisoInicial> avisos = new List<AvisoInicial>();
                AvisoInicial mensajeInicial = new AvisoInicial();
                var avisosIniciales = _context.Componentes.Where(c => c.Padre == "modulo2").OrderBy(c => c.Orden).ToList();
                foreach (var mensaje in avisosIniciales)
                {
                    mensajeInicial.Id = mensaje.Id;
                    mensajeInicial.tipoComponente = mensaje.TipoComponente;
                    mensajeInicial.url = mensaje.Url;
                    mensajeInicial.descripcion = mensaje.Descripcion;

                    avisos.Add(mensajeInicial);
                }
                ret = StatusCode(StatusCodes.Status200OK, avisos);
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Modulos", "ModulosController", "GetAllModulos", ex.Message);
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        public IActionResult GetModulosGuiasRapidas()
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
                _errorService.SaveErrorMessage("_authorizationService.CheckAuthorization", "ModulosController",
                    "GetModulosAndComponentsForApp", "Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }

            try
            {
                var modulos = _context.Modulos.Where(c => c.Favorito == true).OrderBy(c => c.Orden)
                    .Include(m => m.Componentes)
                        .ThenInclude(m => m.Desplazantes)
                        .ToList();

                ret = StatusCode(StatusCodes.Status200OK, modulos);
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Modulos", "ModulosController", "GetAllModulosGuiasRapidas", ex.Message);
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }


        [HttpPost()]
        public IActionResult SaveMenuModulo()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try {
                var modulo = new Modulo();
                modulo.TiempoInactividad = 5;
                modulo.Id = Int32.Parse(Request.Form["Id"]);
                modulo.Titulo = Request.Form["Titulo"];
                modulo.AccesoDirecto = Int32.Parse(Request.Form["AccesoDirecto"]);
                modulo.Orden = Int32.Parse(Request.Form["Orden"]);
                modulo.Desplegable = Int32.Parse(Request.Form["Desplegable"]);
                modulo.IdModulo = Request.Form["IdModulo"];
                modulo.Padre = Request.Form["Padre"];
                modulo.Url = Request.Form["Url"];   
                modulo.Favorito = Convert.ToBoolean(Request.Form["Favorito"]);
                modulo.TipoGuia = Request.Form["TipoGuia"];
                modulo.UrlFondo = Request.Form["UrlFondo"];
                modulo.BackgroundColor = Request.Form["BackgroundColor"];
                if (modulo.TipoGuia == "null" || modulo.TipoGuia == null)
                    modulo.TipoGuia = "";
                if (modulo.Padre == "undefined")
                    modulo.Padre = null;
                if(modulo.Padre != null && modulo.Padre != "null")
                {
                    var colorPadre = _context.Modulos.Where(mod => mod.IdModulo == modulo.Padre).Select(mod => mod.BackgroundColor).FirstOrDefault();
                    modulo.BackgroundColor = colorPadre;
                }
                foreach (var formFile in Request.Form.Files)
                {
                    var fulPath = Path.Combine(_env.ContentRootPath, "wwwroot\\files", formFile.FileName);
                    using (FileStream fs = System.IO.File.Create(fulPath))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }
                }
                _context.Add(modulo);
                _context.SaveChanges();

                if (modulo.Padre == null)
                {
                    DateTime fechaHoy = DateTime.Now;
                    var usuarios = _context.Usuarios.AsNoTracking();
                    foreach (var usuario in usuarios)
                    {
                        var progreso = new Progreso();
                        progreso.IdUsuario = usuario.Id;
                        progreso.IdModulo = modulo.Id;
                        progreso.Finalizado = false;
                        progreso.FechaInicio = fechaHoy;
                        progreso.Porcentaje = 0;
                        progreso.FechaActualizacion = fechaHoy;
                        _context.Add(progreso);
                    }
                    _context.SaveChanges();
                }

                ret = StatusCode(StatusCodes.Status201Created, modulo);
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Add", "ModulosController", 
                    "SaveMenuModulo", ex.Message);
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
                modulo.Favorito = Convert.ToBoolean(Request.Form["Favorito"]);
                modulo.TipoGuia = Request.Form["TipoGuia"];
                modulo.UrlFondo = Request.Form["UrlFondo"];
                modulo.Url = Request.Form["Url"];
                modulo.BackgroundColor = Request.Form["BackgroundColor"];
                if (modulo.TipoGuia == "undefined" || modulo.TipoGuia == null)
                    modulo.TipoGuia = "";
                if (modulo.Padre != null && modulo.Padre != "null")
                {
                    var colorPadre = _context.Modulos.Where(mod => mod.IdModulo == modulo.Padre).Select(mod => mod.BackgroundColor).FirstOrDefault();
                    modulo.BackgroundColor = colorPadre;
                }
                foreach (var formFile in Request.Form.Files)
                {
                    var fulPath = Path.Combine(_env.ContentRootPath, "wwwroot\\files", formFile.FileName);
                    using (FileStream fs = System.IO.File.Create(fulPath))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }
                }

                _context.Update(modulo);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status200OK, modulo);
            
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Modulos", "ModulosController", 
                    "updateModulo", ex.Message);
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
                var modulo = _context.Modulos.Where(x => x.Id == id)
                    .Include(m => m.Progresos)
                    .Include(m => m.Componentes)
                        .ThenInclude(m => m.Desplazantes).FirstOrDefault();
                if (modulo.Padre == null || modulo.Padre == "null")
                {
                    var modulosHijos = _context.Modulos.Where(x => x.Padre == modulo.IdModulo).ToList();
                    foreach (var submodul in modulosHijos)
                    {
                        var submodulo = _context.Modulos.Where(x => x.Id == submodul.Id)
                            .Include(m => m.Componentes.OrderBy(m => m.Orden))
                                .ThenInclude(m => m.Desplazantes).FirstOrDefault();
                        _context.Remove(submodulo);
                        _context.SaveChanges();
                    }
                    var modPadre = _context.Modulos.Find(id);
                    var progresos = _context.Progresos.Where(p => p.IdModulo == id).ToList();
                    foreach (var progres in progresos)
                    {
                        var progreso = _context.Progresos.Find(progres.Id);
                        _context.Progresos.Remove(progreso);
                        _context.SaveChanges();
                    }
                    _context.Remove(modPadre);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Remove(modulo);
                    _context.SaveChanges();
                }

                ret = StatusCode(StatusCodes.Status201Created, modulo);
            
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Modulos.Find", "ModulosController", 
                    "deleteModulo", ex.Message);
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        public IActionResult SendAlertasEmailComisionistas()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                _errorService.SaveErrorMessage("!Request.Headers.ContainsKey", "ModulosController",
                    "SendAlertas", "Faltan Headers Auth - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                _errorService.SaveErrorMessage("_authorizationService.CheckAuthorization", "ModulosController",
                    "SendAlertas", "Credenciales Incorrectas - Unauthorized/Sin Autorizacion");
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
                    infoUsuario.IdUsuarioKiosko = user.IdUsuario;
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
                
                var comisionistas = GetComisionistas();
                string stringComisionistas = JsonConvert.SerializeObject(comisionistas.Value);
                var enumComisionistas = JsonConvert.DeserializeObject<IEnumerable<ComisionistasApi>>(stringComisionistas);

                var arrayComisionistas = new Comisionistas();
                foreach (var com in enumComisionistas)
                {
                    var infoComisionista = new DataComisionista();
                    foreach (var emp in com.empleados)
                    {
                        int matches = 0;
                        foreach (var user in alertas.UsuariosAlertas)
                        {
                            if (emp.id_Empleado == user.IdUsuarioKiosko)
                            {
                                user.Nombre = emp.nombre + " " + emp.apellidos;
                                infoComisionista.Empleados.Add(user);
                                matches++;
                            }
                        }
                        if (matches > 0)
                        {
                            infoComisionista.EmailComisionista = com.correo;
                            infoComisionista.NombreComisionista = com.nombre_Comisionista;
                            arrayComisionistas.DataComisionistas.Add(infoComisionista);
                        }
                    }
                }


                string cuerpoMensaje = "";
                foreach (var com in arrayComisionistas.DataComisionistas)
                {
                    com.NombreComisionista = "";
                    cuerpoMensaje += "<h2>Hola " + com.NombreComisionista + " Los siguientes usuarios no han retomado su capacitacion</h2><br/><br/>";
                    foreach (var emp in com.Empleados)
                    {
                        cuerpoMensaje += "<div style='height: auto; border: 1px solid blue; padding: 10px 10px 10px 10px; margin-bottom: 20px;'>";
                        cuerpoMensaje += "<h4 style='margin-bottom: 10px;'>";
                        cuerpoMensaje += emp.Nombre;
                        cuerpoMensaje += "</h4>";
                        foreach (var modulo in emp.ModulosInactivos)
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
                    }
                    catch (Exception ex)
                    {
                        _errorService.SaveErrorMessage("_mailService.SendEmailGmail", "ModulosController",
                            "SendAlertas", ex.Message);
                        message = new { status = "error", message = ex.Message };
                        ret = StatusCode(StatusCodes.Status500InternalServerError, message);
                        throw ex;
                    }
                    cuerpoMensaje = "";
                }
                message = new { status = "ok", message = "Correo(s) enviado(s)" };
                ret = StatusCode(StatusCodes.Status200OK, alertas);
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Usuarios - alertas = new DataUsuario()", 
                    "ModulosController", "SendAlertas", ex.Message);
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return ret;
        }
        
        [HttpPost]
        public IActionResult SendAlertasLocales([FromBody] Usuario usuario)
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                _errorService.SaveErrorMessage("!Request.Headers.ContainsKey", "ModulosController",
                    "SendAlertas", "Faltan Headers Auth - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                _errorService.SaveErrorMessage("_authorizationService.CheckAuthorization", "ModulosController",
                    "SendAlertas", "Credenciales Incorrectas - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            try
            {
                var usuarios = _context.Usuarios.Where(u => u.IdUsuario == usuario.Id)
                .Include(u => u.Progresos.Where(p => p.Porcentaje != 100).Where(p => p.Porcentaje != null))
                .ThenInclude(p => p.IdModuloNavigation).AsNoTracking().FirstOrDefault();

                var alertas = new DataUsuario();

                var infoUsuario = new UsuarioAlerta();
                infoUsuario.Nombre = usuarios.NombreUsuario;
                infoUsuario.IdUsuarioKiosko = usuarios.IdUsuario;
                foreach (var p in usuarios.Progresos.ToList())
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

                string cuerpoMensaje = "";
                cuerpoMensaje += "Hola " + usuarios.NombreUsuario + " continúa con capacitación<br/>";
                foreach (var modulo in infoUsuario.ModulosInactivos)
                {
                    cuerpoMensaje += "<b>Modulo Inactivo: " + modulo.Modulo + "</b><br/>";
                    cuerpoMensaje += "Porcentaje de avance: " + modulo.Porcentaje + "%<br/>";
                    cuerpoMensaje += "Tiempo de inactividad: " + modulo.TiempoInactividad + " días<br/>";
                }
                try
                {
                    ret = StatusCode(StatusCodes.Status200OK, cuerpoMensaje);
                }
                catch (Exception ex)
                {
                    _errorService.SaveErrorMessage("_mailService.SendEmailGmail", "ModulosController",
                        "SendAlertas", ex.Message);
                    message = new { status = "error", message = ex.Message };
                    ret = StatusCode(StatusCodes.Status500InternalServerError, message);
                    throw ex;
                }
                cuerpoMensaje = "";
                ret = StatusCode(StatusCodes.Status200OK, alertas);
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("_context.Usuarios - alertas = new DataUsuario()",
                    "ModulosController", "SendAlertas", ex.Message);
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return ret;
        }
        private bool UsuarioExists(int IdUsuario)
        {
            return _context.Usuarios.Any(e => e.Id == IdUsuario);
        }
        public JsonResult GetComisionistas()
        {
            var enpointUrl = "https://accesossicom.mikiosko.mx/api/Usuarios/getcomisionistas";
            var username = "SicomAcess";
            var password = "$1c0om007";
            List<ComisionistasApi> dataComisionistas = new List<ComisionistasApi>();
            try
            {
                string authEncoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                    .GetBytes(username + ":" + password));
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(enpointUrl);
                httpWebRequest.Headers.Add("Authorization", "Basic " + authEncoded);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dataComisionistas = JsonConvert.DeserializeObject<List<ComisionistasApi>>(result);
                }
                return Json(dataComisionistas);
            }
            catch (Exception ex)
            {
                var message = new { status = "error", message = ex.Message };
                return Json(message);
            }
        }
    }
}