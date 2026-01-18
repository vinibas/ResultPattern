/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class GlobalConfiguration
{
    public static ConcurrentDictionary<string, (int StatusCode, string Title)> ErrorTypeMaps { get; } = new ()
        {
            [ErrorTypes.Failure] = (StatusCodes.Status500InternalServerError, "Server Failure"),
            [ErrorTypes.Validation] = (StatusCodes.Status400BadRequest, "Bad Request"),
            [ErrorTypes.NotFound] = (StatusCodes.Status404NotFound, "Not Found"),
            [ErrorTypes.Conflict] = (StatusCodes.Status409Conflict, "Conflict"),
            [ErrorTypes.Unauthorized] = (StatusCodes.Status401Unauthorized, "Unauthorized"),
            [ErrorTypes.Forbidden] = (StatusCodes.Status403Forbidden, "Forbidden"),
        };

    public static bool UseProblemDetails { get; set; } = true;

    internal static int GetStatusCode(string errorType)
        => ErrorTypeMaps.TryGetValue(errorType, out var map) ?
            map.StatusCode : StatusCodes.Status500InternalServerError;

    internal static string GetTitle(string errorType)
        => ErrorTypeMaps.TryGetValue(errorType, out var map) ?
            map.Title : "Server Failure";
}
