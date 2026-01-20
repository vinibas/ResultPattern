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

namespace ViniBas.ResultPattern.AspNet;

internal interface ISimpleResultMatcher<TResult>
{
    TResult Match<TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TSuccess : TResult
        where TFailure : TResult;

    TResult Match<TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TSuccess : TResult
        where TFailure : TResult;

    Task<TResult> MatchAsync<TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TSuccess : TResult
        where TFailure : TResult;

    Task<TResult> MatchAsync<TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TSuccess : TResult
        where TFailure : TResult;
}

internal interface ITypedResultMatcher
{
    TResult Match<TResult, TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult;

    TResult Match<TResult, TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, TSuccess>? onSuccess,
        Func<ResultResponse, TFailure>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult;

    Task<TResult> MatchAsync<TResult, TSuccess, TFailure>(
        ResultBase resultBase,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult;

    Task<TResult> MatchAsync<TResult, TSuccess, TFailure>(
        ResultResponse response,
        Func<ResultResponse, Task<TSuccess>>? onSuccess,
        Func<ResultResponse, Task<TFailure>>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult
        where TSuccess : IResult
        where TFailure : IResult;
}