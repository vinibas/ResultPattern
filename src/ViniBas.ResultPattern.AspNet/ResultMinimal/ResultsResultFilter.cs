/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

public sealed class ResultsResultFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var endpointResult = await next(context);

        return endpointResult switch
        {
            Error error => ConvertToResultResponseOrProblemDetails(error),
            IEnumerable<Error> errors => ConvertToResultResponseOrProblemDetails((Error)errors.ToList()),
            ResultBase result => result.IsSuccess ?
                result.ToResponse() :
                ConvertToProblemDetailsIfConfigured(result.ToResponse()),
            ResultResponseError resultResponseError => ConvertToProblemDetailsIfConfigured(resultResponseError),
            _ => endpointResult,
        };
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

public static class ResultsResultFilterExtensions
{
    public static RouteHandlerBuilder WithResultsResultFilter(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ResultsResultFilter>();
}