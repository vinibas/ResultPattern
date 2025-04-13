/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class MatchResultsExtensionsTests
{
    public MatchResultsExtensionsTests()
        => GlobalConfiguration.UseProblemDetails = true;

    public static TheoryData<bool, bool?> AllScenariosTestData => MatchTestsHelper.AllScenariosTestData;

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResult_ResultSuccess_ShouldReturnOnSuccess(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;

        var actResultOmitOnFailure = MatchTestsHelper.ResultSuccess.Match(ActionForSuccess, useProblemDetailsParam);
        var actResultTOmitOnFailure = MatchTestsHelper.ResultTSuccess.Match(ActionForSuccessT, useProblemDetailsParam);

        var actResultPassingOnFailure = MatchTestsHelper.ResultSuccess.Match(ActionForSuccess, ActionForFailure);
        var actResultTPassingOnFailure = MatchTestsHelper.ResultTSuccess.Match(ActionForSuccessT, ActionForFailure);

        AssertMatchSuccess(actResultOmitOnFailure);
        AssertMatchSuccess(actResultPassingOnFailure);
        AssertMatchTSuccess(actResultTOmitOnFailure);
        AssertMatchTSuccess(actResultTPassingOnFailure);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResult_ResultFailure_ShouldReturnError(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;

        var actResultOmitOnFailure = MatchTestsHelper.ResultFailure.Match(ActionForSuccess, useProblemDetailsParam);
        var actResultTOmitOnFailure = MatchTestsHelper.ResultTFailure.Match(ActionForSuccessT, useProblemDetailsParam);

        var actResultPassingOnFailure = MatchTestsHelper.ResultFailure.Match(ActionForSuccess, ActionForFailure);
        var actResultTPassingOnFailure = MatchTestsHelper.ResultTFailure.Match(ActionForSuccessT, ActionForFailure);

        var useProblemDetailsValue = useProblemDetailsParam ?? useProblemDetailsGlobal;
        AssertMatchFailure(actResultOmitOnFailure, true, useProblemDetailsValue);
        AssertMatchFailure(actResultPassingOnFailure, false, useProblemDetailsValue);
        AssertMatchFailure(actResultTOmitOnFailure, true, useProblemDetailsValue);
        AssertMatchFailure(actResultTPassingOnFailure, false, useProblemDetailsValue);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResultResponse_ResultSuccess_ShouldReturnOnSuccess(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;

        var actResultOmitOnFailure = MatchTestsHelper.ResultSuccess.ToResponse().Match(ActionForSuccess, useProblemDetailsParam);
        var actResultTOmitOnFailure = MatchTestsHelper.ResultTSuccess.ToResponse().Match(ActionForSuccessT, useProblemDetailsParam);

        var actResultPassingOnFailure = MatchTestsHelper.ResultSuccess.ToResponse().Match(ActionForSuccess, ActionForFailure);
        var actResultTPassingOnFailure = MatchTestsHelper.ResultTSuccess.ToResponse().Match(ActionForSuccessT, ActionForFailure);

        AssertMatchSuccess(actResultOmitOnFailure);
        AssertMatchSuccess(actResultPassingOnFailure);
        AssertMatchTSuccess(actResultTOmitOnFailure);
        AssertMatchTSuccess(actResultTPassingOnFailure);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResultResponse_ResultFailure_ShouldReturnError(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;

        var actResultOmitOnFailure = MatchTestsHelper.ResultFailure.ToResponse().Match(ActionForSuccess, useProblemDetailsParam);
        var actResultTOmitOnFailure = MatchTestsHelper.ResultTFailure.ToResponse().Match(ActionForSuccessT, useProblemDetailsParam);

        var actResultPassingOnFailure = MatchTestsHelper.ResultFailure.ToResponse().Match(ActionForSuccess, ActionForFailure);
        var actResultTPassingOnFailure = MatchTestsHelper.ResultTFailure.ToResponse().Match(ActionForSuccessT, ActionForFailure);

        var useProblemDetailsValue = useProblemDetailsParam ?? useProblemDetailsGlobal;
        AssertMatchFailure(actResultOmitOnFailure, true, useProblemDetailsValue);
        AssertMatchFailure(actResultPassingOnFailure, false, useProblemDetailsValue);
        AssertMatchFailure(actResultTOmitOnFailure, true, useProblemDetailsValue);
        AssertMatchFailure(actResultTPassingOnFailure, false, useProblemDetailsValue);
    }

    #region Match Actions

    private IActionResult ActionForSuccess(ResultResponse resultResponse)
        => new CreatedResult((string?)null, resultResponse);

    private IActionResult ActionForSuccessT(ResultResponse resultResponse)
        => new OkObjectResult(resultResponse);

    private IActionResult ActionForFailure(ResultResponse resultResponse)
        => new BadRequestObjectResult(resultResponse);

    #endregion

    #region Asserts

    private void AssertMatchSuccess(IActionResult result)
    {
        var createdResult = Assert.IsType<CreatedResult>(result);
        var resultTyped = Assert.IsType<ResultResponseSuccess>(createdResult.Value);
        Assert.True(resultTyped.IsSuccess);
    }
    private void AssertMatchTSuccess(IActionResult result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal(MatchTestsHelper.SuccessValue, resultResponseValue.Data);
    }

    private void AssertMatchFailure(IActionResult result, bool omitOnFailure, bool useProblemDetailsValue)
    {
        if (omitOnFailure)
        {
            var objRes = Assert.IsType<ObjectResult>(result);

            if (useProblemDetailsValue)
            {
                var problemDetails = Assert.IsType<ProblemDetails>(objRes.Value);
                Assert.False(problemDetails.Extensions["isSuccess"] as bool?);
                Assert.Equal(MatchTestsHelper.ErrorDescription, problemDetails.Detail);
            }
            else
            {
                var resultResponseValue = Assert.IsType<ResultResponseError>(objRes.Value);
                Assert.False(resultResponseValue.IsSuccess);
                Assert.Equal([MatchTestsHelper.ErrorDescription], resultResponseValue.Errors);
            }
        }
        else
        {
            var objRes = Assert.IsType<BadRequestObjectResult>(result);
            var resultResponseValue = Assert.IsType<ResultResponseError>(objRes.Value);
            Assert.False(resultResponseValue.IsSuccess);
            Assert.Equal([MatchTestsHelper.ErrorDescription], resultResponseValue.Errors);
        }
    }

    #endregion
}
