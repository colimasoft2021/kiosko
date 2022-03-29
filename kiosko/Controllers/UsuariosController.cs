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

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUsuario,Usuario1,Clave,Rol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuario,NombreUsuario,Clave,Rol")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            if(progresos == null)
            {
                return false;
            }else
            {
                return true;
            }
        }

    }
}
