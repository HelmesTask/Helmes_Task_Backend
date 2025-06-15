namespace App.DTO.v1_0;

public class PostAppUserSectorsRequest
{
    public AppUser User { get; set; }
    public List<Guid> SectorIdsList { get; set; }
}