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
    public async Task<ActionResult<List<App.DTO.v1_0.Sector>>> GetAppUserSectors(Guid sessionId)
    {
        var sectors = _uow.AppUserSectorRepository.GetAllAppUserSectionsAsync(sessionId);
        return Ok();
    }

}