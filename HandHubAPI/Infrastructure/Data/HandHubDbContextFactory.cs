using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HandHubAPI.Infrastructure.Data;

internal class HandHubDbContextFactory : IDesignTimeDbContextFactory<HandHubDbContext>
{
    public HandHubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HandHubDbContext>();
        // TODO: REPLACE WITH YOUR ACTUAL CONNECTION STRING
        var connectionString = Enviroments.ConnectionString_Docker;
        optionsBuilder.UseSqlServer(connectionString);

        return new HandHubDbContext(optionsBuilder.Options);


    }
}