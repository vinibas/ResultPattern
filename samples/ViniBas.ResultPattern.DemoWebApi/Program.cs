using ViniBas.ResultPattern.AspNet;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.DemoWebApi.Endpoints;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMyService, MyService>();

builder.Services.AddControllers(opt => opt.Filters.Add<ActionResultFilter>());

var app = builder.Build();

app.UseHttpsRedirection();

app.RegisterUserEndpoints();

app.MapControllers();

Error.ErrorTypes.AddTypes("NotAcceptable");
ErrorTypeMaps.Maps.Add("NotAcceptable", (StatusCodes.Status406NotAcceptable, "Not Acceptable"));

app.Run();
