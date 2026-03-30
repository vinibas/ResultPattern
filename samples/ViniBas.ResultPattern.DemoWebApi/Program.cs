/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Scalar.AspNetCore;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.DemoWebApi.Endpoints;
using ViniBas.ResultPattern.AspNet.Mvc;
using ViniBas.ResultPattern.AspNet.MinimalApi;
using ViniBas.ResultPattern.AspNet.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers(opt => opt.Filters.Add<ResponseMappingFilter>());
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<TypedResultCastExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

if (app.Environment.IsProduction())
    app.UseExceptionHandler();

app.UseHttpsRedirection();

app.RegisterUserGenericEndpoints();
app.RegisterUserUnionEndpoints();

app.MapControllers();

// Registering a new custom error type
Error.ErrorTypes.AddTypes(CustomErrorHelper.NotAcceptableKey);
GlobalConfiguration.ErrorTypeMaps.TryAdd(
    CustomErrorHelper.NotAcceptableKey,
    (StatusCodes.Status406NotAcceptable, CustomErrorHelper.NotAcceptableValue));
GlobalConfiguration.UseProblemDetails = true; // Default value
GlobalConfiguration.UnwrapSuccessData = false; // Default value

app.Run();
