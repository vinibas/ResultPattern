/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class MatchResultsExtensionsTests
{
    private readonly Mock<ISimpleResultMatcher<IActionResult>> _matcherMock = new();
    private readonly IActionResult _defaultSuccessResult = new ObjectResult("Success");
    private readonly IActionResult _defaultErrorResult = new ObjectResult("Error");
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Conflict("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create(
        ["An error occurred."], 
        ErrorTypes.Conflict);
    
    public MatchResultsExtensionsTests()
        => ResultMatcherFactory.ActionResultMatcherFactory = new(() => _matcherMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResult_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultSuccessResult);
        
        var result = MatchResultsExtensions.Match(_resultSuccess, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultProb = MatchResultsExtensions.Match(_resultSuccess, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultSuccessResult, result);
        Assert.Equal(_defaultSuccessResult, resultProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResultResponse_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultSuccessResult);
        
        var resultResp = MatchResultsExtensions.Match(_resultResponseSuccess, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultRespProb = MatchResultsExtensions.Match(_resultResponseSuccess, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultSuccessResult, resultResp);
        Assert.Equal(_defaultSuccessResult, resultRespProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResult_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultErrorResult);

        var result = MatchResultsExtensions.Match(_resultError, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultProb = MatchResultsExtensions.Match(_resultError, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultErrorResult, result);
        Assert.Equal(_defaultErrorResult, resultProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResultResponse_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultErrorResult);

        var resultResp = MatchResultsExtensions.Match(_resultResponseError, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultRespProb = MatchResultsExtensions.Match(_resultResponseError, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultErrorResult, resultResp);
        Assert.Equal(_defaultErrorResult, resultRespProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }
}
