/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

internal sealed class MinimalApiMatcher : ISimpleResultMatcher<IResult>
{
    internal Func<ResultResponse, IResult> OnSuccessFallback { get; set; } 
        = FallbackMinimalMatchHelper.OnSuccessFallback;
    internal Func<ResultResponse, bool?, IResult> OnFailureFallback { get; set; } 
        = FallbackMinimalMatchHelper.OnFailureFallback;

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
            (onSuccess is not null ? onSuccess(response) : OnSuccessFallback(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureFallback(response, useProblemDetails));

    public async Task<IResult> MatchAsync<TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IResult
        where TFailure : IResult
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess(response) : OnSuccessFallback(response))
            : (onFailure is not null ? await onFailure(response) : OnFailureFallback(response, useProblemDetails));

    public Task<IResult> MatchAsync<TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IResult
        where TFailure : IResult
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);
}
