/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.Configurations;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet;

internal interface IFilterMappings
{
    object? MapToResultResponse(object? originalResult);
}

internal abstract class FilterMappings : IFilterMappings
{
    public object? MapToResultResponse(object? originalResult)
    {
        return originalResult switch
        {
            Error error => CallFallbackMatcher(((Result)error).ToResponse()),
            IEnumerable<Error> errors => CallFallbackMatcher(((Result)(Error)errors.ToList()).ToResponse()),
            ResultBase result => CallFallbackMatcher(result.ToResponse()),
            _ => originalResult,
        };
    }

    protected abstract object? CallFallbackMatcher(ResultResponse resultResponse);
}

internal class FilterMvcMappings : FilterMappings
{
    protected override object? CallFallbackMatcher(ResultResponse resultResponse)
        => resultResponse.IsSuccess ?
        FallbackMvcMatchHelper.OnSuccessFallback(resultResponse) :
        FallbackMvcMatchHelper.OnFailureFallback(resultResponse);
}

internal class FilterMinimalApiMappings : FilterMappings
{
    protected override object? CallFallbackMatcher(ResultResponse resultResponse)
        => resultResponse.IsSuccess ?
        FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse) :
        FallbackMinimalMatchHelper.OnFailureFallback(resultResponse);
}
