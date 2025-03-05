using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet;

public static class ModelStateExtensions
{
    public static IActionResult ToProblemDetailsActionResult(this ModelStateDictionary modelState)
        => modelState.ModelStateToError().ToProblemDetailsActionResult();

    public static Error ModelStateToError(this ModelStateDictionary modelState)
    {
        if (modelState.IsValid) throw new InvalidOperationException("Unable to convert a valid ModelState to a ProblemDetails.");

        var modelStateErrors = modelState.SelectMany(kv =>
        {
            if (kv.Value is null)
                return [ new Error.ErrorDetails(kv.Key, string.Empty) ];

            return kv.Value.Errors.Select(e => new Error.ErrorDetails(kv.Key, e.ErrorMessage));

        }).ToList();
        
        return new Error(modelStateErrors, ErrorTypes.Validation);
    }
}
