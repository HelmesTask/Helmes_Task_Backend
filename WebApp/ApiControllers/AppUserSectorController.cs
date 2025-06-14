using System.Net;
using App.Contracts.DAL;
using App.DAL.DTO;
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
        var sectorIds = (await _uow.AppUserSectorRepository.GetAllAppUserSectionIdsAsync(sessionId)).ToList();
        
        return Ok(sectorIds);
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
        List<Guid> existingSectors = (await _uow.AppUserSectorRepository.GetAllAppUserSectionIdsAsync(sessionId)).ToList();
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
            if (_uow.SectorRepository.Exists(sectorId))
            {
                App.DAL.DTO.AppUserSector appUserSector = new App.DAL.DTO.AppUserSector
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
        _uow.AppUserSectorRepository.RemoveExistingAppUserSectors(existingSectors, appUser.Id);
        
        await _uow.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPost("PostAppUserSectors/{sessionId}")]
    [ProducesResponseType(typeof(List<Guid>),(int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult> PostAppUserSectors(List<Guid> sectorIdList, App.DTO.v1_0.AppUser newUser, Guid sessionId)
    {
        newUser.Id = Guid.NewGuid();
        App.DAL.DTO.AppUser mappedUser = _userMapper.Map(newUser)!;
        foreach (var sectorId in sectorIdList)
        {
            if (sectorId == Guid.Empty)
            {
                return BadRequest("Invalid sector ID provided.");
            }
            
            if (_uow.SectorRepository.Exists(sectorId))
            {
                App.DAL.DTO.AppUserSector appUserSector = new App.DAL.DTO.AppUserSector
                {
                    Id = Guid.NewGuid(),
                    AppUserId = mappedUser!.Id,
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