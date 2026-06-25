using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fepi.Api.Data;

public class FepiDbContextFactory : IDesignTimeDbContextFactory<FepiDbContext>
{
    public FepiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FepiDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=fepi;Username=postgres;Password=Mariana1"
        );

        return new FepiDbContext(optionsBuilder.Options);
    }
}