/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests.Generated;

using ResultsOkAndBadRequest = Results<Ok<string>, BadRequest<string>>;

public class TypedMatchExtensionsSmokeTests : IDisposable
{
    private readonly Mock<ITypedResultMatcher> _matcherMock = new();
    private readonly Ok<string> _successResult = TypedResults.Ok("Success");
    private readonly BadRequest<string> _errorResult = TypedResults.BadRequest("Error");
    private readonly Result _resultSuccess = Result.Success();

    public TypedMatchExtensionsSmokeTests()
        => ResultMatcherFactory.TypedMatcherFactory = new(() => _matcherMock.Object);

    [Fact]
    public void Match_ShouldDelegateToMatcher()
    {
        _matcherMock
            .Setup(m => m.Match(
                It.IsAny<ResultBase>(),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<Func<ResultResponse, ResultsOkAndBadRequest>>(),
                It.IsAny<bool?>()))
            .Returns((ResultsOkAndBadRequest)_successResult);

        var result = MatchTResultsExtensions.Match<ResultsOkAndBadRequest>(
            _resultSuccess,
            onSuccess: r => _successResult,
            onFailure: r => _errorResult);

        Assert.Equal(_successResult, result.Result);

        _matcherMock.Verify(m => m.Match(
            _resultSuccess,
            It.IsNotNull<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            It.IsNotNull<Func<ResultResponse, ResultsOkAndBadRequest>>(),
            null), Times.Once);
    }

    [Fact]
    public async Task MatchAsync_ShouldDelegateToMatcher()
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.IsAny<ResultBase>(),
                It.IsAny<Func<ResultResponse, Task<ResultsOkAndBadRequest>>>(),
                It.IsAny<Func<ResultResponse, Task<ResultsOkAndBadRequest>>>(),
                It.IsAny<bool?>()))
            .ReturnsAsync((ResultsOkAndBadRequest)_successResult);

        var result = await MatchTResultsExtensions.MatchAsync<ResultsOkAndBadRequest>(
            _resultSuccess,
            onSuccess: async r => _successResult,
            onFailure: async r => _errorResult);

        Assert.Equal(_successResult, result.Result);

        _matcherMock.Verify(m => m.MatchAsync(
            _resultSuccess,
            It.IsNotNull<Func<ResultResponse, Task<ResultsOkAndBadRequest>>>(),
            It.IsNotNull<Func<ResultResponse, Task<ResultsOkAndBadRequest>>>(),
            null), Times.Once);
    }

    public void Dispose()
        => ResultMatcherFactory.Reset();
}
