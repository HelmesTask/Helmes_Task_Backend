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

public class AppUserSectorController : ControllerBase
{
    private readonly IAppUnitOfWork _uow;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector> _mapper;
    
    public AppUserSectorController(IAppUnitOfWork uow, IMapper autoMapper)
    {
        _uow = uow;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector>(autoMapper);
    }
    
    [HttpGet("GetAppUserSectors/{sessionId}")]
    [ProducesResponseType(typeof(List<App.DTO.v1_0.Sector>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<Guid>>> GetAppUserSectors(Guid sessionId)
    {
        var sectorIds = await _uow.AppUserSectorRepository.GetAllAppUserSectionsAsync(sessionId);
        
        return Ok(sectorIds);
    }
    
    [HttpPut("PutAppUserSectors/{sessionId}")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]    
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult> PutAppUserSectors(List<Guid> sectorIdList)
    {
        foreach (var sector in sectorIdList)
        {
            //check is sector is guid
            if (sector == Guid.Empty)
            {
                return BadRequest("Invalid sector ID provided.");
            }

            
        }
        
        return Ok();
    }

}