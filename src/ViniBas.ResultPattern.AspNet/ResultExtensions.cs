using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.ResponseResults;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class ResultsExtensions
{
    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IActionResult> onSuccess)
        => resultResponse.Match(onSuccess, response => response.ToProblemDetailsActionResult());

    public static IActionResult Match(this ResultResponse resultResponse,
        Func<ResultResponse, IActionResult> onSuccess, Func<ResultResponse, IActionResult> onFailure)
        => resultResponse.IsSuccess ? onSuccess(resultResponse) : onFailure(resultResponse);

    public static ProblemDetails ToProblemDetails(this ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess || resultResponse is not ResultResponseError resultResponseError)
            throw new InvalidOperationException();

        return new ProblemDetails()
        {
            Title = GetTitle(resultResponseError.Type),
            Status = GetStatusCode(resultResponseError.Type),
            Detail = string.Join(Environment.NewLine, resultResponseError.Errors),
            Extensions =
            {
                ["success"] = false,
                ["errors"] = resultResponseError.Errors,
            },
        };

        static int GetStatusCode(ErrorType errorType)
            => errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

        static string GetTitle(ErrorType errorType)
            => errorType switch
            {
                ErrorType.Validation => "Bad Request",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                _ => "Server Failure",
            };
    }

    public static IActionResult ToProblemDetailsActionResult(this ResultResponse resultResponse)
        => ProblemDetailsToActionResult(resultResponse.ToProblemDetails());

    public static ProblemDetails ToProblemDetails(this Error error)
        => ((Result) error).ToActionResponse().ToProblemDetails();

    public static IActionResult ToProblemDetailsActionResult(this Error error)
        => ProblemDetailsToActionResult(error.ToProblemDetails());

    private static IActionResult ProblemDetailsToActionResult(ProblemDetails problemDetails)
        => new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
        };

    public static IActionResult ToProblemDetailsActionResult(this ModelStateDictionary modelState)
        => ModelStateToError(modelState).ToProblemDetailsActionResult();

    private static Error ModelStateToError(ModelStateDictionary modelState)
    {
        if (modelState.IsValid) throw new InvalidOperationException();

        var modelStateErrors = modelState.SelectMany(kv =>
        {
            if (kv.Value is null)
                return [ new Error.ErrorDetails(kv.Key, string.Empty) ];

            return kv.Value.Errors.Select(e => new Error.ErrorDetails(kv.Key, e.ErrorMessage));

        }).ToList();
        
        return new Error(modelStateErrors, ErrorType.Validation);
    }
}
