using Base.Domain;

namespace App.Domain;

public class Sector : BaseEntityId
{
    public string SectorName { get; set; } = default!;
    public ICollection<Sector> ParentSectors { get; set; } = new List<Sector>();
}