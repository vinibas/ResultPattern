/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
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
    internal Func<ResultResponseSuccess, IActionResult> OnSuccessFallback { get; set; }
        = FallbackMvcMatchHelper.OnSuccessFallback;
    internal Func<ResultResponseError, IActionResult> OnFailureFallback { get; set; }
        = FallbackMvcMatchHelper.OnFailureFallback;

    public IActionResult Match(
        ResultBase resultBase,
        Func<ResultResponseSuccess, IActionResult>? onSuccess,
        Func<ResultResponseError, IActionResult>? onFailure)
        => Match(resultBase.ToResponse(), onSuccess, onFailure);

    public IActionResult Match<TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, IActionResult>? onSuccess,
        Func<ResultResponseError, IActionResult>? onFailure)
        => Match(resultBase.ToResponse(), onSuccess, onFailure);

    public IActionResult Match(
        ResultResponse response,
        Func<ResultResponseSuccess, IActionResult>? onSuccess,
        Func<ResultResponseError, IActionResult>? onFailure)
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess((ResultResponseSuccess)response) : OnSuccessFallback((ResultResponseSuccess)response)) :
            (onFailure is not null ? onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

    public IActionResult Match<TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, IActionResult>? onSuccess,
        Func<ResultResponseError, IActionResult>? onFailure)
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess((ResultResponseSuccess<TData>)response) : FallbackMvcMatchHelper.OnSuccessFallback((ResultResponseSuccess<TData>)response)) :
            (onFailure is not null ? onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

    public Task<IActionResult> MatchAsync(
        ResultBase resultBase,
        Func<ResultResponseSuccess, Task<IActionResult>>? onSuccess,
        Func<ResultResponseError, Task<IActionResult>>? onFailure)
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure);

    public Task<IActionResult> MatchAsync<TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, Task<IActionResult>>? onSuccess,
        Func<ResultResponseError, Task<IActionResult>>? onFailure)
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure);

    public async Task<IActionResult> MatchAsync(
        ResultResponse response,
        Func<ResultResponseSuccess, Task<IActionResult>>? onSuccess,
        Func<ResultResponseError, Task<IActionResult>>? onFailure)
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess((ResultResponseSuccess)response) : OnSuccessFallback((ResultResponseSuccess)response))
            : (onFailure is not null ? await onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

    public async Task<IActionResult> MatchAsync<TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, Task<IActionResult>>? onSuccess,
        Func<ResultResponseError, Task<IActionResult>>? onFailure)
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess((ResultResponseSuccess<TData>)response) : FallbackMvcMatchHelper.OnSuccessFallback((ResultResponseSuccess<TData>)response))
            : (onFailure is not null ? await onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));
}
