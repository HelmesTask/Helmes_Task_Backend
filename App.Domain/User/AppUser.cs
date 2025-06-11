using Base.Contracts.Domain;
using Base.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.User;

public class AppUser : BaseEntityId
{
    public string userName { get; set; } = default!;
    public ICollection<Sector>? Sectors;
}