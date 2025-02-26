using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class ActionResultFilterTests
{
    private readonly ActionExecutedContext _context;
    private readonly ObjectResult _errorObjectResult;
    private readonly ObjectResult _problemDetailsObjectResult;
    private readonly ObjectResult _successObjectResult;
    private readonly ActionResultFilter _filter;

    public ActionResultFilterTests()
    {
        var error = new Error("code", "description", ErrorType.Validation);
        _errorObjectResult = new ObjectResult(error);
        _problemDetailsObjectResult = new ObjectResult(new ProblemDetails());
        _successObjectResult = new ObjectResult(Result.Success());

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
    public void OnActionExecuted_ShouldConvertErrorToProblemDetails()
    {
        _context.Result = _errorObjectResult;

        _filter.OnActionExecuted(_context);

        var result = Assert.IsType<ObjectResult>(_context.Result);
        Assert.IsType<ProblemDetails>(result.Value);
    }

    [Fact]
    public void OnActionExecuted_ShouldConvertResultToActionResponse()
    {
        _context.Result = _successObjectResult;

        _filter.OnActionExecuted(_context);

        var resultObject = Assert.IsType<ObjectResult>(_context.Result);
        Assert.IsType<ResultResponseSuccess>(resultObject.Value);
    }

    [Fact]
    public void OnActionExecuted_ShouldNotModifyProblemDetails()
    {
        _context.Result = _problemDetailsObjectResult;

        _filter.OnActionExecuted(_context);

        var result = Assert.IsType<ObjectResult>(_context.Result);
        Assert.Equal(_problemDetailsObjectResult.Value, result.Value);
    }
}
