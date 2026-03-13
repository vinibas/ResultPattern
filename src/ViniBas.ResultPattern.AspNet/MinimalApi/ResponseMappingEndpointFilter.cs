/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ViniBas.ResultPattern.AspNet.MinimalApi;

public sealed class ResponseMappingEndpointFilter : IEndpointFilter
{
    internal IFilterMappings filterMappings = new FilterMinimalApiMappings();

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var endpointResult = await next(context);
        return filterMappings.MapToResultResponse(endpointResult);
    }
}

public static class ResponseMappingEndpointFilterExtensions
{
    public static RouteHandlerBuilder WithResponseMappingFilter(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ResponseMappingEndpointFilter>();
}
