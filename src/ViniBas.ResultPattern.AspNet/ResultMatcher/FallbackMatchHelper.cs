/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.Configurations;
using ViniBas.ResultPattern.AspNet.MinimalApi;
using ViniBas.ResultPattern.AspNet.Mvc;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher;

internal static class FallbackMvcMatchHelper
{
    internal static IActionResult OnSuccessFallback(ResultResponse resultResponse)
    {
        if (!resultResponse.IsSuccess)
            throw new InvalidOperationException("Invalid success result response type.");

        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.Mvc is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        if (fallbackContext.UnwrapSuccessData)
            return FallbackMatchHelper.IsAResultResponseSuccessWithGenericData(resultResponse, out var data) ?
                new OkObjectResult(data) :
                new OkResult();

        return new OkObjectResult(resultResponse);
    }

    internal static IActionResult OnFailureFallback(ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess)
            throw new InvalidOperationException("Invalid error result response type.");

        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.Mvc is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        var resultResponseError = (ResultResponseError)resultResponse;

        return fallbackContext.UseProblemDetails ?
            resultResponseError.ToProblemDetailsActionResult() :
            resultResponseError.ToObjectResult(fallbackContext.UnwrapSuccessData);
    }

    private static IActionResult ToObjectResult(this ResultResponseError resultResponseError, bool unwrapSuccessData)
    {
        var statusCode = GlobalConfiguration.GetStatusCode(resultResponseError.Type);

        return unwrapSuccessData ?
            new ObjectResult(resultResponseError.Errors) { StatusCode = statusCode } :
            new ObjectResult(resultResponseError) { StatusCode = statusCode };
    }
}

internal static class FallbackMinimalMatchHelper
{
    internal static IResult OnSuccessFallback(ResultResponse resultResponse)
    {
        if (!resultResponse.IsSuccess)
            throw new InvalidOperationException("Invalid success result response type.");

        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.MinimalApi is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        if (fallbackContext.UnwrapSuccessData)
            return FallbackMatchHelper.IsAResultResponseSuccessWithGenericData(resultResponse, out var data) ?
                TypedResults.Ok(data) :
                TypedResults.Ok();

        return TypedResults.Ok(resultResponse);
    }

    internal static IResult OnFailureFallback(ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess)
            throw new InvalidOperationException("Invalid error result response type.");

        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.MinimalApi is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        var resultResponseError = (ResultResponseError)resultResponse;

        return fallbackContext.UseProblemDetails ?
            resultResponseError.ToProblemDetailsResult() :
            resultResponseError.ToJsonTypedResult(fallbackContext.UnwrapSuccessData);
    }

    private static IResult ToJsonTypedResult(this ResultResponseError resultResponseError, bool unwrapSuccessData)
    {
        var statusCode = GlobalConfiguration.GetStatusCode(resultResponseError.Type);
        return unwrapSuccessData ?
            TypedResults.Json(resultResponseError.Errors, statusCode: statusCode) :
            TypedResults.Json(resultResponseError, statusCode: statusCode);
    }
}


file class FallbackMatchHelper
{
    internal static FallbackContext BuildFallbackContext()
        => new (GetUnwrapSuccessCurrentValue(), GetUseProblemDetailsCurrentValue());

    private static bool GetUnwrapSuccessCurrentValue()
        => ScopedConfiguration.Current?.UnwrapSuccessData
            ?? GlobalConfiguration.UnwrapSuccessData;

    private static bool GetUseProblemDetailsCurrentValue()
        => ScopedConfiguration.Current?.UseProblemDetails
            ?? GlobalConfiguration.UseProblemDetails;

    internal static bool IsAResultResponseSuccessWithGenericData(ResultResponse resultResponse, out object? data)
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
