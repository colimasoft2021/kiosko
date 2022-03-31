#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kiosko.Models;

namespace kiosko.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly KioskoCmsContext _context;

        public UsuariosController(KioskoCmsContext context)
        {
            _context = context;
        }

        private bool UsuarioExists(int IdUsuario)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == IdUsuario);
        }

        [HttpPost()]
        public IActionResult saveNewUser([FromBody] Usuario usuario)
        {
            Console.WriteLine(usuario);
            IActionResult ret = null;

            if (!UsuarioExists(usuario.IdUsuario))
            {
                _context.Add(usuario);
                _context.SaveChanges();
                ret = StatusCode(StatusCodes.Status201Created, usuario);
            }
            else
            {
                var result = _context.Usuarios.Where(u => u.IdUsuario == usuario.IdUsuario);
                ret = StatusCode(StatusCodes.Status201Created, result);
            }



            var modulos = _context.Modulos.Where(m => m.Padre == null);

            DateTime fechaHoy = DateTime.Now;

            foreach (var m in modulos)
            {
                var progreso = new Progreso();
                progreso.IdUsuario = usuario.IdUsuario;
                progreso.IdModulo = m.Id;
                progreso.Finalizado = false;
                progreso.FechaInicio = fechaHoy;
                progreso.Porcentaje = 0;
                if (!ProgresoExists(usuario.IdUsuario, m.Id))
                {
                    _context.Add(progreso);
                }
            }
            _context.SaveChanges();
            return ret;
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
            IActionResult ret = null;
            var updateProgreso = _context.Progresos.Where(p => p.IdModulo == progreso.IdModulo).Where(p => p.IdUsuario == progreso.IdUsuario).FirstOrDefault();
            updateProgreso.Porcentaje = progreso.Porcentaje;
            _context.Update(updateProgreso);
            _context.SaveChanges();
            ret = StatusCode(StatusCodes.Status201Created, progreso);
            return ret;
        }
        /*
        public IActionResult CheckProgress()
        {
            IActionResult ret = null;
            var emailGerente = "jriveraj3@gmail.com";
            var updateProgreso = _context.Progresos.Where(p => p.IdModulo == progreso.IdModulo).Where(p => p.IdUsuario == progreso.IdUsuario).FirstOrDefault();
            updateProgreso.Porcentaje = progreso.Porcentaje;
            _context.Update(updateProgreso);
            _context.SaveChanges();
            ret = StatusCode(StatusCodes.Status201Created, progreso);
            return ret;
        }
        */

    }
}
