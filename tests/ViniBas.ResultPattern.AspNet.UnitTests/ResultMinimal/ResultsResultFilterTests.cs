/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

public class ResultsResultFilterTests
{
    private readonly ResultsToTestDataBuilder _dataB = new ();

    [Fact]
    public async Task InvokeAsync_Error_ReturnsProblemDetails()
    {
        GlobalConfiguration.UseProblemDetails = true;
        
        foreach (var resultErrorTst in _dataB._resultErrorsToTest)
        {
            var result = await InvokeFilter(resultErrorTst.ResultToTest);

            var probDet = Assert.IsType<ProblemDetails>(result);
            Assert.Equal(resultErrorTst.ExpectedStatusCode, probDet.Status);
            Assert.Equal(resultErrorTst.ExpectedMessages, probDet.Extensions["errors"]);
        }
    }

    [Fact]
    public async Task InvokeAsync_Error_ReturnsResultResponseError()
    {
        GlobalConfiguration.UseProblemDetails = false;

        foreach (var resultErrorTst in _dataB._resultErrorsToTest)
        {
            var result = await InvokeFilter(resultErrorTst.ResultToTest);

            var rre = Assert.IsType<ResultResponseError>(result);
            Assert.Equal(resultErrorTst.ExpectedType, rre.Type);
            Assert.Equal(resultErrorTst.ExpectedMessages, rre.Errors);
        }
    }


    [Fact]
    public async Task InvokeAsync_SuccessResult_ReturnsResultResponse()
    {
        foreach (var successToTest in _dataB._resultSuccessesToTest)
        {
            var response = await InvokeFilter(successToTest.ResultToTest);

            if (successToTest.Data is null)
                Assert.IsType<ResultResponseSuccess>(response);
            else
            {
                var resSuc = Assert.IsType<ResultResponseSuccess<object>>(response);
                Assert.Equal(successToTest.Data, resSuc.Data);
            }
        }
    }

    [Fact]
    public async Task OnActionExecuted_ShouldNotModifyProblemDetails()
    {
        var problemDetails = new ProblemDetails
        {
            Title = "ProbDet Tst",
            Status = 409,
            Type = "Tst",
            Extensions = new Dictionary<string, object?>() { { "errors", new List<string> { "error test" } } },
        };
    
        var response = await InvokeFilter(problemDetails);
        
        var probDet = Assert.IsType<ProblemDetails>(response);
        Assert.Equal(problemDetails, probDet);
        Assert.Equal("Tst", probDet.Type);
        Assert.Equal(new List<string> { "error test" }, probDet.Extensions["errors"]);
    }

    [Fact]
    public async Task InvokeAsync_OtherObject_ReturnsOriginalObject()
    {
        var obj = new { Test = "Value" };

        var result = await InvokeFilter(obj);
        
        Assert.Same(obj, result);
    }
    
    private static async Task<object?> InvokeFilter(object endpointResult)
    {
        var filter = new ResultsResultFilter();
        var httpContext = new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext, Array.Empty<object>(), new Endpoint(null, null, null));
        var next = new EndpointFilterDelegate(_ => new ValueTask<object?>(endpointResult));

        return await filter.InvokeAsync(context, next);
    }
}
