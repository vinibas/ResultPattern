/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using ViniBas.ResultPattern.AspNet.MinimalApi;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.UnitTests.MinimalApi;

public class TypedResultCastFallbackFilterTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldReturnNormally()
    {
        var expected = TypedResults.Ok("test");

        var result = await InvokeFilter(next: _ => new ValueTask<object?>(expected));

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task InvokeAsync_WhenTypedResultCastException_ShouldReturnOriginalIResult()
    {
        IResult originalResult = TypedResults.Ok("data");
        var exception = new TypedResultCastException(originalResult, typeof(BadRequest<string>), typeof(Ok<string>));

        var result = await InvokeFilter(next: _ => throw exception);

        Assert.IsType<Ok<string>>(result);
        Assert.Same(originalResult, result);
    }

    [Fact]
    public async Task InvokeAsync_WhenTypedResultCastException_ShouldLogError()
    {
        IResult originalResult = TypedResults.Ok("data");
        var exception = new TypedResultCastException(originalResult, typeof(BadRequest<string>), typeof(Ok<string>));
        var mockLogger = new Mock<ILogger<TypedResultCastFallbackFilter>>();

        var result = await InvokeFilter(
            next: _ => throw exception,
            logger: mockLogger.Object);

        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenOtherException_ShouldNotCatch()
    {
        var exception = new InvalidOperationException("other error");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => InvokeFilter(next: _ => throw exception).AsTask());
    }

    [Fact]
    public async Task InvokeAsync_WhenNoLoggerRegistered_ShouldStillReturnOriginalIResult()
    {
        IResult originalResult = TypedResults.NotFound();
        var exception = new TypedResultCastException(originalResult, typeof(Ok), typeof(NotFound));

        var result = await InvokeFilter(
            next: _ => throw exception,
            logger: null);

        Assert.IsType<NotFound>(result);
        Assert.Same(originalResult, result);
    }

    private static async ValueTask<object?> InvokeFilter(
        EndpointFilterDelegate next,
        ILogger<TypedResultCastFallbackFilter>? logger = null)
    {
        var filter = new TypedResultCastFallbackFilter();

        var services = new Mock<IServiceProvider>();
        services
            .Setup(s => s.GetService(typeof(ILogger<TypedResultCastFallbackFilter>)))
            .Returns(logger!);

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(r => r.Method).Returns("GET");
        mockRequest.Setup(r => r.Path).Returns(new PathString("/test"));

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.RequestServices).Returns(services.Object);
        mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

        var context = new DefaultEndpointFilterInvocationContext(
            mockHttpContext.Object, Array.Empty<object>(), new Endpoint(null, null, null));

        return await filter.InvokeAsync(context, next);
    }
}
