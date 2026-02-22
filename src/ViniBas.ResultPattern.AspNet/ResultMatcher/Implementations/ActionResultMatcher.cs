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

    public IActionResult Match(
        ResultBase resultBase,
        Func<ResultResponse, IActionResult>? onSuccess,
        Func<ResultResponse, IActionResult>? onFailure,
        bool? useProblemDetails)
        => Match(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);

    public IActionResult Match(
        ResultResponse response,
        Func<ResultResponse, IActionResult>? onSuccess,
        Func<ResultResponse, IActionResult>? onFailure,
        bool? useProblemDetails)
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess(response) : OnSuccessFallback(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureFallback(response, useProblemDetails));

    public async Task<IActionResult> MatchAsync(
        ResultResponse response,
        Func<ResultResponse, Task<IActionResult>>? onSuccess,
        Func<ResultResponse, Task<IActionResult>>? onFailure,
        bool? useProblemDetails)
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess(response) : OnSuccessFallback(response))
            : (onFailure is not null ? await onFailure(response) : OnFailureFallback(response, useProblemDetails));

    public Task<IActionResult> MatchAsync(
        ResultBase resultBase,
        Func<ResultResponse, Task<IActionResult>>? onSuccess,
        Func<ResultResponse, Task<IActionResult>>? onFailure,
        bool? useProblemDetails)
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);
}
