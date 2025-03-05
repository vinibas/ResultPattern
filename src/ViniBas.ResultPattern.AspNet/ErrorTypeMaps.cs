using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

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