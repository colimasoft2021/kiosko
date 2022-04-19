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
        ErrorService _errorService;

        public UsuariosController(KioskoCmsContext context, AuthorizationService authorizationService,
            ErrorService errorService)
        {
            _context = context;
            _authorizationService = authorizationService;
            _errorService = errorService;
        }

        private bool UsuarioExists(int IdUsuario)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == IdUsuario);
        }

        [HttpPost()]
        public IActionResult saveNewUser([FromBody] Usuario usuario)
        {
            var message = new { status = "", message = "" };
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                _errorService.SaveErrorMessage("Request.Headers.ContainsKey", "UsuariosController", 
                    "saveNewUser", "Faltan Headers Auth - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                _errorService.SaveErrorMessage("_authorizationService.CheckAuthorization", "UsuariosController",
                    "saveNewUser", "Credenciales Incorrectas - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
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
                _errorService.SaveErrorMessage("Usuario - _context.Add", "UsuariosController", 
                    "saveNewUser", ex.Message);
                message = new { status = "error", message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, message);
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
            var message = new { status = "", message = "" };
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                _errorService.SaveErrorMessage("Request.Headers.ContainsKey", "UsuariosController",
                    "UpdateProgress", "Faltan Headers Auth - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            var paramAuthorization = Request.Headers["Authorization"].ToString();
            var isAuthorized = _authorizationService.CheckAuthorization(paramAuthorization);
            if (!isAuthorized)
            {
                _errorService.SaveErrorMessage("_authorizationService.CheckAuthorization", "UsuariosController",
                    "UpdateProgress", "Credenciales Incorrectas - Unauthorized/Sin Autorizacion");
                message = new { status = "error", message = "Unauthorized" };
                return StatusCode(StatusCodes.Status401Unauthorized, message);
            }
            try
            {
                IActionResult ret = null;
                DateTime fechaHoy = DateTime.Now;
                var updateProgreso = _context.Progresos.Where(p => p.IdModulo == progreso.IdModulo).Where(p => p.IdUsuario == progreso.IdUsuario).FirstOrDefault();
                if(progreso.Porcentaje <= updateProgreso.Porcentaje )
                {
                    message = new { status = "succed", message = "Progreso no actualizado" };
                    return StatusCode(StatusCodes.Status204NoContent, message);
                }
                updateProgreso.Porcentaje = progreso.Porcentaje;
                updateProgreso.FechaActualizacion = fechaHoy;
                if (progreso.Porcentaje == 100)
                {
                    updateProgreso.FechaFin = fechaHoy;
                    updateProgreso.Finalizado = true;
                }
                _context.Update(updateProgreso);
                _context.SaveChanges();
                message = new { status = "ok", message = "Progreso actualizado correctamente" };
                ret = StatusCode(StatusCodes.Status200OK, message);
                return ret;
            }
            catch (Exception ex)
            {
                _errorService.SaveErrorMessage("updateProgreso - _context.Progresos", "UsuariosController",
                    "UpdateProgress", ex.Message);
                message = new { status = "ok", message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
        }

    }
}
