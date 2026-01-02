/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet;

public interface IFilterMappings
{
    object? MapToResultResponse(object? originalResult);
}

internal class FilterMappings : IFilterMappings
{
    public object? MapToResultResponse(object? originalResult)
    {
        return originalResult switch
        {
            Error error => ConvertToResultResponseOrProblemDetails(error),
            IEnumerable<Error> errors => ConvertToResultResponseOrProblemDetails((Error)errors.ToList()),
            ResultBase result => result.IsSuccess ?
                result.ToResponse() :
                ConvertToProblemDetailsIfConfigured(result.ToResponse()),
            ResultResponseError resultResponseError => ConvertToProblemDetailsIfConfigured(resultResponseError),
            _ => originalResult,
        };
    }

    private object? ConvertToResultResponseOrProblemDetails(Error error)
        => GlobalConfiguration.UseProblemDetails ?
            error.ToProblemDetails() :
            ((Result)error).ToResponse();

    private object? ConvertToProblemDetailsIfConfigured(ResultResponse error)
        => GlobalConfiguration.UseProblemDetails ?
            error.ToProblemDetails() :
            error;
}