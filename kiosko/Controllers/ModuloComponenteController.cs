#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kiosko.Models;

namespace kiosko.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuloComponenteController : ControllerBase
    {
        private readonly KioskoCmsContext _context;

        public ModuloComponenteController(KioskoCmsContext context)
        {
            _context = context;
        }

        // GET: api/ModuloComponente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Modulo>>> GetModulos()
        {
            var modulos = _context.Modulos.OrderBy(c => c.Orden);
            foreach (var m in modulos)
            {
                _context.Componentes.Where(c => c.IdModulo == m.Id).OrderBy(c => c.Orden).Load();
            }
            return await modulos.ToListAsync();
        }

        // GET: api/ModuloComponente/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Modulo>> GetModulo(int id)
        {
            var modulo = await _context.Modulos.FindAsync(id);

            if (modulo == null)
            {
                return NotFound();
            }

            return modulo;
        }

        // PUT: api/ModuloComponente/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModulo(int id, Modulo modulo)
        {
            if (id != modulo.Id)
            {
                return BadRequest();
            }

            _context.Entry(modulo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuloExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ModuloComponente
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Modulo>> PostModulo(Modulo modulo)
        {
            _context.Modulos.Add(modulo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModulo", new { id = modulo.Id }, modulo);
        }

        // DELETE: api/ModuloComponente/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModulo(int id)
        {
            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo == null)
            {
                return NotFound();
            }

            _context.Modulos.Remove(modulo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModuloExists(int id)
        {
            return _context.Modulos.Any(e => e.Id == id);
        }
    }
}
