/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class MatchResultsExtensionsTests
{
    public MatchResultsExtensionsTests()
    {
        GlobalConfiguration.UseProblemDetails = true;
    }

    public static TheoryData<bool, bool, bool?> AllScenariosTestData => MatchTestsHelper.AllScenariosTestData;

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchOmitOnFailure_ResultSuccess_ShouldReturnOnSuccess(
        bool isAResponseType, bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var resultArranges = MatchTestsHelper.CreateResultSuccessArranges();

        var (resultMatched, resultMatchedT) = CreateResultMatchedAct(resultArranges, isAResponseType, useProblemDetailsParam);

        AssertMatchSuccess(resultMatched);
        AssertMatchTSuccess(resultMatchedT);
    }

    private (IActionResult ResultMatched, IActionResult ResultMatchedT) CreateResultMatchedAct(
        MatchTestsHelper.ResultArranges resultArranges, bool isAResponseType, bool? useProblemDetailsParam)
    {

        var resultMatched = isAResponseType ?
            resultArranges.ResultResponse
                .Match(rr => new CreatedResult((string?)null, rr), useProblemDetailsParam) :
            resultArranges.Result
                .Match(rr => new CreatedResult((string?)null, rr), useProblemDetailsParam);
        var resultMatchedT = isAResponseType ?
            resultArranges.ResultResponseT
                .Match(rr => new OkObjectResult(rr), useProblemDetailsParam) :
            resultArranges.ResultT
                .Match(rr => new OkObjectResult(rr), useProblemDetailsParam);

        return (resultMatched, resultMatchedT);
    }

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
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchOmitOnFailure_ResultFailure_ShouldReturnOnFailure(
        bool isAResponseType, bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var resultArranges = MatchTestsHelper.CreateResultFailureArranges();
        
        var (resultMatched, resultMatchedT) = CreateResultMatchedAct(resultArranges, isAResponseType, useProblemDetailsParam);

        AssertMatchFailure(resultMatched, useProblemDetailsParam ?? useProblemDetailsGlobal);
        AssertMatchFailure(resultMatchedT, useProblemDetailsParam ?? useProblemDetailsGlobal);
    }

    private void AssertMatchFailure(IActionResult result, bool useProblemDetails)
    {
        var objRes = Assert.IsType<ObjectResult>(result);
        if (useProblemDetails)
        {
            var problemDetails = Assert.IsType<ProblemDetails>(objRes.Value);
            Assert.False(problemDetails.Extensions["isSuccess"] as bool?);
            Assert.Equal("Error", problemDetails.Detail);
        }
        else
        {
            var resultResponseValue = Assert.IsType<ResultResponseError>(objRes.Value);
            Assert.False(resultResponseValue.IsSuccess);
            Assert.Equal(["Error"], resultResponseValue.Errors);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MatchPassingOnFailure_ResultFailure_ShouldReturnOnFailure(bool isAResponseType)
    {
        var resultArranges = MatchTestsHelper.CreateResultFailureArranges();
        var result = resultArranges.Result;
        var resultT = resultArranges.ResultT;
        var resultResponseError = resultArranges.ResultResponse;
        
        Func<ResultResponse, IActionResult> onSuccess = response => new NoContentResult();
        Func<ResultResponse, IActionResult> onSuccessData = response => new ObjectResult(response);
        Func<ResultResponse, IActionResult> onFailure = response => new BadRequestObjectResult(response);

        var resultMatched = isAResponseType ?
            resultResponseError.Match(onSuccess, onFailure) :
            result.Match(onSuccess, onFailure);
        var resultMatchedT = isAResponseType ?
            resultResponseError.Match(onSuccessData, onFailure) :
            resultT.Match(onSuccessData, onFailure);

        foreach (var resultMatch in new IActionResult[] { resultMatched, resultMatchedT })
        {
            var badRequestResultCast = Assert.IsType<BadRequestObjectResult>(resultMatch);
            var resultResponseErrorCast = Assert.IsType<ResultResponseError>(badRequestResultCast.Value);
            Assert.False(resultResponseErrorCast.IsSuccess);
            Assert.Equal(["Error"], resultResponseErrorCast.Errors);
        }
    }

}
