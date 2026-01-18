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

public class TypedResultMatcherTests
{
    private readonly TypedResultMatcher _matcher;
    private readonly Mock<ITypedResultFallbackMatcher> _matchFallbackInstance;
    private readonly Mock<Func<ResultResponse, Ok<ResultResponseSuccess>>> _onSuccessFallback;
    private readonly Mock<Func<ResultResponse, bool?, BadRequest<ResultResponseError>>> _onFailureFallback;
    private readonly Ok<ResultResponseSuccess> _successFallbackResult;
    private readonly BadRequest<ResultResponseError> _failureFallbackResult;
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Validation("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create(
            [new ErrorDetails("Code", "Error occurred.")],
            ErrorTypes.Validation);


    public TypedResultMatcherTests()
    {
        _matcher = new TypedResultMatcher();

        _matchFallbackInstance = new Mock<ITypedResultFallbackMatcher>();
        _matcher.MatchFallbackInstance = _matchFallbackInstance.Object;
        
        _onSuccessFallback = new Mock<Func<ResultResponse, Ok<ResultResponseSuccess>>>();
        _onFailureFallback = new Mock<Func<ResultResponse, bool?, BadRequest<ResultResponseError>>>();

        _successFallbackResult = TypedResults.Ok(_resultResponseSuccess);
        _failureFallbackResult = TypedResults.BadRequest(_resultResponseError);

        _onSuccessFallback.Setup(f => f(It.IsAny<ResultResponse>()))
            .Returns(_successFallbackResult);
        _onFailureFallback.Setup(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()))
            .Returns((ResultResponse r, bool? u) => _failureFallbackResult);

        _matcher.MatchFallbackInstance = new TypedResultFallbackMatcher
        {
            OnSuccessFallback = _onSuccessFallback.Object,
            OnFailureFallback = _onFailureFallback.Object
        };
    }
    
    [Fact]
    public void Match_PassingOnSuccess_WhenSuccess_ReturnsExpectedResult()
    {
        // Act
        var matcherResult = _matcher.Match<Results<Ok<ResultResponse>, IResult>, Ok<ResultResponse>, IResult>(
            _resultSuccess,
            onSuccess: r => TypedResults.Ok(r),
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match<Results<Ok<ResultResponse>, IResult>, Ok<ResultResponse>, IResult>(
            _resultResponseSuccess,
            onSuccess: r => TypedResults.Ok(r),
            onFailure: null,
            null);

        // Assert
        var okResult = Assert.IsType<Ok<ResultResponse>>(matcherResult.Result);
        Assert.Equal(_resultResponseSuccess, okResult.Value);
        okResult = Assert.IsType<Ok<ResultResponse>>(matcherResultResponse.Result);
        Assert.Equal(_resultResponseSuccess, okResult.Value);
        
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void Match_DontPassingOnSuccess_WhenSuccess_ReturnsFallbackResult()
    {
        // Arrange
        _matchFallbackInstance
            .Setup(m => m.Match<Results<Ok<ResultResponseSuccess>, IResult>, Ok<ResultResponseSuccess>, IResult>(
                It.IsAny<ResultResponseSuccess>(),
                It.IsAny<Func<ResultResponse, Ok<ResultResponseSuccess>>>(),
                null,
                null))
            .Returns(() => _successFallbackResult);

        // Act
        var matcherResult = _matcher.Match<Results<Ok<ResultResponse>, IResult>, Ok<ResultResponse>, IResult>(
            _resultSuccess,
            onSuccess: null,
            onFailure: null,
            null);
        var matcherResultResponse = _matcher.Match<Results<Ok<ResultResponse>, IResult>, Ok<ResultResponse>, IResult>(
            _resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            null);

        // Assert
        var okResult = Assert.IsType<Ok<ResultResponseSuccess>>(matcherResult.Result);
        Assert.Equal(_resultResponseSuccess, okResult.Value);
        okResult = Assert.IsType<Ok<ResultResponseSuccess>>(matcherResultResponse.Result);
        Assert.Equal(_resultResponseSuccess, okResult.Value);

        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Exactly(2));
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_PassingOnFailure_WhenError_ReturnsExpectedResult(bool? useProblemDetails)
    {
        // Act
        var matcherResult = _matcher.Match<Results<IResult, BadRequest>, IResult, BadRequest>(
            _resultError,
            onSuccess: null,
            onFailure: r => TypedResults.BadRequest(),
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<Results<IResult, BadRequest>, IResult, BadRequest>(
            _resultResponseError,
            onSuccess: null,
            onFailure: r => TypedResults.BadRequest(),
            useProblemDetails);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest>(matcherResult.Result);
        Assert.Equal(badRequestResult, matcherResult.Result);
        badRequestResult = Assert.IsType<BadRequest>(matcherResultResponse.Result);
        Assert.Equal(badRequestResult, matcherResultResponse.Result);

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
        _matchFallbackInstance
            .Setup(m => m.Match<Results<IResult, BadRequest<ResultResponseError>>, IResult, BadRequest<ResultResponseError>>(
                It.IsAny<ResultResponseError>(),
                It.IsAny<Func<ResultResponse, Ok<ResultResponseError>>>(),
                null,
                null))
            .Returns(() => _failureFallbackResult);

        // Act
        var matcherResult = _matcher.Match<Results<IResult, BadRequest>, IResult, BadRequest>(
            _resultError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);
        var matcherResultResponse = _matcher.Match<Results<IResult, BadRequest>, IResult, BadRequest>(
            _resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<ResultResponseError>>(matcherResult.Result);
        Assert.Equal(_failureFallbackResult, matcherResult.Result);
        badRequestResult = Assert.IsType<BadRequest<ResultResponseError>>(matcherResultResponse.Result);
        Assert.Equal(_failureFallbackResult, matcherResultResponse.Result);

        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), useProblemDetails), Times.Exactly(2));
    }
}