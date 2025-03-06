/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

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
