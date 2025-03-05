using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMinimal;

public sealed class ResultsResultFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var endpointResult = await next(context);

        return endpointResult switch
        {
            Error error => error.ToProblemDetails(),
            IEnumerable<Error> errors => ((Error)errors.ToList()).ToProblemDetails(),
            ResultBase result => result.IsSuccess ?
                result.ToActionResponse() :
                result.ToActionResponse().ToProblemDetails(),
            ResultResponseError resultResponseError => resultResponseError.ToProblemDetails(),
            _ => endpointResult,
        };
    }
}

public static class ResultsResultFilterExtensions
{
    public static RouteHandlerBuilder WithResultsResultFilter(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<ResultsResultFilter>();
}