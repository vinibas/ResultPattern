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

public static class MatchTResultsExtensions
{
    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>, 
    /// but can be any type that implements <see cref="IResult"/>.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like <see cref="TypedResults.Ok()"/> or <see cref="TypedResults.Created()"/>, 
    /// but can be anything that implements <see cref="IResult"/> and is compatible with <typeparamref name="T_Result"/>.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; 
    /// if false, returns an error <see cref="IResult"/>; if null, the behavior is determined by 
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success, or an error result on failure.</returns>
    public static T_Result Match<T_Result, T_Success>(this ResultResponse resultResponse,
        Func<ResultResponse, T_Success> onSuccess, bool? useProblemDetails = null)
        where T_Result : IResult
        where T_Success : IResult
        => ResultMatcherFactory.GetTypedMatcher.Match<T_Result, T_Success, IResult>(
            resultResponse, onSuccess, null, useProblemDetails);

    /// <summary>
    /// Checks whether a <see cref="ResultResponse"/> is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>, 
    /// but can be any type that implements <see cref="IResult"/>.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like <see cref="TypedResults.Ok()"/> or <see cref="TypedResults.Created()"/>, 
    /// but can be anything that implements <see cref="IResult"/> and is compatible with <typeparamref name="T_Result"/>.</typeparam>
    /// <typeparam name="T_Failure">The type of the result expected on failure. 
    /// Typically something like <see cref="TypedResults.BadRequest()"/> or <see cref="TypedResults.NotFound()"/>, 
    /// but can be anything that implements <see cref="IResult"/> and is compatible with <typeparamref name="T_Result"/>.</typeparam>
    /// <param name="resultResponse">The <see cref="ResultResponse"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success, or of <paramref name="onFailure"/> in case of failure.</returns>
    public static T_Result Match<T_Result, T_Success, T_Failure>(this ResultResponse resultResponse,
        Func<ResultResponse, T_Success> onSuccess, Func<ResultResponse, T_Failure> onFailure)
        where T_Result : IResult
        where T_Success : IResult
        where T_Failure : IResult
        => ResultMatcherFactory.GetTypedMatcher.Match<T_Result, T_Success, T_Failure>(
            resultResponse, onSuccess, onFailure, null);
    
    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>, 
    /// but can be any type that implements <see cref="IResult"/>.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like <see cref="TypedResults.Ok()"/> or <see cref="TypedResults.Created()"/>, 
    /// but can be anything that implements <see cref="IResult"/> and is compatible with <typeparamref name="T_Result"/>.</typeparam>
    /// <param name="result">The <see cref="ResultBase"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="useProblemDetails">If true, returns a <see cref="ProblemDetails"/> on failure; 
    /// if false, returns an error <see cref="IResult"/>; if null, the behavior is determined by 
    /// <see cref="GlobalConfiguration.UseProblemDetails"/>.</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function on success, or an error result on failure.</returns>
    public static T_Result Match<T_Result, T_Success>(this ResultBase result,
        Func<ResultResponse, T_Success> onSuccess, bool? useProblemDetails = null)
        where T_Result : IResult
        where T_Success : IResult
        => ResultMatcherFactory.GetTypedMatcher.Match<T_Result, T_Success, IResult>(
            result, onSuccess, null, useProblemDetails);

    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a <see cref="Results{T1, T2}"/>, 
    /// but can be any type that implements <see cref="IResult"/>.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like <see cref="TypedResults.Ok()"/> or <see cref="TypedResults.Created()"/>, 
    /// but can be anything that implements <see cref="IResult"/> and is compatible with <typeparamref name="T_Result"/>.</typeparam>
    /// <typeparam name="T_Failure">The type of the result expected on failure. 
    /// Typically something like <see cref="TypedResults.BadRequest()"/> or <see cref="TypedResults.NotFound()"/>, 
    /// but can be anything that implements <see cref="IResult"/> and is compatible with <typeparamref name="T_Result"/>.</typeparam>
    /// <param name="result">The <see cref="ResultBase"/> to evaluate.</param>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the <paramref name="onSuccess"/> function in case of success, or of <paramref name="onFailure"/> in case of failure.</returns>
    public static T_Result Match<T_Result, T_Success, T_Failure>(this ResultBase result,
        Func<ResultResponse, T_Success> onSuccess, Func<ResultResponse, T_Failure> onFailure)
        where T_Result : IResult
        where T_Success : IResult
        where T_Failure : IResult
        => ResultMatcherFactory.GetTypedMatcher.Match<T_Result, T_Success, T_Failure>(
            result, onSuccess, onFailure, null);
}