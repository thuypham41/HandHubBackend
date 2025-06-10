using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HandHubAPI.Infrastructure.Data;

internal class LonerDbContextFactory : IDesignTimeDbContextFactory<HandHubDbContext>
{
    public HandHubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HandHubDbContext>();
        // TODO: REPLACE WITH YOUR ACTUAL CONNECTION STRING
        var connectionString = Enviroments.ConnectionString_SSMS;
        optionsBuilder.UseSqlServer(connectionString);

        return new HandHubDbContext(optionsBuilder.Options);

        
    }
}