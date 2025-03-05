using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this ResultResponse resultResponse)
    {
        if (resultResponse.IsSuccess || resultResponse is not ResultResponseError resultResponseError)
            throw new InvalidOperationException("Unable to convert a valid ResultResponse to a ProblemDetails.");

        return new ProblemDetails()
        {
            Title = GetTitle(resultResponseError.Type),
            Status = GetStatusCode(resultResponseError.Type),
            Detail = string.Join(Environment.NewLine, resultResponseError.Errors),
            Extensions =
            {
                ["isSuccess"] = false,
                ["errors"] = resultResponseError.Errors,
            },
        };
    }

    private static int GetStatusCode(string errorType)
        => ErrorTypeMaps.Maps.TryGetValue(errorType, out var map) ?
            map.StatusCode : StatusCodes.Status500InternalServerError;

    private static string GetTitle(string errorType)
        => ErrorTypeMaps.Maps.TryGetValue(errorType, out var map) ?
            map.Title : "Server Failure";
}
