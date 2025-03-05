using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

public static class MatchTResultsExtensions
{
    private static readonly Dictionary<Type, MethodInfo?> _implicitOperatorCache = new();

    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a Results<>, 
    /// but can be any type that implements IResult.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like TypedResults.Ok() or TypedResults.Created(), 
    /// but can be anything that implements IResult and is compatible with T_Result.</typeparam>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static T_Result Match<T_Result, T_Success>(this ResultResponse resultResponse,
        Func<ResultResponse, T_Result> onSuccess)
        where T_Result : IResult
        where T_Success : IResult
        => resultResponse.Match<T_Result, T_Success, IResult>(
            onSuccess, response => TreatCast<T_Result>(response.ToProblemDetailsResult()));

    private static T_Result TreatCast<T_Result>(ProblemHttpResult problemDetailsResult)
        where T_Result : IResult
    {
        var problemDetailsIResult = (IResult)problemDetailsResult;

        // First check the type, to try to avoid Reflection, for performance reasons
        if (problemDetailsIResult is T_Result problemDetailsTResult)
            return problemDetailsTResult;
        
        var implicitOperator = getImplicitOperatorFromCacheOrReflection<T_Result>();
        if (implicitOperator != null)
            return (T_Result)implicitOperator.Invoke(null, [ problemDetailsResult ])!;
        
        try
        {
            // Attempt to cast to classes with explicit conversion operators
            return (T_Result)problemDetailsIResult;
        }
        catch (InvalidCastException)
        {
            throw new InvalidOperationException(
                $"The type provided for T_Result ({typeof(T_Result).Name}) is not compatible " +
                $"with the failure result ({typeof(ProblemHttpResult).Name}). " + Environment.NewLine +
                "T_Result must be a type that can accept ProblemHttpResult or a compatible interface.");
        }
    }

    private static MethodInfo? getImplicitOperatorFromCacheOrReflection<T_Result>() where T_Result : IResult
    {
        Type typeOfT_Result = typeof(T_Result);
        
        if (!_implicitOperatorCache.TryGetValue(typeOfT_Result, out MethodInfo? implicitOperator))
        {
            implicitOperator = typeOfT_Result
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "op_Implicit" &&
                    m.ReturnType == typeOfT_Result &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(ProblemHttpResult));
            
            _implicitOperatorCache[typeOfT_Result] = implicitOperator;
        }

        return implicitOperator;
    }

    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a Results<>, 
    /// but can be any type that implements IResult.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like TypedResults.Ok() or TypedResults.Created(), 
    /// but can be anything that implements IResult and is compatible with T_Result.</typeparam>
    /// <typeparam name="T_Failure">The type of the result expected on failure. 
    /// Typically something like TypedResults.BadRequest() or TypedResults.NotFound(), 
    /// but can be anything that implements IResult and is compatible with T_Result.</typeparam>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static T_Result Match<T_Result, T_Success, T_Failure>(this ResultResponse resultResponse,
        Func<ResultResponse, T_Result> onSuccess, Func<ResultResponse, T_Result> onFailure)
        where T_Result : IResult
        where T_Success : IResult
        where T_Failure : IResult
        => resultResponse.IsSuccess ? onSuccess(resultResponse) : onFailure(resultResponse);
    
    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a Results<>, 
    /// but can be any type that implements IResult.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like TypedResults.Ok() or TypedResults.Created(), 
    /// but can be anything that implements IResult and is compatible with T_Result.</typeparam>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static T_Result Match<T_Result, T_Success>(this ResultBase result,
        Func<ResultResponse, T_Result> onSuccess)
        where T_Result : IResult
        where T_Success : IResult
        => result.ToActionResponse().Match<T_Result, T_Success>(onSuccess);

    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <typeparam name="T_Result">The type of the expected result. Typically a Results<>, 
    /// but can be any type that implements IResult.</typeparam>
    /// <typeparam name="T_Success">The type of the expected result on success. 
    /// Typically something like TypedResults.Ok() or TypedResults.Created(), 
    /// but can be anything that implements IResult and is compatible with T_Result.</typeparam>
    /// <typeparam name="T_Failure">The type of the result expected on failure. 
    /// Typically something like TypedResults.BadRequest() or TypedResults.NotFound(), 
    /// but can be anything that implements IResult and is compatible with T_Result.</typeparam>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static T_Result Match<T_Result, T_Success, T_Failure>(this ResultBase result,
        Func<ResultResponse, T_Result> onSuccess, Func<ResultResponse, T_Result> onFailure)
        where T_Result : IResult
        where T_Success : IResult
        where T_Failure : IResult
        => result.ToActionResponse().Match<T_Result, T_Success, IResult>(onSuccess, onFailure);
}