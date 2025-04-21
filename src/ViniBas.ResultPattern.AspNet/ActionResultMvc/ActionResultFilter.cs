/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.ActionResultMvc;

public sealed class ActionResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null) return;

        var contextResult = context.Result;

        if (contextResult is ObjectResult objectResult &&
            objectResult.Value is not ProblemDetails)
        {
            objectResult.Value = objectResult.Value switch
            {
                Error error => ConvertToResultResponseOrProblemDetails(error),
                IEnumerable<Error> errors => ConvertToResultResponseOrProblemDetails((Error)errors.ToList()),
                ResultBase result => result.IsSuccess ?
                    result.ToResponse() :
                    ConvertToProblemDetailsIfConfigured(result.ToResponse()),
                ResultResponseError resultResponseError => ConvertToProblemDetailsIfConfigured(resultResponseError),
                _ => objectResult.Value,
            };
        }
    }

    private static object? ConvertToResultResponseOrProblemDetails(Error error)
        => GlobalConfiguration.UseProblemDetails ?
            error.ToProblemDetails() :
            ((Result)error).ToResponse();

    private static object? ConvertToProblemDetailsIfConfigured(ResultResponse error)
        => GlobalConfiguration.UseProblemDetails ?
            error.ToProblemDetails() :
            error;
}
