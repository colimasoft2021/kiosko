﻿#nullable disable
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

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult deleteComponent(int id)
        {
            IActionResult ret = null;
            var message = new { status = "", message = "" };
            try
            {
                var componente = _context.Componentes.Find(id);
                _context.Componentes.Remove(componente);
                _context.SaveChanges();
                message = new { status = "ok", message = "Componente eliminado" };
                ret = StatusCode(StatusCodes.Status200OK, message);
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
        public IActionResult deleteDesplazante(int id)
        {
            IActionResult ret = null;
            var message = new { status = "", message = "" };
            try
            {
                var desplazante = _context.Desplazantes.Find(id);
                _context.Desplazantes.Remove(desplazante);
                _context.SaveChanges();
                message = new { status = "ok", message = "Componente eliminado" };
                ret = StatusCode(StatusCodes.Status200OK, message);
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
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

            var componentes = _context.Componentes.Where(c => c.IdModulo == idModulo).OrderBy(c => c.Orden);

            foreach (var c in componentes)
            {
                _context.Desplazantes.Where(d => d.IdComponente == c.Id).Load();
            }
            return Json(componentes);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult saveComponentForModulo2()
        {
            IActionResult ret = null;
            var message = new { status = "", message = "" };
            try
            {
                var componente = new Componente();
                componente.Id = Int32.Parse(Request.Form["Id"]);
                componente.Padre = Request.Form["Padre"];
                componente.TipoComponente = Request.Form["TipoComponente"];
                componente.Url = Request.Form["Url"];
                componente.UrlDos = Request.Form["UrlDos"];
                componente.UrlTres = Request.Form["UrlTres"];
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

                _context.Add(componente);
                _context.SaveChanges();
                ret = StatusCode(StatusCodes.Status201Created, componente);
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
        public IActionResult updateComponentForModulo2()
        {
            IActionResult ret = null;
            var message = new { status = "", message = "" };
            try
            {
                var componente = new Componente();
                componente.Id = Int32.Parse(Request.Form["Id"]);
                componente.Padre = Request.Form["Padre"];
                componente.TipoComponente = Request.Form["TipoComponente"];
                componente.Url = Request.Form["Url"];
                componente.UrlDos = Request.Form["UrlDos"];
                componente.UrlTres = Request.Form["UrlTres"];
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

                _context.Update(componente);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status200OK, componente);
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
        public IActionResult saveDesplazantes()
        {
            IActionResult ret = null;
            var message = new { status = "", message = "" };
            try
            {
                var desplazante = new Desplazante();
                desplazante.Id = Int32.Parse(Request.Form["Id"]);
                desplazante.Url = Request.Form["Url"];
                desplazante.Titulo = Request.Form["Titulo"];
                desplazante.Texto = Request.Form["Texto"];
                desplazante.IdComponente = Int32.Parse(Request.Form["IdComponente"]);

                foreach (var formFile in Request.Form.Files)
                {
                    var fulPath = Path.Combine(_env.ContentRootPath, "wwwroot\\files", formFile.FileName);
                    using (FileStream fs = System.IO.File.Create(fulPath))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }
                }

                _context.Add(desplazante);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status201Created, desplazante);
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
        public IActionResult updateDesplazantes()
        {
            IActionResult ret = null;
            var message = new { status = "", message = "" };
            try
            {
                var desplazante = new Desplazante();
                desplazante.Id = Int32.Parse(Request.Form["Id"]);
                desplazante.Url = Request.Form["Url"];
                desplazante.Titulo = Request.Form["Titulo"];
                desplazante.Texto = Request.Form["Texto"];
                desplazante.IdComponente = Int32.Parse(Request.Form["IdComponente"]);

                foreach (var formFile in Request.Form.Files)
                {
                    var fulPath = Path.Combine(_env.ContentRootPath, "wwwroot\\files", formFile.FileName);
                    using (FileStream fs = System.IO.File.Create(fulPath))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }
                }

                _context.Update(desplazante);
                _context.SaveChanges();

                ret = StatusCode(StatusCodes.Status201Created, desplazante);
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }
    }
}
