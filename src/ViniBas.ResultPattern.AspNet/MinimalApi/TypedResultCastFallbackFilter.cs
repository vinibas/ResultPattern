/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.MinimalApi;

/// <summary>
/// Endpoint filter that catches <see cref="TypedResultCastException"/> and returns
/// the original <see cref="IResult"/> directly instead of letting the
/// exception propagate as a 500 error.
/// <para>
/// Recommended for <b>production</b> environments with typed Match results (<c>Results&lt;...&gt;</c>),
/// where it prevents cast mismatches from surfacing as 500 errors to end users.
/// The mismatch is logged so it can be investigated and fixed.
/// </para>
/// </summary>
public sealed class TypedResultCastFallbackFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (TypedResultCastException ex)
        {
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<TypedResultCastFallbackFilter>))
                as ILogger<TypedResultCastFallbackFilter>;

            var request = context.HttpContext.Request;
            logger?.LogError(ex,
                "Typed result cast failed on {Method} {Path}. The original IResult will be returned as fallback.",
                request.Method, request.Path);

            return ex.OriginalResult;
        }
    }
}

/// <summary>
/// Extension methods for registering the <see cref="TypedResultCastFallbackFilter"/>.
/// </summary>
public static class TypedResultCastFallbackFilterExtensions
{
    /// <summary>
    /// Adds the <see cref="TypedResultCastFallbackFilter"/> to the endpoint pipeline.
    /// This filter catches typed result cast mismatches and returns the original
    /// <see cref="IResult"/> directly, preventing the exception from propagating as a 500 error.
    /// </summary>
    public static RouteHandlerBuilder WithTypedResultCastFallback(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<TypedResultCastFallbackFilter>();
}
