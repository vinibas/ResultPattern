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

namespace ViniBas.ResultPattern.AspNet;

internal interface ISimpleResultMatcher<TResult>
{
    TResult Match(
        ResultBase resultBase,
        Func<ResultResponseSuccess, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure);

    TResult Match<TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure);

    TResult Match(
        ResultResponse response,
        Func<ResultResponseSuccess, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure);

    TResult Match<TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure);

    Task<TResult> MatchAsync(
        ResultBase resultBase,
        Func<ResultResponseSuccess, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure);

    Task<TResult> MatchAsync<TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure);

    Task<TResult> MatchAsync(
        ResultResponse response,
        Func<ResultResponseSuccess, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure);

    Task<TResult> MatchAsync<TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure);
}

internal interface ITypedResultMatcher
{
    TResult Match<TResult>(
        ResultBase resultBase,
        Func<ResultResponseSuccess, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    TResult Match<TResult, TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    TResult Match<TResult>(
        ResultResponse response,
        Func<ResultResponseSuccess, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    TResult Match<TResult, TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, TResult>? onSuccess,
        Func<ResultResponseError, TResult>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    Task<TResult> MatchAsync<TResult>(
        ResultBase resultBase,
        Func<ResultResponseSuccess, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    Task<TResult> MatchAsync<TResult, TData>(
        ResultBase resultBase,
        Func<ResultResponseSuccess<TData>, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    Task<TResult> MatchAsync<TResult>(
        ResultResponse response,
        Func<ResultResponseSuccess, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;

    Task<TResult> MatchAsync<TResult, TData>(
        ResultResponse response,
        Func<ResultResponseSuccess<TData>, Task<TResult>>? onSuccess,
        Func<ResultResponseError, Task<TResult>>? onFailure)
        where TResult : IResult, IEndpointMetadataProvider;
}
