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
                Error error => error.ToProblemDetails(),
                IEnumerable<Error> errors => ((Error)errors.ToList()).ToProblemDetails(),
                ResultBase result => result.IsSuccess ?
                    result.ToResponse() :
                    result.ToResponse().ToProblemDetails(),
                ResultResponseError resultResponseError => resultResponseError.ToProblemDetails(),
                _ => objectResult.Value,
            };
        }
    }
}
