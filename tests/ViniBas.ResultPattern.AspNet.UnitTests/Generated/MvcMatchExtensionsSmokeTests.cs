/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.Mvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.Generated;

public class MvcMatchExtensionsSmokeTests : IDisposable
{
    private readonly Mock<ISimpleResultMatcher<IActionResult>> _matcherMock = new();
    private readonly FakeHelpers _fakes = new();

    public MvcMatchExtensionsSmokeTests()
        => ResultMatcherFactory.ActionResultMatcherFactory = new(() => _matcherMock.Object);

    [Fact]
    public void Match_ShouldDelegateToMatcher()
    {
        _matcherMock
            .Setup(m => m.Match(
                It.IsAny<ResultBase>(),
                It.IsAny<Func<ResultResponse, IActionResult>>(),
                It.IsAny<Func<ResultResponse, IActionResult>>()))
            .Returns(_fakes.ActionResultSuccess);

        var result = MvcMatchResultsExtensions.Match(
            _fakes.ResultSuccess,
            onSuccess: r => _fakes.ActionResultSuccess,
            onFailure: r => _fakes.ActionResultError);

        Assert.Equal(_fakes.ActionResultSuccess, result);

        _matcherMock.Verify(m => m.Match(
            _fakes.ResultSuccess,
            It.IsNotNull<Func<ResultResponse, IActionResult>>(),
            It.IsNotNull<Func<ResultResponse, IActionResult>>()),
            Times.Once);
    }

    [Fact]
    public async Task MatchAsync_ShouldDelegateToMatcher()
    {
        _matcherMock
            .Setup(m => m.MatchAsync(
                It.IsAny<ResultBase>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>(),
                It.IsAny<Func<ResultResponse, Task<IActionResult>>>()))
            .ReturnsAsync(_fakes.ActionResultSuccess);

        var result = await MvcMatchResultsExtensions.MatchAsync(
            _fakes.ResultSuccess,
            onSuccess: async r => _fakes.ActionResultSuccess,
            onFailure: async r => _fakes.ActionResultError);

        Assert.Equal(_fakes.ActionResultSuccess, result);

        _matcherMock.Verify(m => m.MatchAsync(
            _fakes.ResultSuccess,
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>(),
            It.IsNotNull<Func<ResultResponse, Task<IActionResult>>>()),
            Times.Once);
    }

    public void Dispose()
        => ResultMatcherFactory.Reset();
}
