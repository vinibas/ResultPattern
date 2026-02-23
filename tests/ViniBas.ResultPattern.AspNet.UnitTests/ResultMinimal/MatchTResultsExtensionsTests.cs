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

public class MatchTResultsExtensionsTests : IDisposable
{
    private readonly Mock<ITypedResultMatcher> _matcherMock = new();
    private readonly Ok<string> _successResult = TypedResults.Ok("Success");
    private readonly BadRequest<string> _errorResult = TypedResults.BadRequest("Error");
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Conflict("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create(
        [ new ErrorDetails("Code", "An error occurred.") ],
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
            .Setup(m => m.Match<ResultsOkAndBadRequest>(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<bool?>()))
            .Returns((ResultsOkAndBadRequest)_successResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultSuccess, r => _successResult, r => _errorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultSuccess, r => _successResult, useProblemDetails);

        Assert.Equal(_successResult, result.Result);
        Assert.Equal(_successResult, resultProb.Result);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            It.IsNotNull<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResultResponse_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest>(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<bool?>()))
            .Returns((ResultsOkAndBadRequest)_successResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultResponseSuccess, r => _successResult, r => _errorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultResponseSuccess, r => _successResult, useProblemDetails);

        Assert.Equal(_successResult, result.Result);
        Assert.Equal(_successResult, resultProb.Result);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            It.IsNotNull<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResult_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest>(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<bool?>()))
            .Returns((ResultsOkAndBadRequest)_errorResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultError, r => _successResult, r => _errorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultError, r => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, result.Result);
        Assert.Equal(_errorResult, resultProb.Result);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultError,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultError,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            It.IsNotNull<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void MatchResultResponse_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match<ResultsOkAndBadRequest>(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<bool?>()))
            .Returns((ResultsOkAndBadRequest)_errorResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultResponseError, r => _successResult, r => _errorResult);
        var resultProb = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultResponseError, r => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, result.Result);
        Assert.Equal(_errorResult, resultProb.Result);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<ResultsOkAndBadRequest>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            It.IsNotNull<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null), Times.Once);
    }

    public void Dispose()
        => ResultMatcherFactory.Reset();
}
