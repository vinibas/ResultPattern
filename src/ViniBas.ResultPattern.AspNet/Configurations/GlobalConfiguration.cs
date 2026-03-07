/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.Configurations;

/// <summary>
/// Global configuration for the ResultPattern ASP.NET integration.
/// Settings defined here apply to all requests unless overridden by <see cref="ScopedConfiguration"/>.
/// </summary>
public static class GlobalConfiguration
{
    /// <summary>
    /// Maps error types to their corresponding HTTP status code and title.
    /// Pre-populated with default entries for all built-in <see cref="ErrorTypes"/>.
    /// Add or overwrite entries to customize the HTTP response for custom or existing error types.
    /// </summary>
    public static ConcurrentDictionary<string, (int StatusCode, string Title)> ErrorTypeMaps { get; } = new ()
        {
            [ErrorTypes.Failure] = (StatusCodes.Status500InternalServerError, "Server Failure"),
            [ErrorTypes.Validation] = (StatusCodes.Status400BadRequest, "Bad Request"),
            [ErrorTypes.NotFound] = (StatusCodes.Status404NotFound, "Not Found"),
            [ErrorTypes.Conflict] = (StatusCodes.Status409Conflict, "Conflict"),
            [ErrorTypes.Unauthorized] = (StatusCodes.Status401Unauthorized, "Unauthorized"),
            [ErrorTypes.Forbidden] = (StatusCodes.Status403Forbidden, "Forbidden"),
        };

    /// <summary>
    /// When <see langword="true"/>, failure fallbacks return an RFC 7807 <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails" /> response.
    /// When <see langword="false"/>, failure fallbacks return the raw <see cref="ResultResponses.ResultResponseError"/> object.
    /// Defaults to <see langword="true"/>.
    /// Can be overridden per async scope via <see cref="ScopedConfiguration.Override"/>.
    /// </summary>
    public static bool UseProblemDetails { get; set; } = true;

    /// <summary>
    /// When <see langword="true"/>, the built-in success fallback unwraps the response:
    /// returns the <c>Data</c> value directly for <c>Result{TData}</c>, or an empty <c>200 OK</c> / <c>204 No Content</c> for <c>Result</c>.
    /// When <see langword="false"/>, the built-in success fallback returns the full <see cref="ResultResponses.ResultResponseSuccess"/> wrapper.
    /// Defaults to <see langword="false"/>.
    /// Also passed as <see cref="FallbackContext.UnwrapSuccessData"/> when a custom <see cref="FallbackOverrides"/> handler is invoked.
    /// Can be overridden per async scope via <see cref="ScopedConfiguration.Override"/>.
    /// </summary>
    public static bool UnwrapSuccessData { get; set; } = false;

    /// <summary>
    /// Custom fallback handlers that replace the built-in <c>Match</c> fallback behavior
    /// when no <c>onSuccess</c> or <c>onFailure</c> callback is provided by the caller.
    /// Configure <see cref="FallbackOverrides.Mvc"/> and/or <see cref="FallbackOverrides.MinimalApi"/> as needed.
    /// </summary>
    public static FallbackOverrides FallbackOverrides { get; } = new();

    internal static int GetStatusCode(string errorType)
        => ErrorTypeMaps.TryGetValue(errorType, out var map) ?
            map.StatusCode : StatusCodes.Status500InternalServerError;

    internal static string GetTitle(string errorType)
        => ErrorTypeMaps.TryGetValue(errorType, out var map) ?
            map.Title : "Server Failure";
}
