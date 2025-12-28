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
    private readonly Mock<Func<ResultResponse, IActionResult>> _onSuccessDefault;
    private readonly Mock<Func<ResultResponse, bool?, IActionResult>> _onFailureDefault;
    private readonly IActionResult _successDefaultResult = new OkResult();
    private readonly IActionResult _failureDefaultResult = new BadRequestResult();
    
    public ActionResultMatcherTests()
    {
        _matcher = new ActionResultMatcher();
        _onSuccessDefault = new Mock<Func<ResultResponse, IActionResult>>();
        _onFailureDefault = new Mock<Func<ResultResponse, bool?, IActionResult>>();

        _onSuccessDefault.Setup(f => f(It.IsAny<ResultResponse>()))
            .Returns(_successDefaultResult);
        _onFailureDefault.Setup(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()))
            .Returns((ResultResponse r, bool? u) => _failureDefaultResult);

        _matcher.OnSuccessDefault = _onSuccessDefault.Object;
        _matcher.OnFailureDefault = _onFailureDefault.Object;
    }
    
    [Fact]
    public void Match_PassingOnSuccess_WhenSuccess_ReturnsExpectedResult()
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
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseSuccess,
            onSuccess: r => okResult,
            onFailure: null,
            null);

        // Assert
        Assert.Equal(matcherResult, okResult);
        Assert.Equal(matcherResultResponse, okResult);
        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void Match_DontPassingOnSuccess_WhenSuccess_ReturnsDefaultResult()
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
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);

        // Assert
        Assert.Equal(_successDefaultResult, matcherResult);
        Assert.Equal(_successDefaultResult, matcherResultResponse);
        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Exactly(2));
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_PassingOnFailure_WhenError_ReturnsExpectedResult(bool? useProblemDetails)
    {
        // Arrange
        var badRequestResultValue = new BadRequestResult();
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.ListDescriptions(), error.Type);

        // Act
        var matcherResult = _matcher.Match<IActionResult, IActionResult>(
            resultError,
            onSuccess: null,
            onFailure: r => badRequestResultValue,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: r => badRequestResultValue,
            useProblemDetails);

        // Assert
        Assert.Equal(badRequestResultValue, matcherResult);
        Assert.Equal(badRequestResultValue, matcherResultResponse);
        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_DontPassingOnFailure_WhenError_ReturnsDefaultFailure(bool? useProblemDetails)
    {
        // Arrange
        var error = Error.Validation("Code", "An error occurred.");
        var resultError = Result.Failure(error);
        var resultResponseError = ResultResponseError.Create(error.ListDescriptions(), error.Type);

        // Act
        var matcherResult = _matcher.Match<IActionResult, IActionResult>(
            resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<IActionResult, IActionResult>(
            resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);

        // Assert
        Assert.Equal(_failureDefaultResult, matcherResult);
        Assert.Equal(_failureDefaultResult, matcherResultResponse);
        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), useProblemDetails), Times.Exactly(2));
    }
}
