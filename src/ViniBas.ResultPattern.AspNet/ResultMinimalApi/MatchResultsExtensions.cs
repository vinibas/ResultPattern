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
    private static ISimpleResultMatcher<IResult> Matcher => ResultMatcherFactory.GetMinimalApiMatcher;

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="IResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="IResult"/> on failure.</returns>
    public static IResult Match(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, IResult> onSuccess, bool? useProblemDetails = null)
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="IResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static IResult Match(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, IResult> onSuccess, Func<ResultResponseError, IResult> onFailure)
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="IResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="IResult"/> on failure.</returns>
    public static IResult Match<TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, IResult> onSuccess, bool? useProblemDetails = null)
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="IResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static IResult Match<TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, IResult> onSuccess, Func<ResultResponseError, IResult> onFailure)
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="IResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="IResult"/> on failure.</returns>
    public static IResult Match(this Result result,
        Func<ResultResponseSuccess, IResult> onSuccess, bool? useProblemDetails = null)
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="IResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static IResult Match(this Result result,
        Func<ResultResponseSuccess, IResult> onSuccess, Func<ResultResponseError, IResult> onFailure)
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="IResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="IResult"/> on failure.</returns>
    public static IResult Match<TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, IResult> onSuccess, bool? useProblemDetails = null)
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="IResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="IResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static IResult Match<TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, IResult> onSuccess, Func<ResultResponseError, IResult> onFailure)
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="Task{IResult}"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="Task{IResult}"/> on failure.</returns>
    public static Task<IResult> MatchAsync(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Task<IResult>> onSuccess, bool? useProblemDetails = null)
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<IResult> MatchAsync(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Task<IResult>> onSuccess, Func<ResultResponseError, Task<IResult>> onFailure)
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="IResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="IResult"/> on failure.</returns>
    public static Task<IResult> MatchAsync<TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, Task<IResult>> onSuccess, bool? useProblemDetails = null)
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<IResult> MatchAsync<TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, Task<IResult>> onSuccess, Func<ResultResponseError, Task<IResult>> onFailure)
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="Task{IResult}"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="Task{IResult}"/> on failure.</returns>
    public static Task<IResult> MatchAsync(this Result result,
        Func<ResultResponseSuccess, Task<IResult>> onSuccess, bool? useProblemDetails = null)
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<IResult> MatchAsync(this Result result,
        Func<ResultResponseSuccess, Task<IResult>> onSuccess, Func<ResultResponseError, Task<IResult>> onFailure)
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <see cref="IResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <see cref="IResult"/> on failure.</returns>
    public static Task<IResult> MatchAsync<TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, Task<IResult>> onSuccess, bool? useProblemDetails = null)
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{IResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<IResult> MatchAsync<TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, Task<IResult>> onSuccess, Func<ResultResponseError, Task<IResult>> onFailure)
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);
}
