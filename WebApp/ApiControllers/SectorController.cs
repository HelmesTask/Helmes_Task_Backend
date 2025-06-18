using System.Net;
using App.Contracts.DAL;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;


namespace WebApp.ApiControllers;

/// <summary>
/// API controller for managing sectors.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class SectorController : ControllerBase
{
    private readonly IAppUnitOfWork _uow;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector> _mapper;

    /// <summary>
    /// Constructor for SectorController.
    /// </summary>
    /// <param name="uow">The Unit of Work for interacting with the data access layer.</param>
    /// <param name="autoMapper">The AutoMapper instance for DTO mapping.</param>
    public SectorController(IAppUnitOfWork uow, IMapper autoMapper)
    {
        _uow = uow;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector>(autoMapper);
    }
 
    /// <summary>
    /// Gets all sectors.
    /// </summary>
    /// <returns>An ActionResult containing a list of Sector DTOs.</returns>
    [HttpGet("GetSectors")]
    [ProducesResponseType(typeof(List<App.DTO.v1_0.Sector>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.Sector>>> GetSectors()
    {
        var sectors = (await _uow.SectorRepository.GetAllEntitiesAsync()).ToList();
        return Ok(sectors);
    }

    /// <summary>
    /// Creates a new sector.
    /// </summary>
    /// <param name="sector">The sector DTO to be created.</param>
    /// <returns>An ActionResult indicating BadRequest if the sector data is invalid, or Created if successful.</returns>
    [HttpPost("PostSector")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult> PostSector(App.DTO.v1_0.Sector sector)
    {
        if (sector == null)
        {
            return BadRequest("Invalid sector data provided.");
        }

        sector.Id = Guid.NewGuid();
        
        _uow.SectorRepository.Add(_mapper.Map(sector)!);

        await _uow.SaveChangesAsync();

        return StatusCode((int)HttpStatusCode.Created);
    }
    
    /// <summary>
    /// Updates an existing sector.
    /// </summary>
    /// <param name="id">The ID of the sector to update.</param>
    /// <param name="sector">The updated sector DTO.</param>
    /// <returns>An ActionResult indicating BadRequest if the ID mismatch, or NoContent if successful.</returns>
    [HttpPut("PutSector/{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult> PutSectors(Guid id, App.DTO.v1_0.Sector sector)
    {
        if (id != sector.Id)
        {
            return BadRequest();
        }

        var res = _mapper.Map(sector);

        _uow.SectorRepository.Update(res);

        await _uow.SaveChangesAsync();

        return NoContent();
    }
}