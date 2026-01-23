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

public class MatchResultsExtensionsTests : IDisposable
{
    private readonly Mock<ISimpleResultMatcher<IResult>> _matcherMock = new();
    private readonly FakeHelpers _fakes = new();

    public MatchResultsExtensionsTests()
        => ResultMatcherFactory.MinimalApiMatcherFactory = new(() => _matcherMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void Match_Result_Success_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.IResultSuccess);

        var result = MatchResultsExtensions.Match(_fakes.ResultSuccess, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultT = MatchResultsExtensions.Match(_fakes.ResultStrSuccess, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultProb = MatchResultsExtensions.Match(_fakes.ResultSuccess, r => _fakes.IResultSuccess, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match(_fakes.ResultStrSuccess, r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultSuccess, result);
        Assert.Equal(_fakes.IResultSuccess, resultT);
        Assert.Equal(_fakes.IResultSuccess, resultProb);
        Assert.Equal(_fakes.IResultSuccess, resultTProb);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsync_Result_Success_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.IResultSuccess);

        var result = await MatchResultsExtensions.MatchAsync(_fakes.ResultSuccess, async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultT = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrSuccess, async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultSuccess, async r => _fakes.IResultSuccess, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrSuccess, async r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultSuccess, result);
        Assert.Equal(_fakes.IResultSuccess, resultT);
        Assert.Equal(_fakes.IResultSuccess, resultProb);
        Assert.Equal(_fakes.IResultSuccess, resultTProb);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void Match_ResultResponse_Success_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.IResultSuccess);

        var resultResp = MatchResultsExtensions.Match(_fakes.ResultResponseSuccess, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultRespT = MatchResultsExtensions.Match<string>(_fakes.ResultResponseSuccessStr, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultRespProb = MatchResultsExtensions.Match(_fakes.ResultResponseSuccess, r => _fakes.IResultSuccess, useProblemDetails);
        var resultRespTProb = MatchResultsExtensions.Match<string>(_fakes.ResultResponseSuccessStr, r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultSuccess, resultResp);
        Assert.Equal(_fakes.IResultSuccess, resultRespT);
        Assert.Equal(_fakes.IResultSuccess, resultRespProb);
        Assert.Equal(_fakes.IResultSuccess, resultRespTProb);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsync_ResultResponse_Success_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.IResultSuccess);

        var resultResp = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseSuccess, async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultRespT = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseSuccessStr, async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultRespProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseSuccess, async r => _fakes.IResultSuccess, useProblemDetails);
        var resultRespTProb = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseSuccessStr, async r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultSuccess, resultResp);
        Assert.Equal(_fakes.IResultSuccess, resultRespT);
        Assert.Equal(_fakes.IResultSuccess, resultRespProb);
        Assert.Equal(_fakes.IResultSuccess, resultRespTProb);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void Match_Result_Failure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.IResultError);

        var result = MatchResultsExtensions.Match(_fakes.ResultError, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultT = MatchResultsExtensions.Match(_fakes.ResultStrError, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultProb = MatchResultsExtensions.Match(_fakes.ResultError, r => _fakes.IResultSuccess, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match(_fakes.ResultStrError, r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultError, result);
        Assert.Equal(_fakes.IResultError, resultT);
        Assert.Equal(_fakes.IResultError, resultProb);
        Assert.Equal(_fakes.IResultError, resultTProb);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsync_Result_Failure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.IResultError);

        var result = await MatchResultsExtensions.MatchAsync(_fakes.ResultError, async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultT = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrError, async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultError, async r => _fakes.IResultSuccess, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrError, async r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultError, result);
        Assert.Equal(_fakes.IResultError, resultT);
        Assert.Equal(_fakes.IResultError, resultProb);
        Assert.Equal(_fakes.IResultError, resultTProb);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void Match_ResultResponse_Failure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<Func<ResultResponse, IResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.IResultError);

        var result = MatchResultsExtensions.Match(_fakes.ResultResponseError, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultT = MatchResultsExtensions.Match<string>(_fakes.ResultResponseError, r => _fakes.IResultSuccess, r => _fakes.IResultError);
        var resultProb = MatchResultsExtensions.Match(_fakes.ResultResponseError, r => _fakes.IResultSuccess, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match<string>(_fakes.ResultResponseError, r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultError, result);
        Assert.Equal(_fakes.IResultError, resultT);
        Assert.Equal(_fakes.IResultError, resultProb);
        Assert.Equal(_fakes.IResultError, resultTProb);

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            It.IsNotNull<Func<ResultResponse, IResult>>(),
            null), Times.Exactly(2));

        _matcherMock.Verify(m => m.Match<IResult, IResult>(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, IResult>>(),
            null,
            useProblemDetails), Times.Exactly(2));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsync_ResultResponse_Failure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.IResultError);

        var result = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseError,
            async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultT = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseError,
            async r => _fakes.IResultSuccess, async r => _fakes.IResultError);
        var resultProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseError,
            async r => _fakes.IResultSuccess, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseError,
            async r => _fakes.IResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.IResultError, result);
        Assert.Equal(_fakes.IResultError, resultT);
        Assert.Equal(_fakes.IResultError, resultProb);
        Assert.Equal(_fakes.IResultError, resultTProb);

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IResult>>>(),
            null), Times.Exactly(2));

        _matcherMock.Verify(m => m.MatchAsync<IResult, IResult>(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, Task<IResult>>>(),
            null,
            useProblemDetails), Times.Exactly(2));
    }

    public void Dispose()
        => ResultMatcherFactory.Reset();
}
