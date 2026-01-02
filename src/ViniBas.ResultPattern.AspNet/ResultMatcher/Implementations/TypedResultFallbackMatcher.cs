/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

internal interface ITypedResultFallbackMatcher
{
    public TResult Match<TResult, TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult;
}

internal class TypedResultFallbackMatcher : ITypedResultFallbackMatcher
{
    internal Func<ResultResponse, IResult> OnSuccessFallback { get; set; }
        = FallbackMinimalMatchHelper.OnSuccessFallback;
    internal Func<ResultResponse, bool?, IResult> OnFailureFallback { get; set; }
        = FallbackMinimalMatchHelper.OnFailureFallback;
    internal TypeCaster TypeCasterInstance { get; set; } = new TypeCaster();

    public TResult Match<TResult, TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult
    {
        var result = response.IsSuccess ?
            (onSuccess is not null ? onSuccess(response) : OnSuccessFallback(response)) :
            (onFailure is not null ? onFailure(response) : OnFailureFallback(response, useProblemDetails));

        return TypeCasterInstance.Cast<TResult>(result);
    }

    internal class TypeCaster
    {
        public virtual TResult Cast<TResult>(IResult result) where TResult : IResult
            => TypeCastHelper.TreatCast<TResult>(result);
    }
}