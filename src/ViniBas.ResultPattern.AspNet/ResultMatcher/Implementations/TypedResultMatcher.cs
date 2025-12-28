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

internal sealed class TypedResultMatcher : ITypedResultMatcher
{
    internal ITypedResultDefaultMatcher MatchDefaultInstance { get; set; } = new TypedResultDefaultMatcher();

    public TResult Match<TResult, TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult
        => MatchDefaultInstance.Match<TResult, TSuccess, TFailure>(
            resultBase.ToResponse(), onSuccess, onFailure, useProblemDetails);

    public TResult Match<TResult, TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult
        => MatchDefaultInstance.Match<TResult, TSuccess, TFailure>(
            response, onSuccess, onFailure, useProblemDetails);
}
