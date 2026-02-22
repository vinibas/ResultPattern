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
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ViniBas.ResultPattern.AspNet.ResultMinimalApi;

public static class MatchTResultsExtensions
{
    private static ITypedResultMatcher Matcher => ResultMatcherFactory.GetTypedMatcher;

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static TResult Match<TResult>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, TResult> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static TResult Match<TResult>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, TResult> onSuccess, Func<ResultResponseError, TResult> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static TResult Match<TResult, TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, TResult> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static TResult Match<TResult, TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, TResult> onSuccess, Func<ResultResponseError, TResult> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>; if null, the behavior is determined by
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static TResult Match<TResult>(this Result result,
        Func<ResultResponseSuccess, TResult> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static TResult Match<TResult>(this Result result,
        Func<ResultResponseSuccess, TResult> onSuccess, Func<ResultResponseError, TResult> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>; if null, the behavior is determined by
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static TResult Match<TResult, TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, TResult> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <typeparamref name="TResult"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static TResult Match<TResult, TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, TResult> onSuccess, Func<ResultResponseError, TResult> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static Task<TResult> MatchAsync<TResult>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Task<TResult>> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<TResult> MatchAsync<TResult>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess, Task<TResult>> onSuccess, Func<ResultResponseError, Task<TResult>> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>;
    /// if null, the behavior is determined by <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static Task<TResult> MatchAsync<TResult, TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, Task<TResult>> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="ResultResponse"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<TResult> MatchAsync<TResult, TData>(this ResultResponse resultResponse,
        Func<ResultResponseSuccess<TData>, Task<TResult>> onSuccess, Func<ResultResponseError, Task<TResult>> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            resultResponse,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>; if null, the behavior is determined by
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static Task<TResult> MatchAsync<TResult>(this Result result,
        Func<ResultResponseSuccess, Task<TResult>> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <param name="result">The <see cref="Result"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<TResult> MatchAsync<TResult>(this Result result,
        Func<ResultResponseSuccess, Task<TResult>> onSuccess, Func<ResultResponseError, Task<TResult>> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess)rr),
            rr => onFailure((ResultResponseError)rr),
            null);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure;
    /// if false, returns an error <typeparamref name="TResult"/>; if null, the behavior is determined by
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success,
    /// or an error <typeparamref name="TResult"/> on failure.</returns>
    public static Task<TResult> MatchAsync<TResult, TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, Task<TResult>> onSuccess, bool? useProblemDetails = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            null,
            useProblemDetails);

    /// <summary>
    /// Asynchronously checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>,
    /// but can be any type that implements <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.</typeparam>
    /// <typeparam name="TData">Type of the success data value.</typeparam>
    /// <param name="result">The <see cref="Result{TData}"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <param name="onFailure">Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Task{TResult}"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.</returns>
    public static Task<TResult> MatchAsync<TResult, TData>(this Result<TData> result,
        Func<ResultResponseSuccess<TData>, Task<TResult>> onSuccess, Func<ResultResponseError, Task<TResult>> onFailure)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.MatchAsync(
            result,
            rr => onSuccess((ResultResponseSuccess<TData>)rr),
            rr => onFailure((ResultResponseError)rr),
            null);
}
