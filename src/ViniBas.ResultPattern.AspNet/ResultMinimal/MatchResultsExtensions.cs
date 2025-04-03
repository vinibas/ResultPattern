/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using Microsoft.AspNetCore.Http;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

public static class MatchResultsExtensions
{
    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static IResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IResult> onSuccess, bool? useProblemDetails = null)
        => resultResponse.Match(onSuccess, response => MatchHelper.OnErrorDefault(response, useProblemDetails));

    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static IResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IResult> onSuccess, Func<ResultResponse, IResult> onFailure)
        => resultResponse.IsSuccess ? onSuccess(resultResponse) : onFailure(resultResponse);
    
    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static IResult Match(this ResultBase result,
        Func<ResultResponse, IResult> onSuccess, bool? useProblemDetails = null)
        => result.ToResponse().Match(onSuccess, useProblemDetails);

    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static IResult Match(this ResultBase result,
        Func<ResultResponse, IResult> onSuccess, Func<ResultResponse, IResult> onFailure)
        => result.ToResponse().Match(onSuccess, onFailure);
}
