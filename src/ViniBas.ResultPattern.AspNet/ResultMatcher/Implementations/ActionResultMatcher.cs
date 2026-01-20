/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

internal sealed class ActionResultMatcher : ISimpleResultMatcher<IActionResult>
{
    internal Func<ResultResponse, IActionResult> OnSuccessFallback { get; set; } 
        = FallbackMvcMatchHelper.OnSuccessFallback;
    internal Func<ResultResponse, bool?, IActionResult> OnFailureFallback { get; set; } 
        = FallbackMvcMatchHelper.OnFailureFallback;
    
    public IActionResult Match<TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IActionResult
        where TFailure : IActionResult
        => Match(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);

    public IActionResult Match<TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IActionResult
        where TFailure : IActionResult
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess(response) : OnSuccessFallback(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureFallback(response, useProblemDetails));

    public async Task<IActionResult> MatchAsync<TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IActionResult
        where TFailure : IActionResult
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess(response) : OnSuccessFallback(response))
            : (onFailure is not null ? await onFailure(response) : OnFailureFallback(response, useProblemDetails));

    public Task<IActionResult> MatchAsync<TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TSuccess : IActionResult
        where TFailure : IActionResult
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);
}
