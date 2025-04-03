/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class MatchTResultsExtensionsTests
{
    public static TheoryData<bool, bool, bool?> AllScenariosTestData => MatchTestsHelper.AllScenariosTestData;

    public MatchTResultsExtensionsTests()
        => GlobalConfiguration.UseProblemDetails = true;

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchTOmitOnFailure_ResultSuccess_ShouldReturnOnSuccess(
        bool isAResponseType, bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var resultArranges = MatchTestsHelper.CreateResultSuccessArranges();

        var (resultMatched, resultMatchedT, resultTDataMatched, resultTDataMatchedT) = 
            CreateResultMatchedAct<ProblemHttpResult>(resultArranges, isAResponseType, useProblemDetailsParam);

        AssertResultMatchSuccess(resultMatched);
        AssertResultMatchSuccess(resultMatchedT.Result);
        AssertResultMatchTSuccess(resultTDataMatched);
        AssertResultMatchTSuccess(resultTDataMatchedT.Result);
    }

    private (
        IResult ResultMatched, 
        Results<Created<ResultResponse>, ExpectedErrorType> ResultMatchedT,
        IResult ResultTMatched,
        Results<Ok<ResultResponse>, ExpectedErrorType> ResultTMatchedT)
            CreateResultMatchedAct<ExpectedErrorType>(
        MatchTestsHelper.ResultArranges resultArranges, bool isAResponseType, bool? useProblemDetailsParam)
        where ExpectedErrorType : IResult
    {
        if (isAResponseType)
        {
            var resultMatched = resultArranges.ResultResponse
                    .Match<IResult, IResult>
                    (rr => Results.Created((string?)null, rr), useProblemDetailsParam);
            var resultMatchedT = resultArranges.ResultResponse
                    .Match<Results<Created<ResultResponse>, ExpectedErrorType>, Created<ResultResponse>>
                    (rr => TypedResults.Created((string?)null, rr), useProblemDetailsParam);
            var resultTDataMatched = resultArranges.ResultResponseT
                    .Match<IResult, IResult>
                    (rr => Results.Ok(rr), useProblemDetailsParam);
            var resultTDataMatchedT = resultArranges.ResultResponseT
                    .Match<Results<Ok<ResultResponse>, ExpectedErrorType>, Ok<ResultResponse>>
                    (rr => TypedResults.Ok(rr), useProblemDetailsParam);
            
            return (resultMatched, resultMatchedT, resultTDataMatched, resultTDataMatchedT);
        }
        else
        {
            var resultMatched = resultArranges.Result
                    .Match<IResult, IResult>
                    (rr => Results.Created((string?)null, rr), useProblemDetailsParam);
            var resultMatchedT = resultArranges.Result
                    .Match<Results<Created<ResultResponse>, ExpectedErrorType>, Created<ResultResponse>>
                    (rr => TypedResults.Created((string?)null, rr), useProblemDetailsParam);
            var resultTDataMatched = resultArranges.ResultT
                    .Match<IResult, IResult>
                    (rr => Results.Ok(rr), useProblemDetailsParam);
            var resultTDataMatchedT = resultArranges.ResultT
                    .Match<Results<Ok<ResultResponse>, ExpectedErrorType>, Ok<ResultResponse>>
                    (rr => TypedResults.Ok(rr), useProblemDetailsParam);
        
            return (resultMatched, resultMatchedT, resultTDataMatched, resultTDataMatchedT);
        }
    }

    private void AssertResultMatchSuccess(IResult result)
    {
        var createdResult = Assert.IsType<Created<ResultResponse>>(result);
        var resultTyped = Assert.IsType<ResultResponseSuccess>(createdResult.Value);
        Assert.True(resultTyped.IsSuccess);
    }

    private void AssertResultMatchTSuccess(IResult result)
    {
        var okResult = Assert.IsType<Ok<ResultResponse>>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchTOmitOnFailure_ResultFailure_ShouldReturnOnFailure(
        bool isAResponseType, bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var resultArranges = MatchTestsHelper.CreateResultFailureArranges();
        var useProblemDetails = useProblemDetailsParam ?? useProblemDetailsGlobal;

        if (useProblemDetails)
        {
            var (resultMatched, resultMatchedT, resultTDataMatched, resultTDataMatchedT) = 
                CreateResultMatchedAct<ProblemHttpResult>(resultArranges, isAResponseType, useProblemDetailsParam);

            AssertResultMatchFailure<ProblemHttpResult>(resultMatched, useProblemDetails);
            AssertResultMatchFailure<ProblemHttpResult>(resultMatchedT.Result, useProblemDetails);
            AssertResultMatchFailure<ProblemHttpResult>(resultTDataMatched, useProblemDetails);
            AssertResultMatchFailure<ProblemHttpResult>(resultTDataMatchedT.Result, useProblemDetails);
        }
        else
        {
            var (resultMatched, resultMatchedT, resultTDataMatched, resultTDataMatchedT) = 
                CreateResultMatchedAct<JsonHttpResult<ResultResponseError>>(resultArranges, isAResponseType, useProblemDetailsParam);

            AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultMatched, useProblemDetails);
            AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultMatchedT.Result, useProblemDetails);
            AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultTDataMatched, useProblemDetails);
            AssertResultMatchFailure<JsonHttpResult<ResultResponseError>>(resultTDataMatchedT.Result, useProblemDetails);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MatchTPassingOnFailure_ResultError_ShouldReturnOnFailure(bool isAResponseType)
    {
        var resultArranges = MatchTestsHelper.CreateResultFailureArranges();

        IResult resultMatched, resultDataMatched;
        Results<NoContent, BadRequest<ResultResponse>> resultTMatchedT;
        Results<ContentHttpResult, BadRequest<ResultResponse>> resultDataMatchedT;

        Func<ResultResponse, IResult> onSuccess = response => Results.NoContent();
        Func<ResultResponse, IResult> onSuccessData = response => Results.Content("Test Data");
        Func<ResultResponse, IResult> onFailure = response => Results.BadRequest(response);

        Func<ResultResponse, NoContent> onSuccessT = response => TypedResults.NoContent();
        Func<ResultResponse, ContentHttpResult> onSuccessDataT = response => TypedResults.Content("Test Data");
        Func<ResultResponse, BadRequest<ResultResponse>> onFailureT = response => TypedResults.BadRequest(response);

        if (isAResponseType)
        {
            resultMatched = resultArranges.ResultResponse
                .Match<IResult, IResult, IResult>(onSuccess, onFailure);
            resultTMatchedT = resultArranges.ResultResponse
                .Match<Results<NoContent, BadRequest<ResultResponse>>, NoContent, BadRequest<ResultResponse>>
                (onSuccessT, onFailureT);
            resultDataMatched = resultArranges.ResultResponse
                .Match<IResult, IResult, IResult>(onSuccessData, onFailure);
            resultDataMatchedT = resultArranges.ResultResponse
                .Match<Results<ContentHttpResult, BadRequest<ResultResponse>>, ContentHttpResult, BadRequest<ResultResponse>>
                (onSuccessDataT, onFailureT);
        }
        else
        {
            resultMatched = resultArranges.Result
                .Match<IResult, IResult, IResult>(onSuccess, onFailure);
            resultTMatchedT = resultArranges.Result
                .Match<Results<NoContent, BadRequest<ResultResponse>>, NoContent, BadRequest<ResultResponse>>
                (onSuccessT, onFailureT);
            resultDataMatched = resultArranges.ResultT
                .Match<IResult, IResult, IResult>(onSuccessData, onFailure);
            resultDataMatchedT = resultArranges.ResultT
                .Match<Results<ContentHttpResult, BadRequest<ResultResponse>>, ContentHttpResult, BadRequest<ResultResponse>>
                (onSuccessDataT, onFailureT);
        }

        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultMatched, false);
        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultTMatchedT.Result, false);
        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultDataMatched, false);
        AssertResultMatchFailure<BadRequest<ResultResponse>>(resultDataMatchedT.Result, false);
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
            var badRequestResult = Assert.IsType<ErrorType>(result);
            var resultTyped = Assert.IsType<ResultResponseError>(badRequestResult.Value);
            Assert.False(resultTyped.IsSuccess);
            Assert.Equal(["Error"], resultTyped.Errors);
        }
    }
}
