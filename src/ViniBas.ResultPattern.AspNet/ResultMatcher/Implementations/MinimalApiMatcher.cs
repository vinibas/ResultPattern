/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
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
    internal Func<ResultResponseSuccess, IResult> OnSuccessFallback { get; set; }
        = FallbackMinimalMatchHelper.OnSuccessFallback;
    internal Func<ResultResponseError, IResult> OnFailureFallback { get; set; }
        = FallbackMinimalMatchHelper.OnFailureFallback;

    public IResult Match(
        ResultBase resultBase,
        Func<ResultResponseSuccess, IResult>? onSuccess,
        Func<ResultResponseError, IResult>? onFailure)
        => Match(resultBase.ToResponse(), onSuccess, onFailure);

    public IResult Match<TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, IResult>? onSuccess,
        Func<ResultResponseError, IResult>? onFailure)
        => Match(resultBase.ToResponse(), onSuccess, onFailure);

    public IResult Match(
        ResultResponse response,
        Func<ResultResponseSuccess, IResult>? onSuccess,
        Func<ResultResponseError, IResult>? onFailure)
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess((ResultResponseSuccess)response) : OnSuccessFallback((ResultResponseSuccess)response)) :
            (onFailure is not null ? onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

    public IResult Match<TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, IResult>? onSuccess,
        Func<ResultResponseError, IResult>? onFailure)
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess((ResultResponseSuccess<TData>)response) : FallbackMinimalMatchHelper.OnSuccessFallback((ResultResponseSuccess<TData>)response)) :
            (onFailure is not null ? onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

    public Task<IResult> MatchAsync(
        ResultBase resultBase,
        Func<ResultResponseSuccess, Task<IResult>>? onSuccess,
        Func<ResultResponseError, Task<IResult>>? onFailure)
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure);

    public Task<IResult> MatchAsync<TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, Task<IResult>>? onSuccess,
        Func<ResultResponseError, Task<IResult>>? onFailure)
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure);

    public async Task<IResult> MatchAsync(
        ResultResponse response,
        Func<ResultResponseSuccess, Task<IResult>>? onSuccess,
        Func<ResultResponseError, Task<IResult>>? onFailure)
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess((ResultResponseSuccess)response) : OnSuccessFallback((ResultResponseSuccess)response))
            : (onFailure is not null ? await onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

    public async Task<IResult> MatchAsync<TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, Task<IResult>>? onSuccess,
        Func<ResultResponseError, Task<IResult>>? onFailure)
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess((ResultResponseSuccess<TData>)response) : FallbackMinimalMatchHelper.OnSuccessFallback((ResultResponseSuccess<TData>)response))
            : (onFailure is not null ? await onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));
}
