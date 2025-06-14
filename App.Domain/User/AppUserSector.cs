using Base.Domain;

namespace App.Domain.User;

public class AppUserSector :  BaseEntityId
{
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = default!; // Navigation property

    // Foreign Key to Sector
    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = default!; // Navigation property
}