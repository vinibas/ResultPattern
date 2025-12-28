/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;
using ViniBas.ResultPattern.AspNet.ResultMinimalApi;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher;

internal static class DefaultMvcMatchHelper
{
    internal static IActionResult OnSuccessDefault(ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess)
            return new OkObjectResult(resultResponse);

        throw new InvalidOperationException("Invalid success result response type.");
    }
    
    internal static IActionResult OnFailureDefault(ResultResponse resultResponse, bool? useProblemDetails)
    {
        if (resultResponse.IsSuccess)
            throw new InvalidOperationException("Invalid error result response type.");

        useProblemDetails ??= GlobalConfiguration.UseProblemDetails;
        var resultResponseError = (ResultResponseError)resultResponse;

        return useProblemDetails.Value ?
            resultResponseError.ToProblemDetailsActionResult() :
            resultResponseError.ToObjectResult();
    }

    private static IActionResult ToObjectResult(this ResultResponseError resultResponseError)
    {
        var statusCode = GlobalConfiguration.GetStatusCode(resultResponseError.Type);
        return new ObjectResult(resultResponseError) { StatusCode = statusCode };
    }
}

internal static class DefaultMinimalMatchHelper
{
    internal static IResult OnSuccessDefault(ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess)
            return TypedResults.Ok(resultResponse);

        throw new InvalidOperationException("Invalid success result response type.");
    }
    
    internal static IResult OnFailureDefault(ResultResponse resultResponse, bool? useProblemDetails)
    {
        if (resultResponse.IsSuccess)
            throw new InvalidOperationException("Invalid error result response type.");
        
        useProblemDetails ??= GlobalConfiguration.UseProblemDetails;
        var resultResponseError = (ResultResponseError)resultResponse;

        return useProblemDetails.Value ? 
            resultResponseError.ToProblemDetailsResult() : 
            resultResponseError.ToJsonTypedResult();
    }
    
    private static IResult ToJsonTypedResult(this ResultResponseError resultResponseError)
    {
        var statusCode = GlobalConfiguration.GetStatusCode(resultResponseError.Type);
        return TypedResults.Json(resultResponseError, statusCode: statusCode);
    }

    /// <summary>
    /// For future use
    /// </summary>
    private static bool IsResultResponseAResultResponseSuccessGeneric(ResultResponse resultResponse, out object? data)
    {
        data = null;
        var resultResponseType = resultResponse.GetType();

        if (!resultResponseType.IsGenericType ||
            resultResponseType.GetGenericTypeDefinition() != typeof(ResultResponseSuccess<>))
            return false;

        var dataProperty = resultResponseType.GetProperty(nameof(ResultResponseSuccess<object>.Data));
        data = dataProperty?.GetValue(resultResponse);
        return true;
    }
}
