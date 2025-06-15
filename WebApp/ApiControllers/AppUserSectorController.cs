using System.Net;
using App.Contracts.DAL;
using App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using AppUser = App.DAL.DTO.AppUser;
using AppUserSector = App.DAL.DTO.AppUserSector;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]

public class AppUserSectorController : ControllerBase
{
    private readonly IAppUnitOfWork _uow;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector> _mapper;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.AppUser, App.DAL.DTO.AppUser> _userMapper;

    
    public AppUserSectorController(IAppUnitOfWork uow, IMapper autoMapper)
    {
        _uow = uow;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Sector, App.DAL.DTO.Sector>(autoMapper);
        _userMapper = new PublicDTOBllMapper<App.DTO.v1_0.AppUser, App.DAL.DTO.AppUser>(autoMapper);
    }
    
    [HttpGet("GetAppUserSectors/{sessionId}")]
    [ProducesResponseType(typeof(List<Guid>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<Guid>>> GetAppUserSectors(Guid sessionId)
    {
        AppUser? appUser = await _uow.AppUserRepository.GetUserBySessionIdAsync(sessionId);
        if(appUser == null)
        {
            return NotFound($"User with session ID {sessionId} not found.");
        }

        var sectorIds = (await _uow.AppUserSectorRepository.GetAllAppUserSectionIdsAsync(appUser.Id)).ToList();
        
        return Ok(sectorIds);
    }
    
    [HttpGet("GetAppUserSectors")]
    [ProducesResponseType(typeof(List<App.DTO.v1_0.AppUserSector>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<Guid>>> GetAllAppUserSectors()
    {
        var sectors = (await _uow.AppUserSectorRepository.GetAllEntitiesAsync(true)).ToList();
        return Ok(sectors);
    }
    
    
    [HttpPut("PutAppUserSectors/{sessionId}")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]    
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult> PutAppUserSectors(List<Guid> sectorIdList, Guid sessionId)
    {
        AppUser? appUser = await _uow.AppUserRepository.GetUserBySessionIdAsync(sessionId);
        if (appUser == null)
        {
            return NotFound($"User with session ID {sessionId} not found.");
        } 
        List<Guid> existingSectors = (await _uow.AppUserSectorRepository.GetAllAppUserSectionIdsAsync(appUser.Id)).ToList();
        foreach (var sectorId in sectorIdList)
        {
            if (sectorId == Guid.Empty)
            {
                return BadRequest("Invalid sector ID provided.");
            }
            if (existingSectors.Contains(sectorId))
            {
                existingSectors.Remove(sectorId);
                continue;
            }
            if (await _uow.SectorRepository.Exists(sectorId))
            {
                AppUserSector appUserSector = new AppUserSector
                {
                    AppUserId = appUser.Id,
                    SectorId = sectorId
                };
                _uow.AppUserSectorRepository.Add(appUserSector);
            }
            else
            {
                return NotFound($"Sector with ID {sectorId} not found.");
            }
        }
        await _uow.AppUserSectorRepository.RemoveExistingAppUserSectors(existingSectors, appUser.Id);
        
        await _uow.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPost("PostAppUserSectors")]
    [ProducesResponseType(typeof(List<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Created)] 
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult> PostAppUserSectors(PostAppUserSectorsRequest postRequest)
    {
        App.DTO.v1_0.AppUser newUser = postRequest.User;
        List<Guid> sectorIdList = postRequest.SectorIdsList;
        
        AppUser mappedUser = _userMapper.Map(newUser)!;
        mappedUser.Id = Guid.NewGuid();
        foreach (var sectorId in sectorIdList.ToList())
        {
            if (sectorId == Guid.Empty)
            {
                return BadRequest("Invalid sector ID provided.");
            }
            
            if (await _uow.SectorRepository.Exists(sectorId))
            {
                App.DAL.DTO.AppUserSector appUserSector = new App.DAL.DTO.AppUserSector
                {
                    Id = Guid.NewGuid(),
                    AppUserId = mappedUser.Id,
                    SectorId = sectorId
                };
                _uow.AppUserSectorRepository.Add(appUserSector);
            }
            else
            {
                sectorIdList.Remove(sectorId);
                //return NotFound("Sector not found: " + sectorId);
            }
        }
        _uow.AppUserRepository.Add(mappedUser);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
    
    

}