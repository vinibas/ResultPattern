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
    private readonly Mock<ITypedResultDefaultMatcher> _matchDefaultInstance;
    private readonly Mock<Func<ResultResponse, Ok<ResultResponseSuccess>>> _onSuccessDefault;
    private readonly Mock<Func<ResultResponse, bool?, BadRequest<ResultResponseError>>> _onFailureDefault;
    private readonly Ok<ResultResponseSuccess> _successDefaultResult;
    private readonly BadRequest<ResultResponseError> _failureDefaultResult;
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Validation("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = new ResultResponseError(
            ["Error occurred."],
            ErrorTypes.Validation);


    public TypedResultMatcherTests()
    {
        _matcher = new TypedResultMatcher();

        _matchDefaultInstance = new Mock<ITypedResultDefaultMatcher>();
        _matcher.MatchDefaultInstance = _matchDefaultInstance.Object;
        
        _onSuccessDefault = new Mock<Func<ResultResponse, Ok<ResultResponseSuccess>>>();
        _onFailureDefault = new Mock<Func<ResultResponse, bool?, BadRequest<ResultResponseError>>>();

        _successDefaultResult = TypedResults.Ok(_resultResponseSuccess);
        _failureDefaultResult = TypedResults.BadRequest(_resultResponseError);

        _onSuccessDefault.Setup(f => f(It.IsAny<ResultResponse>()))
            .Returns(_successDefaultResult);
        _onFailureDefault.Setup(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()))
            .Returns((ResultResponse r, bool? u) => _failureDefaultResult);

        _matcher.MatchDefaultInstance = new TypedResultDefaultMatcher
        {
            OnSuccessDefault = _onSuccessDefault.Object,
            OnFailureDefault = _onFailureDefault.Object
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
        
        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
    }

    [Fact]
    public void Match_DontPassingOnSuccess_WhenSuccess_ReturnsDefaultResult()
    {
        // Arrange
        _matchDefaultInstance
            .Setup(m => m.Match<Results<Ok<ResultResponseSuccess>, IResult>, Ok<ResultResponseSuccess>, IResult>(
                It.IsAny<ResultResponseSuccess>(),
                It.IsAny<Func<ResultResponse, Ok<ResultResponseSuccess>>>(),
                null,
                null))
            .Returns(() => _successDefaultResult);

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

        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Exactly(2));
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
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
        _matchDefaultInstance
            .Setup(m => m.Match<Results<IResult, BadRequest<ResultResponseError>>, IResult, BadRequest<ResultResponseError>>(
                It.IsAny<ResultResponseError>(),
                It.IsAny<Func<ResultResponse, Ok<ResultResponseError>>>(),
                null,
                null))
            .Returns(() => _failureDefaultResult);

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
        Assert.Equal(_failureDefaultResult, matcherResult.Result);
        badRequestResult = Assert.IsType<BadRequest<ResultResponseError>>(matcherResultResponse.Result);
        Assert.Equal(_failureDefaultResult, matcherResultResponse.Result);

        _onSuccessDefault.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _onFailureDefault.Verify(f => f(It.IsAny<ResultResponse>(), useProblemDetails), Times.Exactly(2));
    }
}