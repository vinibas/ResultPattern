/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

public class ResultsResultFilterTests
{
    [Fact]
    public async Task InvokeAsync_Error_ReturnsProblemDetails()
    {
        var error = Error.Failure("TestError", "Error happened");
        
        var result = await InvokeFilter(error);
        
        var problemDetails = Assert.IsType<ProblemDetails>(result);
        Assert.Equal(500, problemDetails.Status);
        Assert.Contains("Error happened", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_Errors_ReturnsProblemDetails()
    {
        List<Error> errors = [
            Error.Validation("Error1", "Description 1"),
            Error.Validation("Error2", "Description 2"),
        ];
        
        var result = await InvokeFilter(errors);

        var problemDetails = Assert.IsType<ProblemDetails>(result);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal("Description 1" + Environment.NewLine + "Description 2", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResult_ReturnsResultResponse()
    {
        var result = Result.Success();
        var resultT = Result.Success("TestSuccess");

        var response = await InvokeFilter(result);
        var responseT = await InvokeFilter(resultT);

        var resultResponseSuccess = Assert.IsType<ResultResponseSuccess>(response);
        var resultResponseSuccessT = Assert.IsType<ResultResponseSuccess<string>>(responseT);
        
        Assert.True(resultResponseSuccess.IsSuccess);
        Assert.True(resultResponseSuccessT.IsSuccess);
    }

    [Fact]
    public async Task InvokeAsync_FailureResult_ReturnsProblemDetails()
    {
        var result = Result.Failure(Error.Conflict("TestConflict", "Conflict message"));

        var response = await InvokeFilter(result);

        var problemDetails = Assert.IsType<ProblemDetails>(response);
        Assert.Equal(409, problemDetails.Status);
        Assert.Contains("Conflict message", problemDetails.Detail);
    }

    [Fact]
    public async Task InvokeAsync_ResultResponseError_ReturnsProblemDetails()
    {
        var error = new ResultResponseError([ "Error 1", "Error 2" ], ErrorTypes.NotFound);

        var result = await InvokeFilter(error);
        
        var problemDetails = Assert.IsType<ProblemDetails>(result);
        Assert.Equal(404, problemDetails.Status);
        Assert.Equal("Error 1" + Environment.NewLine + "Error 2", problemDetails.Detail);
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
