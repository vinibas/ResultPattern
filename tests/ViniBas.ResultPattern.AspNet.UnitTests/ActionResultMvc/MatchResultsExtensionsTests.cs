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

public class MatchResultsExtensionsTests : IDisposable
{
    private readonly Mock<ISimpleResultMatcher<IActionResult>> _matcherMock = new();

    private readonly FakeHelpers _fakes = new();

    public MatchResultsExtensionsTests()
        => ResultMatcherFactory.ActionResultMatcherFactory = new(() => _matcherMock.Object);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void Match_Result_Success_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.Match(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.ActionResultSuccess);

        var result = MatchResultsExtensions.Match(_fakes.ResultSuccess, r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultT = MatchResultsExtensions.Match(_fakes.ResultStrSuccess, r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultProb = MatchResultsExtensions.Match(_fakes.ResultSuccess, r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match(_fakes.ResultStrSuccess, r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultSuccess, result);
        Assert.Equal(_fakes.ActionResultSuccess, resultT);
        Assert.Equal(_fakes.ActionResultSuccess, resultProb);
        Assert.Equal(_fakes.ActionResultSuccess, resultTProb);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
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
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.ActionResultSuccess);

        var result = await MatchResultsExtensions.MatchAsync(_fakes.ResultSuccess, async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultT = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrSuccess, async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultSuccess, async r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrSuccess, async r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultSuccess, result);
        Assert.Equal(_fakes.ActionResultSuccess, resultT);
        Assert.Equal(_fakes.ActionResultSuccess, resultProb);
        Assert.Equal(_fakes.ActionResultSuccess, resultTProb);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultStrSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
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
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.ActionResultSuccess);

        var resultResp = MatchResultsExtensions.Match(_fakes.ResultResponseSuccess,
            r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultRespT = MatchResultsExtensions.Match<string>(_fakes.ResultResponseSuccessStr,
            r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultRespProb = MatchResultsExtensions.Match(_fakes.ResultResponseSuccess,
            r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultRespTProb = MatchResultsExtensions.Match<string>(_fakes.ResultResponseSuccessStr,
            r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultSuccess, resultResp);
        Assert.Equal(_fakes.ActionResultSuccess, resultRespT);
        Assert.Equal(_fakes.ActionResultSuccess, resultRespProb);
        Assert.Equal(_fakes.ActionResultSuccess, resultRespTProb);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
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
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.ActionResultSuccess);

        var resultResp = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseSuccess, async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultRespT = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseSuccessStr, async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultRespProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseSuccess, async r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultRespTProb = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseSuccessStr, async r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultSuccess, resultResp);
        Assert.Equal(_fakes.ActionResultSuccess, resultRespT);
        Assert.Equal(_fakes.ActionResultSuccess, resultRespProb);
        Assert.Equal(_fakes.ActionResultSuccess, resultRespTProb);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultResponseSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultResponseSuccessStr,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
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
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.ActionResultError);

        var result = MatchResultsExtensions.Match(_fakes.ResultError, r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultT = MatchResultsExtensions.Match(_fakes.ResultStrError, r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultProb = MatchResultsExtensions.Match(_fakes.ResultError, r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match(_fakes.ResultStrError, r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultError, result);
        Assert.Equal(_fakes.ActionResultError, resultT);
        Assert.Equal(_fakes.ActionResultError, resultProb);
        Assert.Equal(_fakes.ActionResultError, resultTProb);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
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
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.ActionResultError);

        var result = await MatchResultsExtensions.MatchAsync(_fakes.ResultError, async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultT = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrError, async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultError, async r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultStrError, async r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultError, result);
        Assert.Equal(_fakes.ActionResultError, resultT);
        Assert.Equal(_fakes.ActionResultError, resultProb);
        Assert.Equal(_fakes.ActionResultError, resultTProb);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultStrError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
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
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<bool?>()))
            .Returns(_fakes.ActionResultError);

        var resultResp = MatchResultsExtensions.Match(_fakes.ResultResponseError, r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultTResp = MatchResultsExtensions.Match<string>(_fakes.ResultResponseError, r => _fakes.ActionResultSuccess, r => _fakes.ActionResultError);
        var resultRespProb = MatchResultsExtensions.Match(_fakes.ResultResponseError, r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultTRespProb = MatchResultsExtensions.Match<string>(_fakes.ResultResponseError, r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultError, resultResp);
        Assert.Equal(_fakes.ActionResultError, resultTResp);
        Assert.Equal(_fakes.ActionResultError, resultRespProb);
        Assert.Equal(_fakes.ActionResultError, resultTRespProb);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Exactly(2));

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
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
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_fakes.ActionResultError);

        var resultResp = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseError,
            async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultTResp = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseError,
            async r => _fakes.ActionResultSuccess, async r => _fakes.ActionResultError);
        var resultRespProb = await MatchResultsExtensions.MatchAsync(_fakes.ResultResponseError,
            async r => _fakes.ActionResultSuccess, useProblemDetails);
        var resultTRespProb = await MatchResultsExtensions.MatchAsync<string>(_fakes.ResultResponseError,
            async r => _fakes.ActionResultSuccess, useProblemDetails);

        Assert.Equal(_fakes.ActionResultError, resultResp);
        Assert.Equal(_fakes.ActionResultError, resultTResp);
        Assert.Equal(_fakes.ActionResultError, resultRespProb);
        Assert.Equal(_fakes.ActionResultError, resultTRespProb);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Exactly(2));

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultResponseError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Exactly(2));
    }

    public void Dispose()
        => ResultMatcherFactory.Reset();
}
