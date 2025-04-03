/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

internal static class MatchHelper
{
    internal static IResult OnErrorDefault(ResultResponse resultResponse, bool? useProblemDetails)
    {
        if (resultResponse is not ResultResponseError resultResponseError)
            throw new Exception();

        useProblemDetails ??= GlobalConfiguration.UseProblemDetails;
        return useProblemDetails.Value ? 
            resultResponseError.ToProblemDetailsResult() : 
            resultResponseError.ToJsonTypedResult();
    }
    
    private static IResult ToJsonTypedResult(this ResultResponseError resultResponseError)
    {
        var statusCode = GlobalConfiguration.GetStatusCode(resultResponseError.Type);
        return TypedResults.Json(resultResponseError, statusCode: statusCode);
    }
}
