using Server.Hubs;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
new RegisterService(builder);

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
