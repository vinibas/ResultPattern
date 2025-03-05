using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

public static class ResultToProblemDetailsExtensions
{
    public static ProblemHttpResult ToProblemDetailsResult(this ResultResponse resultResponse)
        => ProblemDetailsToResult(resultResponse.ToProblemDetails());

    public static ProblemDetails ToProblemDetails(this Error error)
        => ((Result) error).ToActionResponse().ToProblemDetails();

    public static ProblemHttpResult ToProblemDetailsResult(this Error error)
        => ProblemDetailsToResult(error.ToProblemDetails());

    private static ProblemHttpResult ProblemDetailsToResult(ProblemDetails problemDetails)
        => TypedResults.Problem(problemDetails);
}