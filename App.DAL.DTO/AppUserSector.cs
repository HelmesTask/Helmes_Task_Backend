using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class AppUserSector : IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid SectorId { get; set; }
    public Guid AppUserId { get; set; }
}