using Base.Contracts.Domain;
using Base.Domain;
namespace App.Domain.User;

public class AppUser : BaseEntityId, IDomainAppUserSessionId
{
    public Guid SessionId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public bool TermsConfirmed { get; set; } = default!;
    public ICollection<Sector>? Sectors;
}