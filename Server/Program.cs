using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Database.Repositories;
using Server.DataBase;
using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// работа с БД
builder.Services.AddSignalR();



IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddSingleton<IConfiguration>(configuration);
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseNpgsql(configuration.GetConnectionString("ExampleStorageDataBase"));
builder.Services.AddScoped<ApplicationDbContext>(provider =>
{
    return new ApplicationDbContext(optionsBuilder.Options);
});




/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ExampleStorageDataBase"))
);*/
builder.Services.AddSingleton<ApplicationDbContextFactory>(provider =>
    new ApplicationDbContextFactory(provider.GetRequiredService<IConfiguration>())
);
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<TaskRepository>();
builder.Services.AddScoped<LabelRepository>();
builder.Services.AddScoped<TeamRepository>();
builder.Services.AddScoped<ColumnRepository>();

builder.Services.AddSignalR();


// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}*/
var ip = configuration.GetValue<string>("DbIp");
var port = configuration.GetValue<int>("DbPort");

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Parse(ip), port);
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<TaskHub>("/taskHub"); 
app.Run();
