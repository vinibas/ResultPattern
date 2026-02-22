/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using System.Threading.Tasks;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

internal sealed class TypedResultMatcher : ITypedResultMatcher
{
    internal Func<ResultResponse, IResult> OnSuccessFallback { get; set; }
        = FallbackMinimalMatchHelper.OnSuccessFallback;
    internal Func<ResultResponse, bool?, IResult> OnFailureFallback { get; set; }
        = FallbackMinimalMatchHelper.OnFailureFallback;
    internal TypeCaster TypeCasterInstance { get; set; } = new TypeCaster();


    public TResult Match<TResult>(
        ResultBase resultBase,
        Func<ResultResponse, TResult>? onSuccess,
        Func<ResultResponse, TResult>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider
        => Match(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);

    public TResult Match<TResult>(
        ResultResponse response,
        Func<ResultResponse, TResult>? onSuccess,
        Func<ResultResponse, TResult>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider
    {
        var result = response.IsSuccess ?
            (onSuccess is not null ? onSuccess(response) : OnSuccessFallback(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureFallback(response, useProblemDetails));

        return TypeCasterInstance.Cast<TResult>(result);
    }

    public Task<TResult> MatchAsync<TResult>(
        ResultBase resultBase,
        Func<ResultResponse, Task<TResult>>? onSuccess,
        Func<ResultResponse, Task<TResult>>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);

    public async Task<TResult> MatchAsync<TResult>(
        ResultResponse response,
        Func<ResultResponse, Task<TResult>>? onSuccess,
        Func<ResultResponse, Task<TResult>>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider
    {
        var result = response.IsSuccess
            ? (onSuccess is not null ? await onSuccess(response) : OnSuccessFallback(response))
            : (onFailure is not null ? await onFailure(response) : OnFailureFallback(response, useProblemDetails));

        return TypeCasterInstance.Cast<TResult>(result);
    }


    internal class TypeCaster
    {
        public virtual TResult Cast<TResult>(IResult result) where TResult : IResult
            => TypeCastHelper.TreatCast<TResult>(result);
    }
}
