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
    internal Func<ResultResponse, IResult> OnFailureFallback { get; set; }
        = FallbackMinimalMatchHelper.OnFailureFallback;

    public IResult Match(
        ResultBase resultBase,
        Func<ResultResponse, IResult>? onSuccess,
        Func<ResultResponse, IResult>? onFailure)
        => Match(resultBase.ToResponse(), onSuccess, onFailure);

    public IResult Match(
        ResultResponse response,
        Func<ResultResponse, IResult>? onSuccess,
        Func<ResultResponse, IResult>? onFailure)
        => response.IsSuccess ?
            (onSuccess is not null ? onSuccess(response) : OnSuccessFallback(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureFallback(response));

    public async Task<IResult> MatchAsync(
        ResultResponse response,
        Func<ResultResponse, Task<IResult>>? onSuccess,
        Func<ResultResponse, Task<IResult>>? onFailure)
        => response.IsSuccess
            ? (onSuccess is not null ? await onSuccess(response) : OnSuccessFallback(response))
            : (onFailure is not null ? await onFailure(response) : OnFailureFallback(response));

    public Task<IResult> MatchAsync(
        ResultBase resultBase,
        Func<ResultResponse, Task<IResult>>? onSuccess,
        Func<ResultResponse, Task<IResult>>? onFailure)
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure);
}
