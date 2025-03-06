/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

public class MatchResultsExtensionsTests
{
    private readonly Result _resultError = Result.Failure(new Error([ new ("err1", "Error 1"), new ("err2", "Error 2") ], ErrorTypes.Validation));
    private readonly ResultResponseError _resultResponseError = new ([ "Error 1", "Error 2" ], ErrorTypes.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_ResultSuccess_ShouldReturnOnSuccess(bool isAResponseType)
    {
        var resultResponse = ResultResponseSuccess.Create();
        var resultResponseT = ResultResponseSuccess.Create("Test Data");
        var result = Result.Success();
        var resultT = Result<string>.Success("Test Data");
        
        var resultMatched = isAResponseType ?
            resultResponse.Match(_ => Results.Created()) :
            result.Match(_ => Results.Created());
        var resultMatchedT = isAResponseType ?
            resultResponse.Match(_ => TypedResults.Created()) :
            result.Match(_ => TypedResults.Created());
        var resultTMatched = isAResponseType ?
            resultResponseT.Match(rr => Results.Ok(rr)) :
            resultT.Match(rr => Results.Ok(rr));
        var resultTMatchedT = isAResponseType ?
            resultResponseT.Match(rr => TypedResults.Ok(rr)) :
            resultT.Match(rr => TypedResults.Ok(rr));

        Assert.IsAssignableFrom<IResult>(resultMatched);
        Assert.IsAssignableFrom<IResult>(resultMatchedT);
        Assert.IsAssignableFrom<IResult>(resultTMatched);
        Assert.IsAssignableFrom<IResult>(resultTMatchedT);
        
        Assert.IsType<Created>(resultMatchedT);
        Assert.IsType<Ok<ResultResponse>>(resultTMatchedT);

        AssertMatchTypedSuccess(resultTMatchedT);
    }

    private void AssertMatchTypedSuccess(IResult result)
    {
        var okResult = Assert.IsAssignableFrom<IValueHttpResult>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_ResultResponseError_ShouldReturnOnFailure(bool isAResponseType)
    {
        Func<ResultResponse, IResult> onSuccess = response => Results.NoContent();
        Func<ResultResponse, IResult> onSuccessData = response => Results.Content("Test Data");
        Func<ResultResponse, IResult> onFailure = response => Results.BadRequest();

        Func<ResultResponse, IResult> onSuccessT = response => TypedResults.NoContent();
        Func<ResultResponse, IResult> onSuccessDataT = response => TypedResults.Content("Test Data");
        Func<ResultResponse, IResult> onFailureT = response => TypedResults.BadRequest();

        var resultFailure = Result.Failure(Error.Failure("1", "Error"));
        var resultFailureT = Result<string>.Failure(Error.Failure("1", "Error"));

        var result = isAResponseType ?
            _resultResponseError.Match(onSuccess, onFailure) :
            resultFailure.Match(onSuccess, onFailure);
        var resultT = isAResponseType ?
            _resultResponseError.Match(onSuccessT, onFailureT) :
            resultFailure.Match(onSuccessT, onFailureT);
        var resultData = isAResponseType ?
            _resultResponseError.Match(onSuccessData, onFailure) :
            resultFailureT.Match(onSuccessData, onFailure);
        var resultDataT = isAResponseType ?
            _resultResponseError.Match(onSuccessDataT, onFailureT) :
            resultFailureT.Match(onSuccessDataT, onFailureT);

        Assert.IsAssignableFrom<IResult>(result);
        Assert.IsType<BadRequest>(resultT);
        Assert.IsAssignableFrom<IResult>(resultData);
        Assert.IsType<BadRequest>(resultDataT);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_ResultError_ShouldReturnProblemDetailsOnFailure(bool isAResponseType)
    {
        var result = isAResponseType ?
            _resultResponseError.Match(_ => Results.NoContent()) :
            _resultError.Match(_ => Results.NoContent());
        var resultT = isAResponseType ?
            _resultResponseError.Match(_ => TypedResults.NoContent()) :
            _resultError.Match(_ => TypedResults.NoContent());
        var resultData = isAResponseType ?
            _resultResponseError.Match(_ => Results.Content("Test Data")) :
            _resultError.Match(_ => Results.Content("Test Data"));
        var resultDataT = isAResponseType ?
            _resultResponseError.Match(_ => TypedResults.Content("Test Data")) :
            _resultError.Match(_ => TypedResults.Content("Test Data"));

        var objRes = Assert.IsAssignableFrom<IResult>(result);
        var objResT = Assert.IsType<ProblemHttpResult>(resultT);
        var probDetT = Assert.IsType<ProblemDetails>(objResT.ProblemDetails);
        Assert.Equal(_problemDetailsDetail, probDetT.Detail);

        var objResData = Assert.IsAssignableFrom<IValueHttpResult>(resultData);
        var probDet = Assert.IsType<ProblemDetails>(objResData.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);

        var objResDataT = Assert.IsAssignableFrom<ProblemHttpResult>(resultDataT);
        probDet = Assert.IsType<ProblemDetails>(objResDataT.ProblemDetails);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }
}
