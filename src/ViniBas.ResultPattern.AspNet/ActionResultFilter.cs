using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public sealed class ActionResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null) return;

        var contextResult = context.Result;

        if (contextResult is ObjectResult objectResult &&
            objectResult.Value is not ProblemDetails)
        {
            objectResult.Value = objectResult.Value switch
            {
                Error error => error.ToProblemDetails(),
                IEnumerable<Error> errors => ((Error)errors.ToList()).ToProblemDetails(),
                ResultBase result => result.IsSuccess ?
                    result.ToActionResponse() :
                    result.ToActionResponse().ToProblemDetails(),
                ResultResponseError resultResponseError => resultResponseError.ToProblemDetails(),
                _ => objectResult.Value,
            };
        }
    }
}
