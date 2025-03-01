using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class ProblemDetailsExtensions
{
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

        static int GetStatusCode(string errorType)
            => ErrorTypeMaps.Maps.TryGetValue(errorType, out var map) ?
                map.StatusCode : StatusCodes.Status500InternalServerError;

        static string GetTitle(string errorType)
            => ErrorTypeMaps.Maps.TryGetValue(errorType, out var map) ?
                map.Title : "Server Failure";
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
        
        return new Error(modelStateErrors, ErrorTypes.Validation);
    }
}

public static class ErrorTypeMaps
{
    public static IDictionary<string, (int StatusCode, string Title)> Maps { get; } =
        new Dictionary<string, (int StatusCode, string Title)>()
        {
            [ErrorTypes.Failure] = (StatusCodes.Status500InternalServerError, "Server Failure"),
            [ErrorTypes.Validation] = (StatusCodes.Status400BadRequest, "Bad Request"),
            [ErrorTypes.NotFound] = (StatusCodes.Status404NotFound, "Not Found"),
            [ErrorTypes.Conflict] = (StatusCodes.Status409Conflict, "Conflict"),
        };
}