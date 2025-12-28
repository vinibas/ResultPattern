/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using ViniBas.ResultPattern.AspNet.ResultMinimalApi;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

public class ResultsResultFilterTests
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

    private async Task<object?> InvokeFilter(object endpointResult)
    {
        var filter = new ResultsResultFilter();
        filter.filterMappings = _mockFilterMappings.Object;
        var httpContext = new DefaultHttpContext();
        var context = new DefaultEndpointFilterInvocationContext(httpContext, Array.Empty<object>(), new Endpoint(null, null, null));
        var next = new EndpointFilterDelegate(_ => new ValueTask<object?>(endpointResult));

        return await filter.InvokeAsync(context, next);
    }
}
