using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.DAL.EF;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(String[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        return new AppDbContext(optionsBuilder.Options);
    }
    
}