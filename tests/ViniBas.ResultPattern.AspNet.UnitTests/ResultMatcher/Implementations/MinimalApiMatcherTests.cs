/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMatcher.Implementations;

public class MinimalApiMatcherTests
{
    private readonly MinimalApiMatcher _matcher;
    private readonly Mock<Func<ResultResponse, IResult>> _onSuccessFallback;
    private readonly Mock<Func<ResultResponse, bool?, IResult>> _onFailureFallback;
    private readonly IResult _successFallbackResult = Results.Ok();
    private readonly IResult _failureFallbackResult = Results.BadRequest();

    public MinimalApiMatcherTests()
    {
        _matcher = new MinimalApiMatcher();
        _onSuccessFallback = new Mock<Func<ResultResponse, IResult>>();
        _onFailureFallback = new Mock<Func<ResultResponse, bool?, IResult>>();

        _onSuccessFallback.Setup(f => f(It.IsAny<ResultResponse>()))
            .Returns(_successFallbackResult);
        _onFailureFallback.Setup(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()))
            .Returns((ResultResponse r, bool? u) => _failureFallbackResult);

        _matcher.OnSuccessFallback = _onSuccessFallback.Object;
        _matcher.OnFailureFallback = _onFailureFallback.Object;
    }
    
    [Fact]
    public void Match_PassingOnSuccess_WhenSuccess_ReturnsExpectedResult()
    {
        // Arrange
        var okResult = TypedResults.Ok();
        var resultSuccess = Result.Success();
        var resultResponseSuccess = ResultResponseSuccess.Create();

        // Act
        var matcherResult = _matcher.Match<IResult, IResult>(
            resultSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match<IResult, IResult>(
            resultResponseSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);

        // Assert
        Assert.Equal(okResult, matcherResult);
        Assert.Equal(okResult, matcherResultResponse);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void Match_DontPassingOnSuccess_WhenSuccess_ReturnsFallbackResult()
    {
        // Arrange
        var resultSuccess = Result.Success();
        var resultResponseSuccess = ResultResponseSuccess.Create();

        // Act
        var matcherResult = _matcher.Match<IResult, IResult>(
            resultSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match<IResult, IResult>(
            resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);

        // Assert
        Assert.IsType<Ok>(matcherResult);
        Assert.IsType<Ok>(matcherResultResponse);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Exactly(2));
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_PassingOnFailure_WhenError_ReturnsExpectedResult(bool? useProblemDetails)
    {
        // Arrange
        var badRequestResult = TypedResults.BadRequest();
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.ListDescriptions(), error.Type);

        // Act
        var matcherResult = _matcher.Match<IResult, IResult>(
            resultError,
            onSuccess: null,
            onFailure: r => badRequestResult,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<IResult, IResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: r => badRequestResult,
            useProblemDetails);

        // Assert
        Assert.Equal(badRequestResult, matcherResult);
        Assert.Equal(badRequestResult, matcherResultResponse);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_DontPassingOnFailure_WhenError_ReturnsFallbackFailure(bool? useProblemDetails)
    {
        // Arrange
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.ListDescriptions(), error.Type);

        // Act
        var matcherResult = _matcher.Match<IResult, IResult>(
            resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<IResult, IResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);

        // Assert
        Assert.Equal(_failureFallbackResult, matcherResult);
        Assert.Equal(_failureFallbackResult, matcherResultResponse);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), useProblemDetails), Times.Exactly(2));
    }
}