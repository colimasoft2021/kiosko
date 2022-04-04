﻿#nullable disable
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

namespace kiosko.Controllers
{
    public class ModulosController : Controller
    {
        private readonly KioskoCmsContext _context;
        MailService _mailService;

        public ModulosController(KioskoCmsContext context, MailService mailService)
        {
            _context = context;
            _mailService = mailService;
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

        public JsonResult GetAllModulos()
        {
            var modulos = _context.Modulos.Where(m => m.Id > 1).OrderBy(m => m.Orden);
            return Json(modulos);
        }
        // GET: Modulos/GetModulosComponentsForApp
        public JsonResult GetModulosAndComponentsForApp()
        {
            var modulos = _context.Modulos.OrderBy(c => c.Orden)
                .Include(m => m.Componentes)
                    .ThenInclude(m => m.Desplazantes)
                    .ToList();
            return Json(modulos);
        }

        [HttpPost()]
        public IActionResult SaveMenuModulo(Modulo modulo)
        {
            IActionResult ret = null;
            modulo.TiempoInactividad = 5;
            _context.Add(modulo);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, modulo);
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateModulo()
        {
            var idModulo = Int32.Parse(Request.Form["idModulo"]);
            var modulo = await _context.Modulos
                .FirstOrDefaultAsync(m => m.Id == idModulo);
            modulo.Titulo = Request.Form["tituloModulo"];
            modulo.TiempoInactividad = Int32.Parse(Request.Form["tiempoInactividad"]);

            IActionResult ret = null;
            _context.Update(modulo);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, modulo);
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult deleteModulo(int id)
        {
            var modulo = _context.Modulos.Find(id);
            IActionResult ret = null;
            _context.Modulos.Remove(modulo);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, modulo);
            return ret;
        }

        [HttpPost()]
        public IActionResult GetModulosAndProgressByUser([FromBody] Usuario usuario)
        {
            var modulos = _context.Modulos;
            foreach (var m in modulos)
            {
                _context.Progresos.Where(p => p.IdModulo == m.Id).Where(p => p.IdUsuario == usuario.IdUsuario).Load();
            }
            return Json(modulos);
        }

        public IActionResult EnviarAlertas()
        {
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
                _mailService.SendEmailGmail("juan.rivera@colimasoft.com", "Alerta de capacitación", cuerpoMensaje);
                return Json(alertas);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
    public partial class DataUsuario
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
}
