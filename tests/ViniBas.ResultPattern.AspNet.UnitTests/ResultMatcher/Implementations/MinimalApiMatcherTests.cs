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
    public async Task Match_PassingOnSuccess_WhenSuccess_ReturnsExpectedResult()
    {
        // Arrange
        var okResult = TypedResults.Ok();
        var resultSuccess = Result.Success();
        var resultResponseSuccess = ResultResponseSuccess.Create();

        // Act
        var matcherResult = _matcher.Match(
            resultSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);
        var matcherResultAsync = await _matcher.MatchAsync(
            resultSuccess,
            onSuccess: async r => okResult,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match(
            resultResponseSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);
        var matcherResultResponseAsync = await _matcher.MatchAsync(
            resultResponseSuccess,
            onSuccess: async r => okResult,
            onFailure: null,
            null);

        // Assert
        Assert.Equal(okResult, matcherResult);
        Assert.Equal(okResult, matcherResultAsync);
        Assert.Equal(okResult, matcherResultResponse);
        Assert.Equal(okResult, matcherResultResponseAsync);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public async Task Match_DontPassingOnSuccess_WhenSuccess_ReturnsFallbackResult()
    {
        // Arrange
        var resultSuccess = Result.Success();
        var resultResponseSuccess = ResultResponseSuccess.Create();

        // Act
        var matcherResult = _matcher.Match(
            resultSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultAsync = await _matcher.MatchAsync(
            resultSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match(
            resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultResponseAsync = await _matcher.MatchAsync(
            resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);

        // Assert
        Assert.IsType<Ok>(matcherResult);
        Assert.IsType<Ok>(matcherResultAsync);
        Assert.IsType<Ok>(matcherResultResponse);
        Assert.IsType<Ok>(matcherResultResponseAsync);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Exactly(4));
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Match_PassingOnFailure_WhenError_ReturnsExpectedResult(bool? useProblemDetails)
    {
        // Arrange
        var badRequestResult = TypedResults.BadRequest();
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.Details, error.Type);

        // Act
        var matcherResult = _matcher.Match(
            resultError,
            onSuccess: null,
            onFailure: r => badRequestResult,
            useProblemDetails);
        var matcherResultAsync = await _matcher.MatchAsync(
            resultError,
            onSuccess: null,
            onFailure: async r => badRequestResult,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match(
            resultResponseError,
            onSuccess: null,
            onFailure: r => badRequestResult,
            useProblemDetails);
        var matcherResultResponseAsync = await _matcher.MatchAsync(
            resultResponseError,
            onSuccess: null,
            onFailure: async r => badRequestResult,
            useProblemDetails);

        // Assert
        Assert.Equal(badRequestResult, matcherResult);
        Assert.Equal(badRequestResult, matcherResultAsync);
        Assert.Equal(badRequestResult, matcherResultResponse);
        Assert.Equal(badRequestResult, matcherResultResponseAsync);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Match_DontPassingOnFailure_WhenError_ReturnsFallbackFailure(bool? useProblemDetails)
    {
        // Arrange
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.Details, error.Type);

        // Act
        var matcherResult = _matcher.Match(
            resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultAsync = await _matcher.MatchAsync(
            resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match(
            resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponseAsync = await _matcher.MatchAsync(
            resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);

        // Assert
        Assert.Equal(_failureFallbackResult, matcherResult);
        Assert.Equal(_failureFallbackResult, matcherResultAsync);
        Assert.Equal(_failureFallbackResult, matcherResultResponse);
        Assert.Equal(_failureFallbackResult, matcherResultResponseAsync);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), useProblemDetails), Times.Exactly(4));
    }
}