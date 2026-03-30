/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ViniBas.ResultPattern.AspNet.MinimalApi;

public sealed class ResponseMappingEndpointFilter : IEndpointFilter
{
    internal IFilterMappings filterMappings = new FilterMinimalApiMappings();

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var endpointResult = await next(context);
        var mapped = filterMappings.MapToResultResponse(endpointResult);

        var innerResult = mapped is INestedHttpResult nested ? nested.Result : mapped;

        if (innerResult is IValueHttpResult { Value: ProblemDetails } && innerResult is not ProblemHttpResult)
            context.HttpContext.Response.OnStarting(() =>
            {
                context.HttpContext.Response.ContentType = "application/problem+json";
                return Task.CompletedTask;
            });

        return mapped;
    }
}

public static class ResponseMappingEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithResponseMappingFilter(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ResponseMappingEndpointFilter>();
}
