using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class  Sector : IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid? ParentSectorId { get; set; }
    public string SectorName { get; set; }
}