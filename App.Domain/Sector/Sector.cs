namespace App.Domain;

public class Sector
{
    public string SectorName { get; set; } = default!;
    public ICollection<Sector> ParentSectors { get; set; } = new List<Sector>();
}