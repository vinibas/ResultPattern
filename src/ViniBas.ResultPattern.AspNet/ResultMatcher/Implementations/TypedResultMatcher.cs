/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

internal sealed class TypedResultMatcher : ITypedResultMatcher
{
    internal Func<ResultResponseSuccess, IResult> OnSuccessFallback { get; set; }
        = FallbackMinimalMatchHelper.OnSuccessFallback;
    internal Func<ResultResponseError, IResult> OnFailureFallback { get; set; }
        = FallbackMinimalMatchHelper.OnFailureFallback;
    internal TypeCaster TypeCasterInstance { get; set; } = new TypeCaster();

    public TResult Match<TResult>(
        ResultBase resultBase,
        Func<ResultResponseSuccess, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Match(resultBase.ToResponse(), onSuccess, onFailure);

    public TResult Match<TResult, TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Match<TResult, TData>(resultBase.ToResponse(), onSuccess, onFailure);

    public TResult Match<TResult>(
        ResultResponse response,
        Func<ResultResponseSuccess, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
    {
        var result = response.IsSuccess ?
            (onSuccess is not null ? onSuccess((ResultResponseSuccess)response) : OnSuccessFallback((ResultResponseSuccess)response)) :
            (onFailure is not null ? onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

        return TypeCasterInstance.Cast<TResult>(result);
    }

    public TResult Match<TResult, TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
    {
        var result = response.IsSuccess ?
            (onSuccess is not null ? onSuccess((ResultResponseSuccess<TData>)response) : FallbackMinimalMatchHelper.OnSuccessFallback((ResultResponseSuccess<TData>)response)) :
            (onFailure is not null ? onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

        return TypeCasterInstance.Cast<TResult>(result);
    }

    public Task<TResult> MatchAsync<TResult>(
        ResultBase resultBase,
        Func<ResultResponseSuccess, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => MatchAsync(resultBase.ToResponse(), onSuccess, onFailure);

    public Task<TResult> MatchAsync<TResult, TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => MatchAsync<TResult, TData>(resultBase.ToResponse(), onSuccess, onFailure);

    public async Task<TResult> MatchAsync<TResult>(
        ResultResponse response,
        Func<ResultResponseSuccess, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
    {
        var result = response.IsSuccess
            ? (onSuccess is not null ? await onSuccess((ResultResponseSuccess)response) : OnSuccessFallback((ResultResponseSuccess)response))
            : (onFailure is not null ? await onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

        return TypeCasterInstance.Cast<TResult>(result);
    }

    public async Task<TResult> MatchAsync<TResult, TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider
    {
        var result = response.IsSuccess
            ? (onSuccess is not null ? await onSuccess((ResultResponseSuccess<TData>)response) : FallbackMinimalMatchHelper.OnSuccessFallback((ResultResponseSuccess<TData>)response))
            : (onFailure is not null ? await onFailure((ResultResponseError)response) : OnFailureFallback((ResultResponseError)response));

        return TypeCasterInstance.Cast<TResult>(result);
    }


    internal class TypeCaster
    {
        public virtual TResult Cast<TResult>(IResult result) where TResult : IResult
            => TypeCastHelper.TreatCast<TResult>(result);
    }
}
