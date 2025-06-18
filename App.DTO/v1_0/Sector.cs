using Base.Contracts.Domain;

namespace App.DTO.v1_0;

public class Sector
{
    public Guid? Id { get; set; }
    public Guid? ParentSectorId { get; set; }
    public string SectorName { get; set; }
}