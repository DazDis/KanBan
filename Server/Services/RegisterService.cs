using Microsoft.EntityFrameworkCore;
using Server.Database.Repositories;
using Server.DataBase;

namespace Server.Services
{
    public class RegisterService
    {
        public RegisterService(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            builder.Services.AddSingleton<IConfiguration>(configuration);
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("ExampleStorageDataBase"));
            builder.Services.AddScoped<ApplicationDbContext>(provider =>
            {
                return new ApplicationDbContext(optionsBuilder.Options);
            });

            builder.Services.AddSingleton<ApplicationDbContextFactory>(provider =>
                new ApplicationDbContextFactory(provider.GetRequiredService<IConfiguration>())
            );
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<TaskRepository>();
            builder.Services.AddScoped<LabelRepository>();
            builder.Services.AddScoped<TeamRepository>();
            builder.Services.AddScoped<ColumnRepository>();

            builder.Services.AddSignalR();

            var ip = configuration.GetValue<string>("DbIp");
            var port = configuration.GetValue<int>("DbPort");

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(System.Net.IPAddress.Parse(ip), port);
            });
        }
    }
}
