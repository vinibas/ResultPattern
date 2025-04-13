/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

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

using ResultsOkAndJson = Results<Ok<ResultResponse>, JsonHttpResult<ResultResponseError>>;
using ResultsCreatedAndJson = Results<Created<ResultResponse>, JsonHttpResult<ResultResponseError>>;
using ResultsOkAndProblem = Results<Ok<ResultResponse>, ProblemHttpResult>;
using ResultsCreatedAndProblem = Results<Created<ResultResponse>, ProblemHttpResult>;
using ResultsOkAndBadRequest = Results<Ok<ResultResponse>, BadRequest<ResultResponse>>;
using ResultsCreatedAndBadRequest = Results<Created<ResultResponse>, BadRequest<ResultResponse>>;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class MatchTResultsExtensionsTests
{
    public static TheoryData<bool, bool?> AllScenariosTestData => MatchTestsHelper.AllScenariosTestData;

    public MatchTResultsExtensionsTests()
        => GlobalConfiguration.UseProblemDetails = true;

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResult_ResultSuccess_ShouldReturnOnSuccess(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;

        #region Omit onFailure

        var actResultOmitOnFailure = MatchTestsHelper.ResultSuccess
            .Match<IResult, IResult>(ActionForSuccess, useProblemDetailsParam);

        var actResultOmitOnFailureTyped = MatchTestsHelper.ResultSuccess
            .Match<ResultsCreatedAndProblem, Created<ResultResponse>>
            (ActionForSuccessTyped, useProblemDetailsParam);

        var actResultTOmitOnFailure = MatchTestsHelper.ResultTSuccess
            .Match<IResult, IResult>(ActionForSuccessT, useProblemDetailsParam);

        var actResultTOmitOnFailureTyped = MatchTestsHelper.ResultTSuccess
            .Match<ResultsOkAndProblem, Ok<ResultResponse>>
            (ActionForSuccessTTyped, useProblemDetailsParam);

        #endregion
        #region Passing onFailure

        var actResultPassingOnFailure = MatchTestsHelper.ResultSuccess
            .Match<IResult, IResult, IResult>(ActionForSuccess, ActionForFailure);

        var actResultPassingOnFailureTyped = MatchTestsHelper.ResultSuccess
            .Match<ResultsCreatedAndBadRequest, Created<ResultResponse>, BadRequest<ResultResponse>>
            (ActionForSuccessTyped, ActionForFailureTyped);

        var actResultTPassingOnFailure = MatchTestsHelper.ResultTSuccess
            .Match<IResult, IResult, IResult>(ActionForSuccessT, ActionForFailure);

        var actResultTPassingOnFailureTyped = MatchTestsHelper.ResultTSuccess
            .Match<ResultsOkAndBadRequest, Ok<ResultResponse>, BadRequest<ResultResponse>>
            (ActionForSuccessTTyped, ActionForFailureTyped);

        #endregion

        AssertMatchSuccess(actResultOmitOnFailure, false);
        AssertMatchSuccess(actResultOmitOnFailureTyped, true, true);
        AssertMatchTSuccess(actResultTOmitOnFailure, false);
        AssertMatchTSuccess(actResultTOmitOnFailureTyped, true, true);
        
        AssertMatchSuccess(actResultPassingOnFailure, false);
        AssertMatchSuccess(actResultPassingOnFailureTyped, true, false);
        AssertMatchTSuccess(actResultTPassingOnFailure, false);
        AssertMatchTSuccess(actResultTPassingOnFailureTyped, true, false);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResult_ResultFailure_ShouldReturnError(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var useProblemDetailsValue = useProblemDetailsParam ?? useProblemDetailsGlobal;

        #region Omit onFailure

        var actResultOmitOnFailure = MatchTestsHelper.ResultFailure
            .Match<IResult, IResult>(ActionForSuccess, useProblemDetailsParam);
            
        var actResultOmitOnFailureTyped = MatchForResultTyped(ActionForSuccessTyped, true, useProblemDetailsParam, useProblemDetailsValue);
        
        var actResultTOmitOnFailure = MatchTestsHelper.ResultTFailure
            .Match<IResult, IResult>(ActionForSuccessT, useProblemDetailsParam);

        var actResultTOmitOnFailureTyped = MatchForResultTyped(ActionForSuccessTTyped, true, useProblemDetailsParam, useProblemDetailsValue);

        #endregion
        #region Passing onFailure

        var actResultPassingOnFailure = MatchTestsHelper.ResultFailure
            .Match<IResult, IResult, IResult>(ActionForSuccess, ActionForFailure);
            
        var actResultPassingOnFailureTyped = MatchForResultTyped(ActionForSuccessTyped, false, useProblemDetailsParam, useProblemDetailsValue);

        var actResultTPassingOnFailure = MatchTestsHelper.ResultTFailure
            .Match<IResult, IResult, IResult>(ActionForSuccessT, ActionForFailure);

        var actResultTPassingOnFailureTyped = MatchForResultTyped(ActionForSuccessTTyped, false, useProblemDetailsParam, useProblemDetailsValue);

        #endregion

        AssertMatchFailure(actResultOmitOnFailure, true, false, false, useProblemDetailsValue);
        AssertMatchFailure(actResultOmitOnFailureTyped, true, true, false,useProblemDetailsValue);
        AssertMatchFailure(actResultTOmitOnFailure, true, false, true, useProblemDetailsValue);
        AssertMatchFailure(actResultTOmitOnFailureTyped, true, true, true, useProblemDetailsValue);

        AssertMatchFailure(actResultPassingOnFailure, false, false, false,useProblemDetailsValue);
        AssertMatchFailure(actResultPassingOnFailureTyped, false, true, false,useProblemDetailsValue);
        AssertMatchFailure(actResultTPassingOnFailure, false, false, true, useProblemDetailsValue);
        AssertMatchFailure(actResultTPassingOnFailureTyped, false, true, true, useProblemDetailsValue);
    }

    private IResult MatchForResultTyped<T_Success>(Func<ResultResponse, T_Success> onSuccess, bool omitOnFailure, bool? useProblemDetailsParam, bool useProblemDetailsValue)
        where T_Success : IResult
    {
        if (!omitOnFailure)
            return MatchTestsHelper.ResultFailure
                .Match<Results<T_Success, BadRequest<ResultResponse>>, T_Success, BadRequest<ResultResponse>>
                (onSuccess, ActionForFailureTyped);
        if (useProblemDetailsValue)
            return MatchTestsHelper.ResultFailure
                .Match<Results<T_Success, ProblemHttpResult>, T_Success>(onSuccess, useProblemDetailsParam);
        else
            return MatchTestsHelper.ResultFailure
                .Match<Results<T_Success, JsonHttpResult<ResultResponseError>>, T_Success>(onSuccess, useProblemDetailsParam);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResultResponse_ResultSuccess_ShouldReturnOnSuccess(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;

        #region Omit onFailure

        var actResultOmitOnFailure = MatchTestsHelper.ResultSuccess.ToResponse()
            .Match<IResult, IResult>(ActionForSuccess, useProblemDetailsParam);

        var actResultOmitOnFailureTyped = MatchTestsHelper.ResultSuccess.ToResponse()
            .Match<ResultsCreatedAndProblem, Created<ResultResponse>>
            (ActionForSuccessTyped, useProblemDetailsParam);

        var actResultTOmitOnFailure = MatchTestsHelper.ResultTSuccess.ToResponse()
            .Match<IResult, IResult>(ActionForSuccessT, useProblemDetailsParam);

        var actResultTOmitOnFailureTyped = MatchTestsHelper.ResultTSuccess.ToResponse()
            .Match<ResultsOkAndProblem, Ok<ResultResponse>>
            (ActionForSuccessTTyped, useProblemDetailsParam);

        #endregion
        #region Passing onFailure

        var actResultPassingOnFailure = MatchTestsHelper.ResultSuccess.ToResponse()
            .Match<IResult, IResult, IResult>(ActionForSuccess, ActionForFailure);

        var actResultPassingOnFailureTyped = MatchTestsHelper.ResultSuccess.ToResponse()
            .Match<ResultsCreatedAndBadRequest, Created<ResultResponse>, BadRequest<ResultResponse>>
            (ActionForSuccessTyped, ActionForFailureTyped);

        var actResultTPassingOnFailure = MatchTestsHelper.ResultTSuccess.ToResponse()
            .Match<IResult, IResult, IResult>(ActionForSuccessT, ActionForFailure);

        var actResultTPassingOnFailureTyped = MatchTestsHelper.ResultTSuccess.ToResponse()
            .Match<ResultsOkAndBadRequest, Ok<ResultResponse>, BadRequest<ResultResponse>>
            (ActionForSuccessTTyped, ActionForFailureTyped);

        #endregion

        AssertMatchSuccess(actResultOmitOnFailure, false);
        AssertMatchSuccess(actResultOmitOnFailureTyped, true, true);
        AssertMatchTSuccess(actResultTOmitOnFailure, false);
        AssertMatchTSuccess(actResultTOmitOnFailureTyped, true, true);
        
        AssertMatchSuccess(actResultPassingOnFailure, false);
        AssertMatchSuccess(actResultPassingOnFailureTyped, true, false);
        AssertMatchTSuccess(actResultTPassingOnFailure, false);
        AssertMatchTSuccess(actResultTPassingOnFailureTyped, true, false);
    }

    [Theory]
    [MemberData(nameof(AllScenariosTestData))]
    public void MatchResultResonse_ResultFailure_ShouldReturnError(bool useProblemDetailsGlobal, bool? useProblemDetailsParam)
    {
        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobal;
        var useProblemDetailsValue = useProblemDetailsParam ?? useProblemDetailsGlobal;

        #region Omit onFailure

        var actResultOmitOnFailure = MatchTestsHelper.ResultFailure
            .Match<IResult, IResult>(ActionForSuccess, useProblemDetailsParam);
            
        var actResultOmitOnFailureTyped = MatchForResultResponseTyped(ActionForSuccessTyped, true, useProblemDetailsParam, useProblemDetailsValue);
        
        var actResultTOmitOnFailure = MatchTestsHelper.ResultTFailure
            .Match<IResult, IResult>(ActionForSuccessT, useProblemDetailsParam);

        var actResultTOmitOnFailureTyped = MatchForResultResponseTyped(ActionForSuccessTTyped, true, useProblemDetailsParam, useProblemDetailsValue);

        #endregion
        #region Passing onFailure

        var actResultPassingOnFailure = MatchTestsHelper.ResultFailure
            .Match<IResult, IResult, IResult>(ActionForSuccess, ActionForFailure);
            
        var actResultPassingOnFailureTyped = MatchForResultResponseTyped(ActionForSuccessTyped, false, useProblemDetailsParam, useProblemDetailsValue);

        var actResultTPassingOnFailure = MatchTestsHelper.ResultTFailure
            .Match<IResult, IResult, IResult>(ActionForSuccessT, ActionForFailure);

        var actResultTPassingOnFailureTyped = MatchForResultResponseTyped(ActionForSuccessTTyped, false, useProblemDetailsParam, useProblemDetailsValue);

        #endregion

        AssertMatchFailure(actResultOmitOnFailure, true, false, false, useProblemDetailsValue);
        AssertMatchFailure(actResultOmitOnFailureTyped, true, true, false,useProblemDetailsValue);
        AssertMatchFailure(actResultTOmitOnFailure, true, false, true, useProblemDetailsValue);
        AssertMatchFailure(actResultTOmitOnFailureTyped, true, true, true, useProblemDetailsValue);

        AssertMatchFailure(actResultPassingOnFailure, false, false, false,useProblemDetailsValue);
        AssertMatchFailure(actResultPassingOnFailureTyped, false, true, false,useProblemDetailsValue);
        AssertMatchFailure(actResultTPassingOnFailure, false, false, true, useProblemDetailsValue);
        AssertMatchFailure(actResultTPassingOnFailureTyped, false, true, true, useProblemDetailsValue);
    }

    private IResult MatchForResultResponseTyped<T_Success>(Func<ResultResponse, T_Success> onSuccess, bool omitOnFailure, bool? useProblemDetailsParam, bool useProblemDetailsValue)
        where T_Success : IResult
    {
        if (!omitOnFailure)
            return MatchTestsHelper.ResultFailure.ToResponse()
                .Match<Results<T_Success, BadRequest<ResultResponse>>, T_Success, BadRequest<ResultResponse>>
                (onSuccess, ActionForFailureTyped);
        if (useProblemDetailsValue)
            return MatchTestsHelper.ResultFailure.ToResponse()
                .Match<Results<T_Success, ProblemHttpResult>, T_Success>(onSuccess, useProblemDetailsParam);
        else
            return MatchTestsHelper.ResultFailure.ToResponse()
                .Match<Results<T_Success, JsonHttpResult<ResultResponseError>>, T_Success>(onSuccess, useProblemDetailsParam);
    }
    #region Match Actions

    private IResult ActionForSuccess(ResultResponse resultResponse)
        => Results.Created((string?)null, resultResponse);

    private IResult ActionForSuccessT(ResultResponse resultResponse)
        => Results.Ok(resultResponse);

    private Created<ResultResponse> ActionForSuccessTyped(ResultResponse resultResponse)
        => TypedResults.Created((string?)null, resultResponse);

    private Ok<ResultResponse> ActionForSuccessTTyped(ResultResponse resultResponse)
        => TypedResults.Ok(resultResponse);

    private IResult ActionForFailure(ResultResponse resultResponse)
        => Results.BadRequest(resultResponse);

    private BadRequest<ResultResponse> ActionForFailureTyped(ResultResponse resultResponse)
        => TypedResults.BadRequest(resultResponse);

    #endregion

    #region Asserts

    private void AssertMatchSuccess(IResult result, bool isTyped, bool useProblemDetails = true)
    {
        if (isTyped)
        {
            INestedHttpResult resultFull = useProblemDetails ?
                Assert.IsType<ResultsCreatedAndProblem>(result) :
                Assert.IsType<ResultsCreatedAndBadRequest>(result);
            result = resultFull.Result;
        }
        
        var createdResult = Assert.IsType<Created<ResultResponse>>(result);
        var resultTyped = Assert.IsType<ResultResponseSuccess>(createdResult.Value);
        Assert.True(resultTyped.IsSuccess);
    }
    private void AssertMatchTSuccess(IResult result, bool isTyped, bool useProblemDetails = true)
    {
        if (isTyped)
        {
            INestedHttpResult resultFull = useProblemDetails ?
                Assert.IsType<ResultsOkAndProblem>(result) :
                Assert.IsType<ResultsOkAndBadRequest>(result);
            result = resultFull.Result;
        }
        
        var okResult = Assert.IsType<Ok<ResultResponse>>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal(MatchTestsHelper.SuccessValue, resultResponseValue.Data);
    }

    private void AssertMatchFailure(IResult result, bool omitOnFailure, bool isTyped, bool hasData, bool useProblemDetailsValue)
    {
        if (isTyped)
        {
            INestedHttpResult resultFull = (omitOnFailure, hasData, useProblemDetailsValue) switch
            {
                (true, true, true) => Assert.IsType<ResultsOkAndProblem>(result),
                (true, true, false) => Assert.IsType<ResultsOkAndJson>(result),
                (true, false, true) => Assert.IsType<ResultsCreatedAndProblem>(result),
                (true, false, false) => Assert.IsType<ResultsCreatedAndJson>(result),
                (false, true, _) => Assert.IsType<ResultsOkAndBadRequest>(result),
                (false, false, _) => Assert.IsType<ResultsCreatedAndBadRequest>(result),
            };

            result = resultFull.Result;
        }
        
        if (omitOnFailure && useProblemDetailsValue)
        {
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            var problemDetails = problemResult.ProblemDetails;
            Assert.False(problemDetails.Extensions["isSuccess"] as bool?);
            Assert.Equal(MatchTestsHelper.ErrorDescription, problemDetails.Detail);

            return;
        }

        IValueHttpResult<ResultResponse> objRes;
    
        if (omitOnFailure)
            objRes = Assert.IsType<JsonHttpResult<ResultResponseError>>(result);
        else
            objRes = Assert.IsType<BadRequest<ResultResponse>>(result);

        var resultResponseValue = Assert.IsType<ResultResponseError>(objRes.Value);
        Assert.False(resultResponseValue.IsSuccess);
        Assert.Equal([MatchTestsHelper.ErrorDescription], resultResponseValue.Errors);
    }

    #endregion
}
