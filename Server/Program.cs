using Microsoft.EntityFrameworkCore;
using ShapesUI.DataBase;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// работа с БД
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseNpgsql(configuration.GetConnectionString("ExampleStorageDataBase"));
services.AddScoped<ApplicationDbContext>(provider =>
{
    return new ApplicationDbContext(optionsBuilder.Options);
});
services.AddScoped<UserRepository>();


services.AddSignalR();




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
