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

/// <summary>
/// API controller for managing application user sectors and receiving users.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AppUserSectorController : ControllerBase
{
    private readonly IAppUnitOfWork _uow;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.AppUser, App.DAL.DTO.AppUser> _userMapper;

    /// <summary>
    /// Constructor for AppUserSectorController.
    /// </summary>
    /// <param name="uow">The Unit of Work for interacting with the data access layer.</param>
    /// <param name="autoMapper">The AutoMapper instance for DTO mapping.</param>
    public AppUserSectorController(IAppUnitOfWork uow, IMapper autoMapper)
    {
        _uow = uow;
        _userMapper = new PublicDTOBllMapper<App.DTO.v1_0.AppUser, App.DAL.DTO.AppUser>(autoMapper);
    }
    
    /// <summary>
    /// Gets an application user by their session ID.
    /// </summary>
    /// <param name="sessionId">The session ID of the application user.</param>
    /// <returns>An ActionResult containing the AppUser DTO if found, or NotFound if not.</returns>
    [HttpGet("GetAppUser/{sessionId}")]
    [ProducesResponseType(typeof(App.DTO.v1_0.AppUser), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.AppUser>> GetAppUserBySessionId(Guid sessionId)
    {
        AppUser? appUser = await _uow.AppUserRepository.GetUserBySessionIdAsync(sessionId);
        if(appUser == null)
        {
            return NotFound($"User with session ID {sessionId} not found.");
        }

        appUser.Id = Guid.Empty;
        return Ok(_userMapper.Map(appUser));
    }
    
    /// <summary>
    /// Gets all sector IDs associated with a specific application user.
    /// </summary>
    /// <param name="sessionId">The session ID of the application user.</param>
    /// <returns>An ActionResult containing a list of sector GUIDs if the user is found, or NotFound if not.</returns>
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
    
    /// <summary>
    /// Gets all application user sectors.
    /// </summary>
    /// <returns>An ActionResult containing a list of AppUserSector DTOs.</returns>
    [HttpGet("GetAppUserSectors")]
    [ProducesResponseType(typeof(List<App.DTO.v1_0.AppUserSector>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.AppUserSector>>> GetAllAppUserSectors()
    {
        var sectors = (await _uow.AppUserSectorRepository.GetAllEntitiesAsync()).ToList();
        return Ok(sectors);
    }
    
    /// <summary>
    /// Updates the sectors associated with an application user.
    /// </summary>
    /// <param name="sessionId">The session ID of the application user.</param>
    /// <param name="sectorIdList">A list of sector GUIDs to associate with the user.</param>
    /// <returns>An ActionResult containing the list of processed sector GUIDs, or NotFound if the user is not found.</returns>
    [HttpPut("PutAppUserSectors/{sessionId}")]
    [ProducesResponseType(typeof(List<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<Guid>>> PutAppUserSectors(Guid sessionId, [FromBody] List<Guid> sectorIdList)
    {
        AppUser? appUser = await _uow.AppUserRepository.GetUserBySessionIdAsync(sessionId);
        if (appUser == null)
        {
            return NotFound($"User with session ID {sessionId} not found.");
        } 
        List<Guid> existingSectors = (await _uow.AppUserSectorRepository.GetAllAppUserSectionIdsAsync(appUser.Id)).ToList();
        foreach (var sectorId in sectorIdList.ToList())
        {
            if (sectorId == Guid.Empty)
            {
                //remove sectorId from the list if it's invalid
                sectorIdList.Remove(sectorId);
                //return BadRequest("Invalid sector ID provided.");
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
                sectorIdList.Remove(sectorId);
                //return NotFound($"Sector with ID {sectorId} not found.");
            }
        }
        await _uow.AppUserSectorRepository.RemoveExistingAppUserSectors(existingSectors, appUser.Id);
        
        await _uow.SaveChangesAsync();
        return Ok(sectorIdList);
        
    }
    
    /// <summary>
    /// Creates a new application user and associates them with a list of sectors.
    /// </summary>
    /// <param name="postRequest">The request body containing the new user data and a list of sector IDs.</param>
    /// <returns>An ActionResult containing the list of successfully associated sector GUIDs, or BadRequest/NotFound if creation fails or sectors are not found.</returns>
    [HttpPost("PostAppUserSectors")]
    [ProducesResponseType(typeof(List<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<Guid>>> PostAppUserSectors(PostAppUserSectorsRequest postRequest)
    {
        App.DTO.v1_0.AppUser newUser = postRequest.User;
        List<Guid> sectorIdList = postRequest.SectorIdsList;
        
        AppUser mappedUser = _userMapper.Map(newUser)!;
        mappedUser.Id = Guid.NewGuid();
        foreach (var sectorId in sectorIdList.ToList())
        {
            if (sectorId == Guid.Empty)
            {
                sectorIdList.Remove(sectorId);
                //return BadRequest("Invalid sector ID provided.");
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
        return Ok(sectorIdList);

    }
}