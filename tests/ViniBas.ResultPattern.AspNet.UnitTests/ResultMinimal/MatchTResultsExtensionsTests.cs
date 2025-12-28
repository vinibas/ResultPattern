/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ResultMinimalApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

using ResultsOkAndBadRequest = Results<Ok<string>, BadRequest<string>>;

public class MatchTResultsExtensionsTests
{
    private readonly Mock<ITypedResultMatcher> _matcherMock = new();
    private readonly Ok<string> _defaultSuccessResult = TypedResults.Ok("Success");
    private readonly BadRequest<string> _defaultErrorResult = TypedResults.BadRequest("Error");
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Conflict("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create(
        ["An error occurred."], 
        ErrorTypes.Conflict);

    public MatchTResultsExtensionsTests()
        => ResultMatcherFactory.TypedMatcherFactory = new(() => _matcherMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResult_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultSuccessResult);
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultSuccessResult);
        
        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultSuccess, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>>(
            _resultSuccess, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultSuccessResult, result.Result);
        Assert.Equal(_defaultSuccessResult, resultProb.Result);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            It.IsNotNull<Func<ResultResponse, BadRequest<string>>>(),
            null), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResultResponse_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultSuccessResult);
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultSuccessResult);
        
        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultResponseSuccess, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>>(
            _resultResponseSuccess, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultSuccessResult, result.Result);
        Assert.Equal(_defaultSuccessResult, resultProb.Result);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            It.IsNotNull<Func<ResultResponse, BadRequest<string>>>(),
            null), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResult_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultErrorResult);
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultErrorResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultError, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>>(
            _resultError, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultErrorResult, result.Result);
        Assert.Equal(_defaultErrorResult, resultProb.Result);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultError,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            It.IsNotNull<Func<ResultResponse, BadRequest<string>>>(),
            null), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResultResponse_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultErrorResult);
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Ok<string>>>(),
                It.IsAny<Func<ResultResponse, BadRequest<string>>>(),
                It.IsAny<bool?>()))
            .Returns(_defaultErrorResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultResponseError, r => _defaultSuccessResult, r => _defaultErrorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest, Ok<string>>(
            _resultResponseError, r => _defaultSuccessResult, useProblemDetails);

        Assert.Equal(_defaultErrorResult, result.Result);
        Assert.Equal(_defaultErrorResult, resultProb.Result);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, IResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest, Ok<string>, BadRequest<string>>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, Ok<string>>>(),
            It.IsNotNull<Func<ResultResponse, BadRequest<string>>>(),
            null), Times.Once);
    }
}
