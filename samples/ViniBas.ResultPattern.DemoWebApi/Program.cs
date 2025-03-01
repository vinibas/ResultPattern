using ViniBas.ResultPattern.AspNet;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.ResultObjects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMyService, MyService>();

builder.Services.AddControllers(opt => opt.Filters.Add<ActionResultFilter>());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Error.ErrorTypes.AddTypes("NotAcceptable");
ErrorTypeMaps.Maps.Add("NotAcceptable", (StatusCodes.Status406NotAcceptable, "Not Acceptable"));

app.Run();
