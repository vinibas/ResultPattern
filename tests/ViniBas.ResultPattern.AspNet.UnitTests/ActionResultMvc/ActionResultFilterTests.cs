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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class ActionResultFilterTests
{
    private readonly ActionExecutedContext _context;
    private readonly ActionResultFilter _filter;
    private readonly ResultsToTestDataBuilder _dataB = new ();

    public ActionResultFilterTests()
    {
        _filter = new ActionResultFilter();

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("", "error");
        
        _context = new ActionExecutedContext(
            new ActionContext(
                httpContext: new DefaultHttpContext(),
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
        GlobalConfiguration.UseProblemDetails = true;

        foreach (var resultErrorTst in _dataB._resultErrorsToTest)
        {
            _context.Result = resultErrorTst.ResultToTestAsObjectResult();

            _filter.OnActionExecuted(_context);

            var result = Assert.IsType<ObjectResult>(_context.Result);
            var probDet = Assert.IsType<ProblemDetails>(result.Value);
            Assert.Equal(resultErrorTst.ExpectedStatusCode, probDet.Status);
            Assert.Equal(resultErrorTst.ExpectedMessages, probDet.Extensions["errors"]);
        }
    }
    
    [Fact]
    public void OnActionExecuted_ShouldConvertFailuresToResultResponseError()
    {
        GlobalConfiguration.UseProblemDetails = false;

        foreach (var resultErrorTst in _dataB._resultErrorsToTest)
        {
            _context.Result = resultErrorTst.ResultToTestAsObjectResult();

            _filter.OnActionExecuted(_context);

            var result = Assert.IsType<ObjectResult>(_context.Result);
            var rre = Assert.IsType<ResultResponseError>(result.Value);
            Assert.Equal(resultErrorTst.ExpectedType, rre.Type);
            Assert.Equal(resultErrorTst.ExpectedMessages, rre.Errors);
        }
    }

    [Fact]
    public void OnActionExecuted_ShouldConvertSuccessToActionResponse()
    {
        foreach (var successToTest in _dataB._resultSuccessesToTest)
        {
            _context.Result = successToTest.ResultToTestAsObjectResult();

            _filter.OnActionExecuted(_context);

            var objRes = Assert.IsType<ObjectResult>(_context.Result);
            if (successToTest.Data is null)
                Assert.IsType<ResultResponseSuccess>(objRes.Value);
            else
            {
                var resSuc = Assert.IsType<ResultResponseSuccess<object>>(objRes.Value);
                Assert.Equal(successToTest.Data, resSuc.Data);
            }
        }
    }

    [Fact]
    public void OnActionExecuted_ShouldNotModifyProblemDetails()
    {
        var problemDetails = new ProblemDetails
        {
            Title = "ProbDet Tst",
            Status = 409,
            Type = "Tst",
            Extensions = new Dictionary<string, object?>() { { "errors", new List<string> { "error test" } } },
        };
    
        _context.Result = new ObjectResult(problemDetails);

        _filter.OnActionExecuted(_context);

        var result = Assert.IsType<ObjectResult>(_context.Result);
        var probDet = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(problemDetails, probDet);
        Assert.Equal("Tst", probDet.Type);
        Assert.Equal(new List<string> { "error test" }, probDet.Extensions["errors"]);
    }

    [Fact]
    public void OnActionExecuted_OtherObject_ReturnsOriginalObject()
    {
        var obj = new { Test = "Value" };
        
        _context.Result = new ObjectResult(obj);
        _filter.OnActionExecuted(_context);

        var result = Assert.IsType<ObjectResult>(_context.Result);
        Assert.Same(obj, result.Value);
    }
}
