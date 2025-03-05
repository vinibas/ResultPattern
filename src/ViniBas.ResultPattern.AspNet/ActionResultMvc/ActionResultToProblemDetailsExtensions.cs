using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ActionResultMvc;

public static class ActionResultToProblemDetailsExtensions
{
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
}
