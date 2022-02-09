#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kiosko.Models;
using Microsoft.Data.SqlClient;

namespace kiosko.Controllers
{
    public class ComponentesController : Controller
    {
        private readonly KioskoCmsContext _context;

        public ComponentesController(KioskoCmsContext context)
        {
            _context = context;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var componente = await _context.Componentes.FindAsync(id);
            _context.Componentes.Remove(componente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponenteExists(int id)
        {
            return _context.Componentes.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult GetAllComponentsForModulo(string? padre)
        {
            if (padre == null)
            {
                return Json(null);
            }

            var componente = _context.Componentes.Where(c => c.Padre == padre).OrderBy(c => c.Orden);
            return Json(componente);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult saveComponentForModulo(Componente componente)
        {
            IActionResult ret = null;
                _context.Add(componente);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status201Created, componente);
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult updateComponentForModulo(Componente componente)
        {
            IActionResult ret = null;
            _context.Update(componente);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, componente);
            return ret;
        }
    }
}
