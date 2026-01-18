/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;
using Xunit;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMatcher.Implementations;

public class TypedResultFallbackMatcherTests
{
    private readonly TypedResultFallbackMatcher _matcher;
    private readonly Mock<Func<ResultResponse, IResult>> _onSuccessFallback;
    private readonly Mock<Func<ResultResponse, bool?, IResult>> _onFailureFallback;
    private readonly Mock<TypedResultFallbackMatcher.TypeCaster> _typeCasterMock;

    private readonly IResult _successResult = TypedResults.Ok("Success");
    private readonly IResult _failureResult = TypedResults.BadRequest("Error");
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create([new ErrorDetails("Code", "Error")], ErrorTypes.Validation);

    public TypedResultFallbackMatcherTests()
    {
        _matcher = new TypedResultFallbackMatcher();
        _onSuccessFallback = new Mock<Func<ResultResponse, IResult>>();
        _onFailureFallback = new Mock<Func<ResultResponse, bool?, IResult>>();
        _typeCasterMock = new Mock<TypedResultFallbackMatcher.TypeCaster>();

        _matcher.OnSuccessFallback = _onSuccessFallback.Object;
        _matcher.OnFailureFallback = _onFailureFallback.Object;
        _matcher.TypeCasterInstance = _typeCasterMock.Object;

        _typeCasterMock.Setup(t => t.Cast<IResult>(It.IsAny<IResult>()))
            .Returns((IResult r) => r);
    }

    [Fact]
    public void Match_PassingOnSuccess_WhenSuccess_ReturnsExpectedResult()
    {
        // Arrange
        var expectedResult = TypedResults.Ok(_resultResponseSuccess);
        
        // Act
        var result = _matcher.Match<IResult, Ok<ResultResponseSuccess>, IResult>(
            _resultResponseSuccess,
            onSuccess: r => expectedResult,
            onFailure: null,
            useProblemDetails: null);

        // Assert
        Assert.Equal(expectedResult, result);
        _onSuccessFallback.Verify(f => f(It.IsAny<ResultResponse>()), Times.Never);
        _typeCasterMock.Verify(t => t.Cast<IResult>(expectedResult), Times.Once);
    }

    [Fact]
    public void Match_DontPassingOnSuccess_WhenSuccess_ReturnsFallbackResult()
    {
        // Arrange
        _onSuccessFallback.Setup(f => f(_resultResponseSuccess)).Returns(_successResult);

        // Act
        var result = _matcher.Match<IResult, IResult, IResult>(
            _resultResponseSuccess,
            onSuccess: null,
            onFailure: null,
            useProblemDetails: null);

        // Assert
        Assert.Equal(_successResult, result);
        _onSuccessFallback.Verify(f => f(_resultResponseSuccess), Times.Once);
        _typeCasterMock.Verify(t => t.Cast<IResult>(_successResult), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_PassingOnFailure_WhenError_ReturnsExpectedResult(bool? useProblemDetails)
    {
        // Arrange
        var expectedResult = TypedResults.BadRequest(_resultResponseError);

        // Act
        var result = _matcher.Match<IResult, IResult, BadRequest<ResultResponseError>>(
            _resultResponseError,
            onSuccess: null,
            onFailure: r => expectedResult,
            useProblemDetails: useProblemDetails);

        // Assert
        Assert.Equal(expectedResult, result);
        _onFailureFallback.Verify(f => f(It.IsAny<ResultResponse>(), It.IsAny<bool?>()), Times.Never);
        _typeCasterMock.Verify(t => t.Cast<IResult>(expectedResult), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_DontPassingOnFailure_WhenError_ReturnsFallbackFailure(bool? useProblemDetails)
    {
        // Arrange
        _onFailureFallback.Setup(f => f(_resultResponseError, useProblemDetails)).Returns(_failureResult);

        // Act
        var result = _matcher.Match<IResult, IResult, IResult>(
            _resultResponseError,
            onSuccess: null,
            onFailure: null,
            useProblemDetails: useProblemDetails);

        // Assert
        Assert.Equal(_failureResult, result);
        _onFailureFallback.Verify(f => f(_resultResponseError, useProblemDetails), Times.Once);
        _typeCasterMock.Verify(t => t.Cast<IResult>(_failureResult), Times.Once);
    }
}