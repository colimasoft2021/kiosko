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
using Microsoft.AspNetCore.Authorization;

namespace kiosko.Controllers
{
    [Authorize]
    public class ComponentesController : Controller
    {
        private readonly KioskoCmsContext _context;
        private readonly IWebHostEnvironment _env;

        public ComponentesController(KioskoCmsContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
        public JsonResult GetAllComponentsForModulo(int? idModulo)
        {
            if (idModulo == null)
            {
                return Json(null);
            }

            var componente = _context.Componentes.Where(c => c.IdModulo == idModulo).OrderBy(c => c.Orden);
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

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult saveComponentForModulo2()
        {
            Console.WriteLine(Request.Form);
            var componente = new Componente();
            componente.Id = Int32.Parse(Request.Form["Id"]);
            componente.Padre = Request.Form["Padre"];
            componente.TipoComponente = Request.Form["TipoComponente"];
            componente.Url = Request.Form["Url"];
            componente.Descripcion = Request.Form["Descripcion"];
            componente.BackgroundColor = Request.Form["BackgroundColor"];
            componente.AgregarFondo = Int32.Parse(Request.Form["AgregarFondo"]);
            componente.Titulo = Request.Form["Titulo"];
            componente.Subtitulo = Request.Form["Subtitulo"];
            componente.Orden = Int32.Parse(Request.Form["Orden"]);
            componente.IdModulo = Int32.Parse(Request.Form["IdModulo"]);

            foreach (var formFile in Request.Form.Files)
            {
                var fulPath = Path.Combine(_env.ContentRootPath, "wwwroot\\files", formFile.FileName);
                using (FileStream fs = System.IO.File.Create(fulPath))
                {
                    formFile.CopyTo(fs);
                    fs.Flush();
                }
            }

            IActionResult ret = null;
            _context.Add(componente);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, componente);
            return ret;
        }
    }
}
