/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

public static class MatchTResultsExtensions
{
    private static readonly Dictionary<TypesForImplicitOperatorCacheKey, MethodInfo?> _implicitOperatorCache = new();

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
        => resultResponse.Match<T_Result, T_Success, IResult>(
            onSuccess, response => TreatCast<T_Result>(MatchHelper.OnErrorDefault(response, useProblemDetails)));

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
        => resultResponse.IsSuccess ?
            TreatCast<T_Result>(onSuccess(resultResponse)) :
            TreatCast<T_Result>(onFailure(resultResponse));
    
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
        => result.ToResponse().Match<T_Result, T_Success>(onSuccess, useProblemDetails);

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
        => result.ToResponse().Match<T_Result, T_Success, T_Failure>(onSuccess, onFailure);

    private static T_Result TreatCast<T_Result>(IResult iresult)
        where T_Result : IResult
    {
        // First check the type, to try to avoid Reflection, for performance reasons
        if (iresult is T_Result tresult)
            return tresult;
        
        var implicitOperator = getImplicitOperatorFromCacheOrReflection<T_Result>(iresult.GetType());
        if (implicitOperator != null)
            return (T_Result)implicitOperator.Invoke(null, new object[] { iresult })!;
        
        try
        {
            // Attempt to cast to classes with explicit conversion operators
            return (T_Result)iresult;
        }
        catch (InvalidCastException)
        {
            throw new InvalidOperationException(
                $"The type provided for T_Result ({typeof(T_Result).Name}) is not compatible " +
                $"with the result ({iresult.GetType().Name}). " + Environment.NewLine +
                "T_Result must be a type that can accept the result or a compatible interface.");
        }
    }

    private static MethodInfo? getImplicitOperatorFromCacheOrReflection<T_Result>(Type tparam) where T_Result : IResult
    {
        Type typeOfT_Result = typeof(T_Result);
        var key = new TypesForImplicitOperatorCacheKey(typeOfT_Result, tparam);
        
        if (!_implicitOperatorCache.TryGetValue(key, out MethodInfo? implicitOperator))
        {
            implicitOperator = typeOfT_Result
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "op_Implicit" &&
                    m.ReturnType == typeOfT_Result &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == tparam);
            
            _implicitOperatorCache[key] = implicitOperator;
        }

        return implicitOperator;
    }
    
    private record TypesForImplicitOperatorCacheKey(Type typeOfT_Result, Type param);
}