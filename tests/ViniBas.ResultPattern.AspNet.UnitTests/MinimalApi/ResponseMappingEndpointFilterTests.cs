/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.MinimalApi;

namespace ViniBas.ResultPattern.AspNet.UnitTests.MinimalApi;

public class ResponseMappingEndpointFilterTests
{
    private readonly Mock<IFilterMappings> _mockFilterMappings = new ();

    [Fact]
    public async Task OnActionExecuted_ShouldCallMapResult()
    {
        var objectResult = new Object();
        var expectedResult = new Object();

        _mockFilterMappings
            .Setup(fm => fm.MapToResultResponse(objectResult))
            .Returns(expectedResult);

        var result = await InvokeFilter(objectResult);

        Assert.Same(expectedResult, result);
        _mockFilterMappings.Verify(fm => fm.MapToResultResponse(objectResult), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldRegisterOnStarting_WhenMappedResultHasProblemDetails()
    {
        var problemDetails = new ProblemDetails { Title = "Error", Status = 400 };
        var jsonResult = TypedResults.BadRequest(problemDetails);

        _mockFilterMappings
            .Setup(fm => fm.MapToResultResponse(It.IsAny<object>()))
            .Returns(jsonResult);

        var (mockResponse, httpContext) = CreateMockHttpContext();
        await InvokeFilter(new Object(), httpContext);

        mockResponse.Verify(r => r.OnStarting(It.IsAny<Func<Task>>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldNotRegisterOnStarting_WhenMappedResultIsProblemHttpResult()
    {
        var problemDetails = new ProblemDetails { Title = "Error", Status = 400 };
        var problemHttpResult = TypedResults.Problem(problemDetails);

        _mockFilterMappings
            .Setup(fm => fm.MapToResultResponse(It.IsAny<object>()))
            .Returns(problemHttpResult);

        var (mockResponse, httpContext) = CreateMockHttpContext();
        await InvokeFilter(new Object(), httpContext);

        mockResponse.Verify(r => r.OnStarting(It.IsAny<Func<Task>>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_ShouldRegisterOnStarting_WhenNestedResultHasProblemDetails()
    {
        var problemDetails = new ProblemDetails { Title = "Error", Status = 400 };
        Results<Ok, BadRequest<ProblemDetails>> wrappedResult = TypedResults.BadRequest(problemDetails);

        _mockFilterMappings
            .Setup(fm => fm.MapToResultResponse(It.IsAny<object>()))
            .Returns(wrappedResult);

        var (mockResponse, httpContext) = CreateMockHttpContext();
        await InvokeFilter(new Object(), httpContext);

        mockResponse.Verify(r => r.OnStarting(It.IsAny<Func<Task>>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldNotRegisterOnStarting_WhenNestedResultHasNoProblemDetails()
    {
        Results<Ok, BadRequest<ProblemDetails>> wrappedResult = TypedResults.Ok();

        _mockFilterMappings
            .Setup(fm => fm.MapToResultResponse(It.IsAny<object>()))
            .Returns(wrappedResult);

        var (mockResponse, httpContext) = CreateMockHttpContext();
        await InvokeFilter(new Object(), httpContext);

        mockResponse.Verify(r => r.OnStarting(It.IsAny<Func<Task>>()), Times.Never);
    }

    private static (Mock<HttpResponse> mockResponse, HttpContext httpContext) CreateMockHttpContext()
    {
        var mockResponse = new Mock<HttpResponse>();
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
        return (mockResponse, mockHttpContext.Object);
    }

    private async Task<object?> InvokeFilter(object endpointResult, HttpContext? httpContext = null)
    {
        var filter = new ResponseMappingEndpointFilter();
        filter.filterMappings = _mockFilterMappings.Object;
        httpContext ??= new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext, Array.Empty<object>(), new Endpoint(null, null, null));
        var next = new EndpointFilterDelegate(_ => new ValueTask<object?>(endpointResult));

        return await filter.InvokeAsync(context, next);
    }
}
