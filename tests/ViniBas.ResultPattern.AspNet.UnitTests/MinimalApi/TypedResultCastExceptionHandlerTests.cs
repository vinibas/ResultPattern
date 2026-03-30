/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViniBas.ResultPattern.AspNet.MinimalApi;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.UnitTests.MinimalApi;

public class TypedResultCastExceptionHandlerTests
{
    private readonly Mock<ILogger<TypedResultCastExceptionHandler>> _mockLogger = new();

    [Fact]
    public async Task TryHandleAsync_WhenTypedResultCastException_ShouldExecuteOriginalResult()
    {
        IResult originalResult = TypedResults.Ok("data");
        var exception = new TypedResultCastException(originalResult, typeof(BadRequest<string>), typeof(Ok<string>));
        var httpContext = CreateHttpContext();

        var handler = new TypedResultCastExceptionHandler(_mockLogger.Object);
        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(200, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WhenTypedResultCastException_ShouldLogError()
    {
        IResult originalResult = TypedResults.Ok("data");
        var exception = new TypedResultCastException(originalResult, typeof(BadRequest<string>), typeof(Ok<string>));
        var httpContext = CreateHttpContext();

        var handler = new TypedResultCastExceptionHandler(_mockLogger.Object);
        await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_WhenOtherException_ShouldReturnFalse()
    {
        var exception = new InvalidOperationException("other error");
        var httpContext = new DefaultHttpContext();

        var handler = new TypedResultCastExceptionHandler(_mockLogger.Object);
        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.False(handled);
    }

    [Fact]
    public async Task TryHandleAsync_WhenNotFoundResult_ShouldPreserveStatusCode()
    {
        IResult originalResult = TypedResults.NotFound();
        var exception = new TypedResultCastException(originalResult, typeof(Ok), typeof(NotFound));
        var httpContext = CreateHttpContext();

        var handler = new TypedResultCastExceptionHandler(_mockLogger.Object);
        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(404, httpContext.Response.StatusCode);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.RequestServices = services;
        return httpContext;
    }
}
