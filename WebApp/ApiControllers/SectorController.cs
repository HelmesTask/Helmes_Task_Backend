using System.Net;
using App.Contracts.DAL;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;


namespace WebApp.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SectorController : ControllerBase
    {
        private readonly IAppUnitOfWork _uow;

        public SectorController(IAppUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/Sector
        [HttpGet("GetSectors")]
        [ProducesResponseType(typeof(List<App.DTO.v1_0.Sector>), (int)HttpStatusCode.OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<List<App.DTO.v1_0.Sector>>> GetSectors()
        {
          var sectors = (await _uow.SectorRepository.GetAllAsync(true)).ToList();
          return Ok(sectors);
        }

        // // GET: api/Sector/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<Sector>> GetSector(Guid id)
        // {
        //   if (_context.Sectors == null)
        //   {
        //       return NotFound();
        //   }
        //     var sector = await _context.Sectors.FindAsync(id);
        //
        //     if (sector == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return sector;
        // }
        //
        // // PUT: api/Sector/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutSector(Guid id, Sector sector)
        // {
        //     if (id != sector.Id)
        //     {
        //         return BadRequest();
        //     }
        //
        //     _context.Entry(sector).State = EntityState.Modified;
        //
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!SectorExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }
        //
        //     return NoContent();
        // }
        //
        // // POST: api/Sector
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<Sector>> PostSector(Sector sector)
        // {
        //   if (_context.Sectors == null)
        //   {
        //       return Problem("Entity set 'AppDbContext.Sectors'  is null.");
        //   }
        //     _context.Sectors.Add(sector);
        //     await _context.SaveChangesAsync();
        //
        //     return CreatedAtAction("GetSector", new { id = sector.Id }, sector);
        // }
        //
        // // DELETE: api/Sector/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteSector(Guid id)
        // {
        //     if (_context.Sectors == null)
        //     {
        //         return NotFound();
        //     }
        //     var sector = await _context.Sectors.FindAsync(id);
        //     if (sector == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _context.Sectors.Remove(sector);
        //     await _context.SaveChangesAsync();
        //
        //     return NoContent();
        // }
        //
        // private bool SectorExists(Guid id)
        // {
        //     return (_context.Sectors?.Any(e => e.Id == id)).GetValueOrDefault();
        // }
    }
}
