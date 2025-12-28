/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

internal sealed class MinimalApiMatcher : ISimpleResultMatcher<IResult>
{
    internal Func<ResultResponse, IResult> OnSuccessDefault { get; set; } 
        = DefaultMinimalMatchHelper.OnSuccessDefault;
    internal Func<ResultResponse, bool?, IResult> OnFailureDefault { get; set; } 
        = DefaultMinimalMatchHelper.OnFailureDefault;

    public IResult Match<TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IResult
        where TFailure : IResult
        => Match(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);

    public IResult Match<TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IResult
        where TFailure : IResult
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess(response) : OnSuccessDefault(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureDefault(response, useProblemDetails));
}
