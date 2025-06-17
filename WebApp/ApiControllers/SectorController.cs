using System.Net;
using App.Contracts.DAL;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;


namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class SectorController : ControllerBase
{
    private readonly IAppUnitOfWork _uow;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector> _mapper;

    public SectorController(IAppUnitOfWork uow, IMapper autoMapper)
    {
        _uow = uow;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector>(autoMapper);
    }
 
    [HttpGet("GetSectors")]
    [ProducesResponseType(typeof(List<App.DTO.v1_0.Sector>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.Sector>>> GetSectors()
    {
        var sectors = (await _uow.SectorRepository.GetAllEntitiesAsync(true)).ToList();
        return Ok(sectors);
    }


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

