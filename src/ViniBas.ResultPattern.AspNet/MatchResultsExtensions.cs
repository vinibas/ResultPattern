using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class MatchResultsExtensions
{
    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IActionResult> onSuccess)
        => resultResponse.Match(onSuccess, response => response.ToProblemDetailsActionResult());

    /// <summary>
    /// Checks whether a ResultResponse is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => resultResponse.IsSuccess ? onSuccess(resultResponse) : onFailure(resultResponse);
    
    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static IActionResult Match(this Result result,
        Func<ResultResponse, IActionResult> onSuccess)
        => result.ToActionResponse().Match(onSuccess);

    /// <summary>
    /// Checks whether a Result is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static IActionResult Match(this Result result,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => result.ToActionResponse().Match(onSuccess, onFailure);
    
    /// <summary>
    /// Checks whether a Result<T> is a success or failure, and returns the result of the function if successful.
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <returns>Returns the result of the onSuccess function on success, or a ProblemDetails on failure.</returns>
    public static IActionResult Match<T>(this Result<T> result,
        Func<ResultResponse, IActionResult> onSuccess)
        => result.ToActionResponse().Match(onSuccess);

    /// <summary>
    /// Checks whether a Result<T> is a success or failure, and returns the result of the corresponding function
    /// </summary>
    /// <param name="onSuccess">Function to be executed in case of success.</param>
    /// <param name="onFailure">Function to be executed in case of failure</param>
    /// <returns>Returns the result of the onSuccess function in case of success, or of onFailure in case of failure.</returns>
    public static IActionResult Match<T>(this Result<T> result,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => result.ToActionResponse().Match(onSuccess, onFailure);
    
}
