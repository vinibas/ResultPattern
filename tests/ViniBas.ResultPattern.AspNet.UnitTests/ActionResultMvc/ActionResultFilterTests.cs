/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class ActionResultFilterTests
{
    private readonly ActionExecutedContext _context;
    private readonly ActionResultFilter _filter;
    private readonly Mock<IFilterMappings> _mockFilterMappings = new ();

    public ActionResultFilterTests()
    {
        _filter = new() { filterMappings = _mockFilterMappings.Object };

        _context = new ActionExecutedContext(
            new ActionContext(
                httpContext: new DefaultHttpContext(),
                routeData: new RouteData(),
                actionDescriptor: new ActionDescriptor(),
                modelState: []
            ), [],
            new Dictionary<string, object>());
    }

    [Fact]
    public void OnActionExecuted_ShouldDoNothing_IfThereIsAnException()
    {
        var exception = new Exception("Test exception");
        _context.Exception = exception;

        _filter.OnActionExecuted(_context);

        Assert.Same(exception, _context.Exception);
        _mockFilterMappings.VerifyNoOtherCalls();
    }

    [Fact]
    public void OnActionExecuted_ShouldDoNothing_IfContextResultIsNotObjectResultOrIsProblemDetails()
    {
        var otherResult = new ContentResult
        {
            Content = "Some content",
            StatusCode = 200
        };
        _context.Result = otherResult;

        _filter.OnActionExecuted(_context);
        
        _mockFilterMappings.VerifyNoOtherCalls();

        var problemDetailsResult = new ObjectResult(new ProblemDetails
        {
            Title = "Some problem details",
            Status = 400
        });
        _context.Result = problemDetailsResult;
        
        _filter.OnActionExecuted(_context);
        
        _mockFilterMappings.VerifyNoOtherCalls();
    }

    [Fact]
    public void OnActionExecuted_ShouldCallMapResult_IfResultValueIsObjectResultAndNotProblemDetails()
    {
        var objectResult = new ObjectResult(new { Test = "Value1" });
        var expectedResult = new ObjectResult(new { Test = "Value2" });

        _context.Result = objectResult;
        _mockFilterMappings
            .Setup(fm => fm.MapToResultResponse(objectResult.Value))
            .Returns(expectedResult);

        _filter.OnActionExecuted(_context);

        Assert.Same(expectedResult, ((ObjectResult)_context.Result).Value);
        _mockFilterMappings.Verify(fm => fm.MapToResultResponse(It.IsAny<object>()), Times.Once);
    }
}
