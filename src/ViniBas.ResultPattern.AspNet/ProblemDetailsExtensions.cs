/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess || resultResponse is not ResultResponseError resultResponseError)
            throw new InvalidOperationException("Unable to convert a valid ResultResponse to a ProblemDetails.");

        return new ProblemDetails()
        {
            Title = GetTitle(resultResponseError.Type),
            Status = GetStatusCode(resultResponseError.Type),
            Detail = string.Join(Environment.NewLine, resultResponseError.Errors),
            Extensions =
            {
                ["isSuccess"] = false,
                ["errors"] = resultResponseError.Errors,
            },
        };
    }

    private static int GetStatusCode(string errorType)
        => ErrorTypeMaps.Maps.TryGetValue(errorType, out var map) ?
            map.StatusCode : StatusCodes.Status500InternalServerError;

    private static string GetTitle(string errorType)
        => ErrorTypeMaps.Maps.TryGetValue(errorType, out var map) ?
            map.Title : "Server Failure";
}
