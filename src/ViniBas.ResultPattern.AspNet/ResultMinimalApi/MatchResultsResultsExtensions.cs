/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ViniBas.ResultPattern.AspNet.ResultMinimalApi;

public static class MatchResultsResultsExtensions
{
    private static ITypedResultMatcher Matcher => ResultMatcherFactory.GetTypedMatcher;

    public static Results<TResult1, TResult2> MatchResults<TResult1, TResult2>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Results<TResult1, TResult2>> onSuccess, Func<ResultResponseError, Results<TResult1, TResult2>> onFailure)
        where TResult1 : IResult, IEndpointMetadataProvider
        where TResult2 : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);
}
