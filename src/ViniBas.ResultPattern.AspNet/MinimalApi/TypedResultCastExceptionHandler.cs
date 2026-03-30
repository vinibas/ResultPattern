/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.MinimalApi;

/// <summary>
/// Exception handler that catches <see cref="TypedResultCastException"/> and executes
/// the original <see cref="IResult"/> directly instead of letting the
/// exception propagate as a 500 error.
/// <para>
/// Recommended for <b>production</b> environments with typed Match results (<c>Results&lt;...&gt;</c>),
/// where it prevents cast mismatches from surfacing as 500 errors to end users.
/// The mismatch is logged so it can be investigated and fixed.
/// </para>
/// </summary>
public sealed class TypedResultCastExceptionHandler(ILogger<TypedResultCastExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not TypedResultCastException ex)
            return false;

        logger.LogError(ex,
            "Typed result cast failed on {Method} {Path}. The original IResult will be returned as fallback.",
            httpContext.Request.Method, httpContext.Request.Path);

        await ex.OriginalResult.ExecuteAsync(httpContext);
        return true;
    }
}
