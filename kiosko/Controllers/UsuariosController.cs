#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kiosko.Models;
using kiosko.Helpers;

namespace kiosko.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly KioskoCmsContext _context;
        AuthorizationService _authorizationService;

        public UsuariosController(KioskoCmsContext context, AuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        private bool UsuarioExists(int IdUsuario)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == IdUsuario);
        }

        [HttpPost()]
        public IActionResult saveNewUser([FromBody] Usuario usuario)
        {
            var error = new { message = "Unauthorized" };
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, error);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, error);
            }
            try { 
                IActionResult ret = null;

                if (!UsuarioExists(usuario.IdUsuario))
                {
                    _context.Add(usuario);
                    _context.SaveChanges();
                }

                var dataUsuario = _context.Usuarios.Where(u => u.IdUsuario == usuario.IdUsuario).AsNoTracking().FirstOrDefault();

                var modulos = _context.Modulos.Where(m => m.Padre == null);

                DateTime fechaHoy = DateTime.Now;

                foreach (var m in modulos)
                {
                    var progreso = new Progreso();
                    progreso.IdUsuario = dataUsuario.Id;
                    progreso.IdModulo = m.Id;
                    progreso.Finalizado = false;
                    progreso.FechaInicio = fechaHoy;
                    progreso.Porcentaje = 0;
                    progreso.FechaActualizacion = fechaHoy;
                    if (!ProgresoExists(dataUsuario.Id, m.Id))
                    {
                        _context.Add(progreso);
                    }
                }
                _context.SaveChanges();
                ret = StatusCode(StatusCodes.Status201Created, dataUsuario);
                return ret;
            }
            catch (Exception ex)
            {
                error = new { message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }

        private bool ProgresoExists(int IdUsuario, int Id)
        {
            var progresos = _context.Progresos.Where(p => p.IdModulo == Id).Where(p => p.IdUsuario == IdUsuario);
            var numberElements = progresos.Count();
            if (numberElements > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost()]
        public IActionResult UpdateProgress([FromBody] Progreso progreso)
        {
            var error = new { message = "Unauthorized" };
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, error);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, error);
            }
            try
            {
                IActionResult ret = null;
                DateTime fechaHoy = DateTime.Now;
                var updateProgreso = _context.Progresos.Where(p => p.IdModulo == progreso.IdModulo).Where(p => p.IdUsuario == progreso.IdUsuario).FirstOrDefault();
                updateProgreso.Porcentaje = progreso.Porcentaje;
                updateProgreso.FechaActualizacion = fechaHoy;
                if (progreso.Porcentaje == 100)
                {
                    updateProgreso.FechaFin = fechaHoy;
                    updateProgreso.Finalizado = true;
                }
                _context.Update(updateProgreso);
                ret = StatusCode(StatusCodes.Status200OK, updateProgreso);
                return ret;
            }
            catch (Exception ex)
            {
                error = new { message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }

    }
}
