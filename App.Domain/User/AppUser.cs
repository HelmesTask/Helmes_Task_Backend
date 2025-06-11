using Base.Contracts.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.User;

public class AppUser : IdentityUser<Guid>, IDomainEntityId
{
    public ICollection<Sector>? Sectors;
}