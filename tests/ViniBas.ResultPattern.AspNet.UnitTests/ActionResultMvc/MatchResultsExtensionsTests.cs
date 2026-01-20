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
    private readonly IActionResult _successResult = new ObjectResult("Success");
    private readonly IActionResult _errorResult = new ObjectResult("Error");
    private readonly Result _resultSuccess = Result.Success();
    private readonly Result _resultError = Result.Failure(Error.Conflict("Code", "An error occurred."));
    private readonly Result<string> _resultTSuccess = Result.Success("Data");
    private readonly Result<string> _resultTError = Result<string>.Failure(Error.Conflict("Code", "An error occurred."));
    private readonly ResultResponseSuccess _resultResponseSuccess = ResultResponseSuccess.Create();
    private readonly ResultResponseError _resultResponseError = ResultResponseError.Create(
        [ new ErrorDetails("Code", "An error occurred.") ], 
        ErrorTypes.Conflict);
    private readonly ResultResponseSuccess<string> _resultResponseSuccessT = ResultResponseSuccess.Create("Data");
    
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
            .Returns(_successResult);
        
        var result = MatchResultsExtensions.Match(_resultSuccess, (ResultResponseSuccess r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultT = MatchResultsExtensions.Match(_resultTSuccess, (ResultResponseSuccess<string> r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultProb = MatchResultsExtensions.Match(_resultSuccess, (ResultResponseSuccess r) => _successResult, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match(_resultTSuccess, (ResultResponseSuccess<string> r) => _successResult, useProblemDetails);

        Assert.Equal(_successResult, result);
        Assert.Equal(_successResult, resultT);
        Assert.Equal(_successResult, resultProb);
        Assert.Equal(_successResult, resultTProb);

        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultTSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultTSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsyncResult_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultBase>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_successResult);
        
        var result = await MatchResultsExtensions.MatchAsync(_resultSuccess, async r => _successResult, async r => _errorResult);
        var resultT = await MatchResultsExtensions.MatchAsync(_resultTSuccess, async r => _successResult, async r => _errorResult);
        var resultProb = await MatchResultsExtensions.MatchAsync(_resultSuccess, async r => _successResult, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync(_resultTSuccess, async r => _successResult, useProblemDetails);

        Assert.Equal(_successResult, result);
        Assert.Equal(_successResult, resultT);
        Assert.Equal(_successResult, resultProb);
        Assert.Equal(_successResult, resultTProb);

        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);

        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultTSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultTSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
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
            .Returns(_successResult);
        
        var resultResp = MatchResultsExtensions.Match(_resultResponseSuccess, (ResultResponseSuccess r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultRespT = MatchResultsExtensions.Match(_resultResponseSuccessT, (ResultResponseSuccess<string> r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultRespProb = MatchResultsExtensions.Match(_resultResponseSuccess, (ResultResponseSuccess r) => _successResult, useProblemDetails);
        var resultRespTProb = MatchResultsExtensions.Match(_resultResponseSuccessT, (ResultResponseSuccess<string> r) => _successResult, useProblemDetails);

        Assert.Equal(_successResult, resultResp);
        Assert.Equal(_successResult, resultRespT);
        Assert.Equal(_successResult, resultRespProb);
        Assert.Equal(_successResult, resultRespTProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseSuccessT,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseSuccessT,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsyncResultResponse_ResultSuccess_ShouldReturnOnSuccess(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultResponse>(r => r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_successResult);
        
        var resultResp = await MatchResultsExtensions.MatchAsync(_resultResponseSuccess, async r => _successResult, async r => _errorResult);
        var resultRespT = await MatchResultsExtensions.MatchAsync(_resultResponseSuccessT, async r => _successResult, async r => _errorResult);
        var resultRespProb = await MatchResultsExtensions.MatchAsync(_resultResponseSuccess, async r => _successResult, useProblemDetails);
        var resultRespTProb = await MatchResultsExtensions.MatchAsync(_resultResponseSuccessT, async r => _successResult, useProblemDetails);

        Assert.Equal(_successResult, resultResp);
        Assert.Equal(_successResult, resultRespT);
        Assert.Equal(_successResult, resultRespProb);
        Assert.Equal(_successResult, resultRespTProb);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultResponseSuccessT,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultResponseSuccess,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultResponseSuccessT,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
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
            .Returns(_errorResult);

        var result = MatchResultsExtensions.Match(_resultError, (ResultResponseSuccess r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultT = MatchResultsExtensions.Match(_resultTError, (ResultResponseSuccess<string> r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultProb = MatchResultsExtensions.Match(_resultError, (ResultResponseSuccess r) => _successResult, useProblemDetails);
        var resultTProb = MatchResultsExtensions.Match(_resultTError, (ResultResponseSuccess<string> r) => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, result);
        Assert.Equal(_errorResult, resultT);
        Assert.Equal(_errorResult, resultProb);
        Assert.Equal(_errorResult, resultTProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultTError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultTError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsyncResult_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultBase>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_errorResult);

        var result = await MatchResultsExtensions.MatchAsync(_resultError, async r => _successResult, async r => _errorResult);
        var resultT = await MatchResultsExtensions.MatchAsync(_resultTError, async r => _successResult, async r => _errorResult);
        var resultProb = await MatchResultsExtensions.MatchAsync(_resultError, async r => _successResult, useProblemDetails);
        var resultTProb = await MatchResultsExtensions.MatchAsync(_resultTError, async r => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, result);
        Assert.Equal(_errorResult, resultT);
        Assert.Equal(_errorResult, resultProb);
        Assert.Equal(_errorResult, resultTProb);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultTError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Once);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultTError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
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
            .Returns(_errorResult);

        var resultResp = MatchResultsExtensions.Match(_resultResponseError, (ResultResponseSuccess r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultTResp = MatchResultsExtensions.Match(_resultResponseError, (ResultResponseSuccess<string> r) => _successResult, (ResultResponseError r) => _errorResult);
        var resultRespProb = MatchResultsExtensions.Match(_resultResponseError, (ResultResponseSuccess r) => _successResult, useProblemDetails);
        var resultTRespProb = MatchResultsExtensions.Match(_resultResponseError, (ResultResponseSuccess<string> r) => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, resultResp);
        Assert.Equal(_errorResult, resultTResp);
        Assert.Equal(_errorResult, resultRespProb);
        Assert.Equal(_errorResult, resultTRespProb);
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            null), Times.Exactly(2));
        
        _matcherMock.Verify(m => m.Match<IActionResult, IActionResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, IActionResult>>(),
            null,
            useProblemDetails), Times.Exactly(2));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task MatchAsyncResultResponse_ResultFailure_ShouldReturnError(bool? useProblemDetails)
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.Is<ResultResponse>(r => !r.IsSuccess),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync(_errorResult);

        var resultResp = await MatchResultsExtensions.MatchAsync(_resultResponseError, async r => _successResult, async r => _errorResult);
        var resultTResp = await MatchResultsExtensions.MatchAsync(_resultResponseError, async r => _successResult, async r => _errorResult);
        var resultRespProb = await MatchResultsExtensions.MatchAsync(_resultResponseError, async r => _successResult, useProblemDetails);
        var resultTRespProb = await MatchResultsExtensions.MatchAsync(_resultResponseError, async r => _successResult, useProblemDetails);

        Assert.Equal(_errorResult, resultResp);
        Assert.Equal(_errorResult, resultTResp);
        Assert.Equal(_errorResult, resultRespProb);
        Assert.Equal(_errorResult, resultTRespProb);
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            null), Times.Exactly(2));
        
        _matcherMock.Verify(m => m.MatchAsync<IActionResult, IActionResult>(
            _resultResponseError,
            It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
            null,
            useProblemDetails), Times.Exactly(2));
    }

    public void Dispose()
        => ResultMatcherFactory.Reset();
}
