/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this Error error)
        => ((Result) error).ToResponse().ToProblemDetails();

    public static ProblemDetails ToProblemDetails(this ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess || resultResponse is not ResultResponseError resultResponseError)
            throw new InvalidOperationException("Unable to convert a valid ResultResponse to a ProblemDetails.");

        return new ProblemDetails()
        {
            Title = GlobalConfiguration.GetTitle(resultResponseError.Type),
            Status = GlobalConfiguration.GetStatusCode(resultResponseError.Type),
            Detail = string.Join("," + Environment.NewLine, resultResponseError.Errors.Select(d => d.ToString())),
            Extensions =
            {
                ["isSuccess"] = false,
                ["errors"] = resultResponseError.Errors,
            },
        };
    }
}
