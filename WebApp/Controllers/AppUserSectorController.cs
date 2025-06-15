using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain.User;

namespace WebApp.Controllers
{
    public class AppUserSectorController : Controller
    {
        private readonly AppDbContext _context;

        public AppUserSectorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AppUserSector
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AppUserSectors.Include(a => a.AppUser).Include(a => a.Sector);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AppUserSector/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.AppUserSectors == null)
            {
                return NotFound();
            }

            var appUserSector = await _context.AppUserSectors
                .Include(a => a.AppUser)
                .Include(a => a.Sector)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUserSector == null)
            {
                return NotFound();
            }

            return View(appUserSector);
        }

        // GET: AppUserSector/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "UserName");
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "SectorName");
            return View();
        }

        // POST: AppUserSector/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppUserId,SectorId,Id")] AppUserSector appUserSector)
        {
            if (ModelState.IsValid)
            {
                appUserSector.Id = Guid.NewGuid();
                _context.Add(appUserSector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "UserName", appUserSector.AppUserId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "SectorName", appUserSector.SectorId);
            return View(appUserSector);
        }

        // GET: AppUserSector/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.AppUserSectors == null)
            {
                return NotFound();
            }

            var appUserSector = await _context.AppUserSectors.FindAsync(id);
            if (appUserSector == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "UserName", appUserSector.AppUserId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "SectorName", appUserSector.SectorId);
            return View(appUserSector);
        }

        // POST: AppUserSector/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AppUserId,SectorId,Id")] AppUserSector appUserSector)
        {
            if (id != appUserSector.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUserSector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserSectorExists(appUserSector.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "UserName", appUserSector.AppUserId);
            ViewData["SectorId"] = new SelectList(_context.Sectors, "Id", "SectorName", appUserSector.SectorId);
            return View(appUserSector);
        }

        // GET: AppUserSector/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.AppUserSectors == null)
            {
                return NotFound();
            }

            var appUserSector = await _context.AppUserSectors
                .Include(a => a.AppUser)
                .Include(a => a.Sector)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUserSector == null)
            {
                return NotFound();
            }

            return View(appUserSector);
        }

        // POST: AppUserSector/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.AppUserSectors == null)
            {
                return Problem("Entity set 'AppDbContext.AppUserSectors'  is null.");
            }
            var appUserSector = await _context.AppUserSectors.FindAsync(id);
            if (appUserSector != null)
            {
                _context.AppUserSectors.Remove(appUserSector);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserSectorExists(Guid id)
        {
          return (_context.AppUserSectors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
