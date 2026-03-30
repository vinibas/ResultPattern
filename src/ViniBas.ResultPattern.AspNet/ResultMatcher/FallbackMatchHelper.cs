/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.Configurations;
using ViniBas.ResultPattern.AspNet.MinimalApi;
using ViniBas.ResultPattern.AspNet.Mvc;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher;

internal static class FallbackMvcMatchHelper
{
    internal static IActionResult OnSuccessFallback(ResultResponseSuccess resultResponse)
    {
        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.Mvc is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        if (fallbackContext.UnwrapSuccessData)
            return new OkResult();

        return new OkObjectResult(resultResponse);
    }

    internal static IActionResult OnSuccessFallback<TData>(ResultResponseSuccess<TData> resultResponse)
    {
        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.Mvc is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        if (fallbackContext.UnwrapSuccessData)
            return new OkObjectResult(resultResponse.Data);

        return new OkObjectResult(resultResponse);
    }

    internal static IActionResult OnFailureFallback(ResultResponseError resultResponseError)
    {
        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.Mvc is { } fallbackOverride)
            return fallbackOverride(resultResponseError, fallbackContext);

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
    internal static IResult OnSuccessFallback(ResultResponseSuccess resultResponse)
    {
        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.MinimalApi is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        if (!GlobalConfiguration.TypedResultMaps.TryGetValue(StatusCodes.Status200OK, out var builder))
            builder = TypedResultBuilders.Ok;

        if (fallbackContext.UnwrapSuccessData)
            return builder.Build();

        return builder.Build(resultResponse);
    }

    internal static IResult OnSuccessFallback<TData>(ResultResponseSuccess<TData> resultResponse)
    {
        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.MinimalApi is { } fallbackOverride)
            return fallbackOverride(resultResponse, fallbackContext);

        if (!GlobalConfiguration.TypedResultMaps.TryGetValue(StatusCodes.Status200OK, out var builder))
            builder = TypedResultBuilders.Ok;

        if (fallbackContext.UnwrapSuccessData)
            return builder.Build(resultResponse.Data);

        return builder.Build(resultResponse);
    }

    internal static IResult OnFailureFallback(ResultResponseError resultResponseError)
    {
        var fallbackContext = FallbackMatchHelper.BuildFallbackContext();

        if (GlobalConfiguration.FallbackOverrides.MinimalApi is { } fallbackOverride)
            return fallbackOverride(resultResponseError, fallbackContext);

        var statusCode = GlobalConfiguration.GetStatusCode(resultResponseError.Type);

        if (!GlobalConfiguration.TypedResultMaps.TryGetValue(statusCode, out var builder))
            builder = !fallbackContext.UseProblemDetails ?
                TypedResultBuilders.Json(statusCode) :
                null;

        if (fallbackContext.UseProblemDetails)
            return builder is null ?
                resultResponseError.ToProblemDetailsResult() :
                builder.Build(resultResponseError.ToProblemDetails());

        return fallbackContext.UnwrapSuccessData ?
            builder!.Build(resultResponseError.Errors) :
            builder!.Build(resultResponseError);
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
}
