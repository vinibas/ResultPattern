/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.ActionResultMvc;

public static class MatchResultsExtensions
{
    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; if false, returns an error <see cref="IActionResult"/>; if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, IActionResult> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.Match<IActionResult, IActionResult>(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure, and returns the result of the corresponding function.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure. Receives a <see cref="ResultResponseError"/>.</param>
    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, IActionResult> onSuccess, Func<ResultResponseError, IActionResult> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> that may contain a typed value is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="T">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess{T}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; if false, returns an error <see cref="IActionResult"/>; if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    public static IActionResult Match<T>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<T>, IActionResult> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.Match<IActionResult, IActionResult>(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure, and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="T">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess{T}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure. Receives a <see cref="ResultResponseError"/>.</param>
    public static IActionResult Match<T>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<T>, IActionResult> onSuccess, Func<ResultResponseError, IActionResult> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="Result"/> (non-generic) is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; if false, returns an error <see cref="IActionResult"/>; if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    public static IActionResult Match(this Result result,
        Func<ResultResponseSuccess, IActionResult> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.Match<IActionResult, IActionResult>(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure, and returns the result of the corresponding function.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure. Receives a <see cref="ResultResponseError"/>.</param>
    public static IActionResult Match(this Result result,
        Func<ResultResponseSuccess, IActionResult> onSuccess, Func<ResultResponseError, IActionResult> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="Result{T}"/> is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="T">Type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess{T}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; if false, returns an error <see cref="IActionResult"/>; if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    public static IActionResult Match<T>(this Result<T> result,
        Func<ResultResponseSuccess<T>, IActionResult> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.Match<IActionResult, IActionResult>(
            result,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="Result{T}"/> is a success or failure, and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="T">Type of the result value.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success. Receives a <see cref="ResultResponseSuccess{T}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure. Receives a <see cref="ResultResponseError"/>.</param>
    public static IActionResult Match<T>(this Result<T> result,
        Func<ResultResponseSuccess<T>, IActionResult> onSuccess, Func<ResultResponseError, IActionResult> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Async version of <see cref="Match(ResultResponse, Func{ResultResponseSuccess, IActionResult}, bool?)"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Task<IActionResult>> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync<IActionResult, IActionResult>(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Async version of <see cref="Match(ResultResponse, Func{ResultResponseSuccess, IActionResult}, Func{ResultResponseError, IActionResult})"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Task<IActionResult>> onSuccess, Func<ResultResponseError, Task<IActionResult>> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Async version of <see cref="Match{T}(ResultResponse, Func{ResultResponseSuccess{T}, IActionResult}, bool?)"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync<T>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<T>, Task<IActionResult>> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync<IActionResult, IActionResult>(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Async version of <see cref="Match(Result, Func{ResultResponseSuccess, IActionResult}, bool?)"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync(this Result result,
        Func<ResultResponseSuccess, Task<IActionResult>> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync<IActionResult, IActionResult>(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Async version of <see cref="Match(Result, Func{ResultResponseSuccess, IActionResult}, Func{ResultResponseError, IActionResult})"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync(this Result result,
        Func<ResultResponseSuccess, Task<IActionResult>> onSuccess, Func<ResultResponseError, Task<IActionResult>> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Async version of <see cref="Match{T}(Result{T}, Func{ResultResponseSuccess{T}, IActionResult}, bool?)"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync<T>(this Result<T> result,
        Func<ResultResponseSuccess<T>, Task<IActionResult>> onSuccess, bool? useProblemDetails = null)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync<IActionResult, IActionResult>(
            result,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Async version of <see cref="Match{T}(Result{T}, Func{ResultResponseSuccess{T}, IActionResult}, Func{ResultResponseError, IActionResult})"/>.
    /// </summary>
    public static Task<IActionResult> MatchAsync<T>(this Result<T> result,
        Func<ResultResponseSuccess<T>, Task<IActionResult>> onSuccess, Func<ResultResponseError, Task<IActionResult>> onFailure)
        => ResultMatcherFactory.GetActionResultMatcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess<T>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);
}
