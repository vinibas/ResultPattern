using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResponseResults;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class MatchResultsExtensions
{
    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IActionResult> onSuccess)
        => resultResponse.Match(onSuccess, response => response.ToProblemDetailsActionResult());

    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => resultResponse.IsSuccess ? onSuccess(resultResponse) : onFailure(resultResponse);
    
    public static IActionResult Match(this Result result,
        Func<ResultResponse, IActionResult> onSuccess)
        => result.ToActionResponse().Match(onSuccess);

    public static IActionResult Match(this Result result,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => result.ToActionResponse().Match(onSuccess, onFailure);
    
    public static IActionResult Match<T>(this Result<T> result,
        Func<ResultResponse, IActionResult> onSuccess)
        => result.ToActionResponse().Match(onSuccess);

    public static IActionResult Match<T>(this Result<T> result,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => result.ToActionResponse().Match(onSuccess, onFailure);
    
}
