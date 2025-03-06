/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class ErrorTypeMaps
{
    public static IDictionary<string, (int StatusCode, string Title)> Maps { get; } =
        new Dictionary<string, (int StatusCode, string Title)>()
        {
            [ErrorTypes.Failure] = (StatusCodes.Status500InternalServerError, "Server Failure"),
            [ErrorTypes.Validation] = (StatusCodes.Status400BadRequest, "Bad Request"),
            [ErrorTypes.NotFound] = (StatusCodes.Status404NotFound, "Not Found"),
            [ErrorTypes.Conflict] = (StatusCodes.Status409Conflict, "Conflict"),
        };
}