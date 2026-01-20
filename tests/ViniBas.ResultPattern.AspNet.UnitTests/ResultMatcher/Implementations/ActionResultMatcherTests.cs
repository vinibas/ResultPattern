/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMatcher.Implementations;

public class ActionResultMatcherTests
{
    private readonly ActionResultMatcher _matcher;
    private readonly Mock<Func<ResultResponse, IActionResult>> _onSuccessFallback;
    private readonly Mock<Func<ResultResponse, bool?, IActionResult>> _onFailureFallback;
    private readonly IActionResult _successFallbackResult = new OkResult();
    private readonly IActionResult _failureFallbackResult = new BadRequestResult();
    
    public ActionResultMatcherTests()
    {
        _matcher = new ActionResultMatcher();
        _onSuccessFallback = new Mock<Func<ResultResponse, IActionResult>>();
        _onFailureFallback = new Mock<Func<ResultResponse, bool?, IActionResult>>();

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
        var okResult = new OkResult();
        var resultSuccess = Result.Success();
        var resultResponseSuccess = ResultResponseSuccess.Create();

        // Act
        var matcherResult = _matcher.Match<IActionResult, IActionResult>(
            resultSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);
        var matcherResultAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultSuccess,
            onSuccess: async r => okResult,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);
        var matcherResultResponseAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultResponseSuccess,
            onSuccess: async r => okResult,
            onFailure: null,
            null);

        // Assert
        Assert.Equal(matcherResult, okResult);
        Assert.Equal(matcherResultAsync, okResult);
        Assert.Equal(matcherResultResponse, okResult);
        Assert.Equal(matcherResultResponseAsync, okResult);
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
        var matcherResult = _matcher.Match<IActionResult, IActionResult>(
            resultSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultResponseAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);

        // Assert
        Assert.Equal(_successFallbackResult, matcherResult);
        Assert.Equal(_successFallbackResult, matcherResultAsync);
        Assert.Equal(_successFallbackResult, matcherResultResponse);
        Assert.Equal(_successFallbackResult, matcherResultResponseAsync);
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
        var badRequestResultValue = new BadRequestResult();
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.Details, error.Type);

        // Act
        var matcherResult = _matcher.Match<IActionResult, IActionResult>(
            resultError,
            onSuccess: null,
            onFailure: r => badRequestResultValue,
            useProblemDetails);
        var matcherResultAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultError,
            onSuccess: null,
            onFailure: async r => badRequestResultValue,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: r => badRequestResultValue,
            useProblemDetails);
        var matcherResultResponseAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: async r => badRequestResultValue,
            useProblemDetails);

        // Assert
        Assert.Equal(badRequestResultValue, matcherResult);
        Assert.Equal(badRequestResultValue, matcherResultAsync);
        Assert.Equal(badRequestResultValue, matcherResultResponse);
        Assert.Equal(badRequestResultValue, matcherResultResponseAsync);
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
        var matcherResult = _matcher.Match<IActionResult, IActionResult>(
            resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
            resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponseAsync = await _matcher.MatchAsync<IActionResult, IActionResult>(
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
