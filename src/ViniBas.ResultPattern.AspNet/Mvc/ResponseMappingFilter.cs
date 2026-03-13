/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ViniBas.ResultPattern.AspNet.Mvc;

public sealed class ResponseMappingFilter : IActionFilter
{
    internal IFilterMappings filterMappings = new FilterMvcMappings();

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null) return;

        var contextResult = context.Result;

        if (contextResult is ObjectResult objectResult &&
            objectResult.Value is not ProblemDetails)
        {
            var mappedResponse = filterMappings.MapToResultResponse(objectResult.Value);

            if (mappedResponse is ObjectResult mappedResult)
                context.Result = mappedResult;
            else
            {
                objectResult.Value = mappedResponse;
                objectResult.DeclaredType = null;

            }
        }
    }
}
