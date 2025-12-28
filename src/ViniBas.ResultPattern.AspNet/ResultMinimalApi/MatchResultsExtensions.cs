/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.ResultMinimalApi;

public static class MatchResultsExtensions
{
    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; 
    /// if false, returns an error <see cref="IResult"/>; if null, the behavior is determined by 
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success, or an error result on failure.</returns>
    public static IResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IResult> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetMinimalApiMatcher.Match<IResult, IResult>(
            resultResponse, onSuccess, null, useProblemDetails);

    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success, or of <paramref name="onFailure"/> in case of failure.</returns>
    public static IResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IResult> onSuccess, Func<ResultResponse, IResult> onFailure)
        => ResultMatcherFactory.GetMinimalApiMatcher.Match(resultResponse, onSuccess, onFailure, null);
    
    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="result">The Result to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; 
    /// if false, returns an error <see cref="IResult"/>; if null, the behavior is determined by 
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success, or an error result on failure.</returns>
    public static IResult Match(this ResultBase result,
        Func<ResultResponse, IResult> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetMinimalApiMatcher.Match<IResult, IResult>(
            result, onSuccess, null, useProblemDetails);

    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="result">The Result to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success, or of <paramref name="onFailure"/> in case of failure.</returns>
    public static IResult Match(this ResultBase result,
        Func<ResultResponse, IResult> onSuccess, Func<ResultResponse, IResult> onFailure)
        => ResultMatcherFactory.GetMinimalApiMatcher.Match(result, onSuccess, onFailure, null);
}
