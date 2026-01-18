/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ResultMinimalApi;
using Microsoft.AspNetCore.Http;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

public class MatchResultsExtensionsTests
{
    private readonly Mock<ISimpleResultMatcher<IResult>> _matcherMock = new();
    private readonly IResult _successResult = Results.Ok("Success");
    private readonly IResult _errorResult = Results.BadRequest("Error");
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Conflict("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create(
        [ new ErrorDetails("Code", "An error occurred.") ], 
        ErrorTypes.Conflict);

    public MatchResultsExtensionsTests()
        => ResultMatcherFactory.MinimalApiMatcherFactory = new(() => _matcherMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResult_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_successResult);
        
        var result = MatchResultsExtensions.Match(_resultSuccess, r => _successResult, r => _errorResult);
        var resultProb = MatchResultsExtensions.Match(_resultSuccess, r => _successResult, useProblemDetails);

        Assert.Equal(_successResult, result);
        Assert.Equal(_successResult, resultProb);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
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
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_successResult);
        
        var result = MatchResultsExtensions.Match(_resultResponseSuccess, r => _successResult, r => _errorResult);
        var resultProb = MatchResultsExtensions.Match(_resultResponseSuccess, r => _successResult, useProblemDetails);

        Assert.Equal(_successResult, result);
        Assert.Equal(_successResult, resultProb);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
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
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_errorResult);

        var result = MatchResultsExtensions.Match(_resultError, r => _successResult, r => _errorResult);
        var resultProb = MatchResultsExtensions.Match(_resultError, r => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, result);
        Assert.Equal(_errorResult, resultProb);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, IResult>>(),
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
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_errorResult);

        var result = MatchResultsExtensions.Match(_resultResponseError, r => _successResult, r => _errorResult);
        var resultProb = MatchResultsExtensions.Match(_resultResponseError, r => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, result);
        Assert.Equal(_errorResult, resultProb);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);
    }
}
