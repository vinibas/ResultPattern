using ViniBas.ResultPattern.AspNet;
using ViniBas.ResultPattern.DemoWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMyService, MyService>();

builder.Services.AddControllers(opt => opt.Filters.Add<ActionResultFilter>());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
