using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class ActionResultFilterTests
{
    private readonly ActionExecutedContext _context;
    private readonly ObjectResult _errorOR;
    private readonly ObjectResult _errorsOR;
    private readonly ObjectResult _resultSuccessOR;
    private readonly ObjectResult _resultSuccessTOR;
    private readonly ObjectResult _resultFailureOR;
    private readonly ObjectResult _resultResponseErrorOR;
    private readonly ObjectResult _problemDetailsOR;
    private readonly ActionResultFilter _filter;

    public ActionResultFilterTests()
    {
        var error = new Error("code", "description", ErrorTypes.Validation);

        _errorOR = new ObjectResult(error);
        _errorsOR = new ObjectResult(new List<Error> { error });
        _resultSuccessOR = new ObjectResult(Result.Success());
        _resultSuccessTOR = new ObjectResult(Result<Object>.Success(new ()));
        _resultFailureOR = new ObjectResult(Result.Failure(error));
        _resultResponseErrorOR = new ObjectResult(new ResultResponseError([ "error" ], ErrorTypes.Failure));
        _problemDetailsOR = new ObjectResult(new ProblemDetails());

        _filter = new ActionResultFilter();

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("", "error");
        var httpContext = new DefaultHttpContext();
        _context = new ActionExecutedContext(
            new ActionContext(
                httpContext: httpContext,
                routeData: new RouteData(),
                actionDescriptor: new ActionDescriptor(),
                modelState: modelState
            ),
            [],
            new Dictionary<string, object>());
    }
    
    [Fact]
    public void OnActionExecuted_ShouldConvertFailuresToProblemDetails()
    {
        var failures = new List<ObjectResult>
        {
            _errorOR,
            _errorsOR,
            _resultFailureOR,
            _resultResponseErrorOR,
        };
            
        foreach (var failure in failures)
        {
            _context.Result = failure;

            _filter.OnActionExecuted(_context);

            var result = Assert.IsType<ObjectResult>(_context.Result);
            Assert.IsType<ProblemDetails>(result.Value);
        }
    }

    [Fact]
    public void OnActionExecuted_ShouldConvertSuccessToActionResponse()
    {
        var successes = new List<(ObjectResult, Type)>
        {
            (_resultSuccessOR, typeof(ResultResponseSuccess)),
            (_resultSuccessTOR, typeof(ResultResponseSuccess<Object>)),
        };

        foreach (var success in successes)
        {
            _context.Result = success.Item1;

            _filter.OnActionExecuted(_context);

            var result = Assert.IsType<ObjectResult>(_context.Result);
            Assert.IsType(success.Item2, result.Value);
        }
    }

    [Fact]
    public void OnActionExecuted_ShouldNotModifyProblemDetails()
    {
        _context.Result = _problemDetailsOR;

        _filter.OnActionExecuted(_context);

        var result = Assert.IsType<ObjectResult>(_context.Result);
        Assert.Equal(_problemDetailsOR.Value, result.Value);
    }
}
