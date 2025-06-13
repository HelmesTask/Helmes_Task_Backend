using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class AppUser : IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; } 
    public string UserName { get; set; } 
    public bool TermsConfirmed { get; set; }
}