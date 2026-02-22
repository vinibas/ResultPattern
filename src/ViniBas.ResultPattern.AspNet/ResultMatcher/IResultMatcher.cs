/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet;

internal interface ISimpleResultMatcher<TResult>
{
    TResult Match(
        ResultBase resultBase,
        Func<ResultResponse, TResult>? onSuccess,
        Func<ResultResponse, TResult>? onFailure,
        bool? useProblemDetails);

    TResult Match(
        ResultResponse response,
        Func<ResultResponse, TResult>? onSuccess,
        Func<ResultResponse, TResult>? onFailure,
        bool? useProblemDetails);

    Task<TResult> MatchAsync(
        ResultBase resultBase,
        Func<ResultResponse, Task<TResult>>? onSuccess,
        Func<ResultResponse, Task<TResult>>? onFailure,
        bool? useProblemDetails);

    Task<TResult> MatchAsync(
        ResultResponse response,
        Func<ResultResponse, Task<TResult>>? onSuccess,
        Func<ResultResponse, Task<TResult>>? onFailure,
        bool? useProblemDetails);
}

internal interface ITypedResultMatcher
{
    TResult Match<TResult>(
        ResultBase resultBase,
        Func<ResultResponse, TResult>? onSuccess,
        Func<ResultResponse, TResult>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider;

    TResult Match<TResult>(
        ResultResponse response,
        Func<ResultResponse, TResult>? onSuccess,
        Func<ResultResponse, TResult>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider;

    Task<TResult> MatchAsync<TResult>(
        ResultBase resultBase,
        Func<ResultResponse, Task<TResult>>? onSuccess,
        Func<ResultResponse, Task<TResult>>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider;

    Task<TResult> MatchAsync<TResult>(
        ResultResponse response,
        Func<ResultResponse, Task<TResult>>? onSuccess,
        Func<ResultResponse, Task<TResult>>? onFailure,
        bool? useProblemDetails)
        where TResult : IResult, IEndpointMetadataProvider;
}
