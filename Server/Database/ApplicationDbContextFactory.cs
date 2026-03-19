using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Server.DataBase;

public sealed class ApplicationDbContextFactory(IConfiguration configuration)
{
    private IConfiguration _configuration = configuration;

    public ApplicationDbContext CreateApplicationContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("ExampleStorageDataBase"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
