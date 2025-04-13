/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

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

    private IResult ActionForSuccess(ResultResponse resultResponse)
        => Results.Created((string?)null, resultResponse);

    private IResult ActionForSuccessT(ResultResponse resultResponse)
        => Results.Ok(resultResponse);

    private IResult ActionForFailure(ResultResponse resultResponse)
        => Results.BadRequest(resultResponse);

    #endregion

    #region Asserts

    private void AssertMatchSuccess(IResult result)
    {
        var createdResult = Assert.IsType<Created<ResultResponse>>(result);
        var resultTyped = Assert.IsType<ResultResponseSuccess>(createdResult.Value);
        Assert.True(resultTyped.IsSuccess);
    }
    private void AssertMatchTSuccess(IResult result)
    {
        var okResult = Assert.IsType<Ok<ResultResponse>>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal(MatchTestsHelper.SuccessValue, resultResponseValue.Data);
    }

    private void AssertMatchFailure(IResult result, bool omitOnFailure, bool useProblemDetailsValue)
    {
        if (omitOnFailure)
        {
            if (useProblemDetailsValue)
            {
                var problemResult = Assert.IsType<ProblemHttpResult>(result);
                var problemDetails = problemResult.ProblemDetails;
                Assert.False(problemDetails.Extensions["isSuccess"] as bool?);
                Assert.Equal(MatchTestsHelper.ErrorDescription, problemDetails.Detail);
            }
            else
            {
                var jsonResult = Assert.IsType<JsonHttpResult<ResultResponseError>>(result);
                var resultResponseValue = Assert.IsType<ResultResponseError>(jsonResult.Value);
                Assert.False(resultResponseValue.IsSuccess);
                Assert.Equal([MatchTestsHelper.ErrorDescription], resultResponseValue.Errors);
            }
        }
        else
        {
            var objRes = Assert.IsType<BadRequest<ResultResponse>>(result);
            var resultResponseValue = Assert.IsType<ResultResponseError>(objRes.Value);
            Assert.False(resultResponseValue.IsSuccess);
            Assert.Equal([MatchTestsHelper.ErrorDescription], resultResponseValue.Errors);
        }
    }

    #endregion
}
