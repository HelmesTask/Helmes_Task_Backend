using Base.Contracts.Domain;

namespace App.DTO.v1_0;

public class AppUser 
{
    public Guid? Id { get; set; }
    public Guid SessionId { get; set; } 
    public string UserName { get; set; } 
    public bool TermsConfirmed { get; set; }
}