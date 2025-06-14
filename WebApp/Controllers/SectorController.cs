using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;

namespace WebApp.Controllers
{
    public class SectorController : Controller
    {
        private readonly AppDbContext _context;

        public SectorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sector
        public async Task<IActionResult> Index()
        {
              return _context.Sectors != null ? 
                          View(await _context.Sectors.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Sectors'  is null.");
        }

        // GET: Sector/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Sectors == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sector == null)
            {
                return NotFound();
            }

            return View(sector);
        }

        // GET: Sector/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sector/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParentSectorId,SectorName,Id")] Sector sector)
        {
            if (ModelState.IsValid)
            {
                sector.Id = Guid.NewGuid();
                _context.Add(sector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sector);
        }

        // GET: Sector/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Sectors == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }
            return View(sector);
        }

        // POST: Sector/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ParentSectorId,SectorName,Id")] Sector sector)
        {
            if (id != sector.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectorExists(sector.Id))
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
            return View(sector);
        }

        // GET: Sector/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Sectors == null)
            {
                return NotFound();
            }

            var sector = await _context.Sectors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sector == null)
            {
                return NotFound();
            }

            return View(sector);
        }

        // POST: Sector/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Sectors == null)
            {
                return Problem("Entity set 'AppDbContext.Sectors'  is null.");
            }
            var sector = await _context.Sectors.FindAsync(id);
            if (sector != null)
            {
                _context.Sectors.Remove(sector);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectorExists(Guid id)
        {
          return (_context.Sectors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
