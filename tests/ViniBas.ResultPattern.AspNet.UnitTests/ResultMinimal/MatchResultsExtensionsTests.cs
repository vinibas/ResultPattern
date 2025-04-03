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
using Microsoft.AspNetCore.Mvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class MatchResultsExtensionsTests
{
    public static TheoryData<bool, bool, bool?> AllScenariosTestData => MatchTestsHelper.AllScenariosTestData;

    public MatchResultsExtensionsTests()
        => GlobalConfiguration.UseProblemDetails = true;

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchOmitOnFailure_ResultSuccess_ShouldReturnOnSuccess(
        bool isAResponseType, bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var resultArranges = MatchTestsHelper.CreateResultSuccessArranges();

        var (resultMatched, resultMatchedT, resultTMatched, resultTMatchedT) = 
            CreateResultMatchedAct(resultArranges, isAResponseType, useProblemDetailsParam);

        AssertResultMatchSuccess(resultMatched);
        AssertResultMatchSuccess(resultMatchedT);
        AssertResultMatchTSuccess(resultTMatched);
        AssertResultMatchTSuccess(resultTMatchedT);
    }

    private (IResult ResultMatched, IResult ResultMatchedT, IResult ResultTMatched, IResult ResultTMatchedT)
        CreateResultMatchedAct(
        MatchTestsHelper.ResultArranges resultArranges, bool isAResponseType, bool? useProblemDetailsParam)
    {
        var resultMatched = isAResponseType ?
            resultArranges.ResultResponse
                .Match(rr => Results.Created((string?)null, rr), useProblemDetailsParam) :
            resultArranges.Result
                .Match(rr => Results.Created((string?)null, rr), useProblemDetailsParam);
        var resultMatchedT = isAResponseType ?
            resultArranges.ResultResponse
                .Match(rr => TypedResults.Created((string?)null, rr), useProblemDetailsParam) :
            resultArranges.Result
                .Match(rr => TypedResults.Created((string?)null, rr), useProblemDetailsParam);
        var resultTMatched = isAResponseType ?
            resultArranges.ResultResponseT
                .Match(rr => Results.Ok(rr), useProblemDetailsParam) :
            resultArranges.ResultT
                .Match(rr => Results.Ok(rr), useProblemDetailsParam);
        var resultTMatchedT = isAResponseType ?
            resultArranges.ResultResponseT
                .Match(rr => TypedResults.Ok(rr), useProblemDetailsParam) :
            resultArranges.ResultT
                .Match(rr => TypedResults.Ok(rr), useProblemDetailsParam);

        return (resultMatched, resultMatchedT, resultTMatched, resultTMatchedT);
    }

    private void AssertResultMatchSuccess(IResult result)
    {
        var createdResult = Assert.IsType<Created<ResultResponse>>(result);
        var resultTyped = Assert.IsType<ResultResponseSuccess>(createdResult.Value);
        Assert.True(resultTyped.IsSuccess);
    }

    private void AssertResultMatchTSuccess(IResult result)
    {
        var okResult = Assert.IsAssignableFrom<IValueHttpResult>(result);
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

        var (resultMatched, resultMatchedT, resultTMatched, resultTMatchedT) = 
            CreateResultMatchedAct(resultArranges, isAResponseType, useProblemDetailsParam);

        var useProblemDetails = useProblemDetailsParam ?? useProblemDetailsGlobal;
        AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultMatched, useProblemDetails);
        AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultMatchedT, useProblemDetails);
        AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultTMatched, useProblemDetails);
        AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultTMatchedT, useProblemDetails);
    }

    private void AssertResultMatchFailure<ErrorType>(IResult result, bool useProblemDetails)
        where ErrorType : IValueHttpResult
    {
        if (useProblemDetails)
        {
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.ProblemDetails);
            Assert.False(problemDetails.Extensions["isSuccess"] as bool?);
            Assert.Equal("Error", problemDetails.Detail);
        }
        else
        {
            var jsonResult = Assert.IsType<ErrorType>(result);
            var resultResponseError = Assert.IsType<ResultResponseError>(jsonResult.Value);
            Assert.False(resultResponseError.IsSuccess);
            Assert.Equal(["Error"], resultResponseError.Errors);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MatchPassingOnFailure_ResultResponseError_ShouldReturnOnFailure(bool isAResponseType)
    {
        Func<ResultResponse, IResult> onSuccess = response => Results.NoContent();
        Func<ResultResponse, IResult> onSuccessData = response => Results.Content("Test Data");
        Func<ResultResponse, IResult> onFailure = response => Results.BadRequest(response);

        Func<ResultResponse, IResult> onSuccessT = response => TypedResults.NoContent();
        Func<ResultResponse, IResult> onSuccessDataT = response => TypedResults.Content("Test Data");
        Func<ResultResponse, IResult> onFailureT = response => TypedResults.BadRequest(response);

        var resultArranges = MatchTestsHelper.CreateResultFailureArranges();
        var result = resultArranges.Result;
        var resultT = resultArranges.ResultT;
        var resultResponseError = resultArranges.ResultResponse;
        
        var resultMatched = isAResponseType ?
            resultResponseError.Match(onSuccess, onFailure) :
            result.Match(onSuccess, onFailure);
        var resultMatchedT = isAResponseType ?
            resultResponseError.Match(onSuccessT, onFailureT) :
            result.Match(onSuccessT, onFailureT);
        var resultMatchedData = isAResponseType ?
            resultResponseError.Match(onSuccessData, onFailure) :
            resultT.Match(onSuccessData, onFailure);
        var resultMatchedDataT = isAResponseType ?
            resultResponseError.Match(onSuccessDataT, onFailureT) :
            resultT.Match(onSuccessDataT, onFailureT);

        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultMatched, false);
        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultMatchedT, false);
        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultMatchedData, false);
        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultMatchedDataT, false);
    }
}
