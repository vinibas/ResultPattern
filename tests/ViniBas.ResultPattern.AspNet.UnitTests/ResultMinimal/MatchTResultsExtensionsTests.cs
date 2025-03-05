using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit.Sdk;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMinimal;

public class MatchTResultsExtensionsTests
{
    private readonly Result _resultError = Result.Failure(new Error([ new ("err1", "Error 1"), new ("err2", "Error 2") ], ErrorTypes.Validation));
    private readonly ResultResponseError _resultResponseError = new ([ "Error 1", "Error 2" ], ErrorTypes.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MatchT_ResultResponseSuccess_ShouldReturnOnSuccess(bool isAResponseType)
    {
        var resultResponse = new ResultResponseSuccess();
        var resultResponseT = new ResultResponseSuccess<string>("Test Data");
        var result = Result.Success();
        var resultT = Result<string>.Success("Test Data");
        
        var resultMatched = isAResponseType ?
            resultResponse.Match<IResult, Created>(_ => Results.Created()) :
            result.Match<IResult, Created>(_ => Results.Created());

        var resultMatchedT = isAResponseType ?
            resultResponse.Match<Results<Created, ProblemHttpResult>, Created>(_ => TypedResults.Created()) :
            result.Match<Results<Created, ProblemHttpResult>, Created>(_ => TypedResults.Created());
            
        var resultTDataMatched = isAResponseType ?
            resultResponseT.Match<IResult, Created>(rr => Results.Ok(rr)) :
            resultT.Match<IResult, Created>(rr => Results.Ok(rr));

        var resultTDataMatchedT = isAResponseType ?
            resultResponseT.Match<Results<Ok<ResultResponse>, ProblemHttpResult>, Ok<ResultResponse>>
                (rr => TypedResults.Ok(rr)) :
            resultT.Match<Results<Ok<ResultResponse>, ProblemHttpResult>, Ok<ResultResponse>>
                (rr => TypedResults.Ok(rr));

        Assert.IsAssignableFrom<IResult>(resultMatched);
        Assert.IsAssignableFrom<IResult>(resultMatchedT);
        Assert.IsType<Results<Created, ProblemHttpResult>>(resultMatchedT);
        Assert.IsAssignableFrom<IResult>(resultTDataMatched);
        AssertMatchTypedSuccess(resultTDataMatchedT);
    }

    private void AssertMatchTypedSuccess(Results<Ok<ResultResponse>, ProblemHttpResult> result)
    {
        var okResult = Assert.IsType<Ok<ResultResponse>>(result.Result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    // [Theory]
    // [InlineData(true)]
    // [InlineData(false)]
    // public void MatchT_ResultResponseError_ShouldReturnOnFailure(bool isAResponseType)
    // {
    //     Func<ResultResponse, IResult> onSuccess = response => Results.NoContent();
    //     Func<ResultResponse, IResult> onSuccessData = response => Results.Content("Test Data");
    //     Func<ResultResponse, IResult> onFailure = response => Results.BadRequest();

    //     Func<ResultResponse, NoContent> onSuccessT = response => TypedResults.NoContent();
    //     Func<ResultResponse, ContentHttpResult> onSuccessDataT = response => TypedResults.Content("Test Data");
    //     Func<ResultResponse, BadRequest> onFailureT = response => TypedResults.BadRequest();

    //     var result = _resultResponseError.Match<IResult, IResult, IResult>(onSuccess, onFailure);
    //     var resultT = _resultResponseError.Match<Results<NoContent, BadRequest>, NoContent, BadRequest>(onSuccessT, onFailureT);
    //     var resultData = _resultResponseError.Match<IResult, IResult, IResult>(onSuccessData, onFailure);
    //     var resultDataT = _resultResponseError.Match<Results<NoContent, BadRequest>, NoContent, BadRequest>(onSuccessDataT, onFailureT);

    //     Assert.IsAssignableFrom<IResult>(result);
    //     Assert.IsType<BadRequest>(resultT);
    //     Assert.IsAssignableFrom<IResult>(resultData);
    //     Assert.IsType<BadRequest>(resultDataT);
    // }

    // [Fact]
    // public void MatchT_ResultError_ShouldReturnOnFailure()
    // {
    //     Func<ResultResponse, IResult> onSuccess = response => Results.NoContent();
    //     Func<ResultResponse, IResult> onSuccessData = response => Results.Content("Test Data");
    //     Func<ResultResponse, IResult> onFailure = response => Results.BadRequest();

    //     Func<ResultResponse, IResult> onSuccessT = response => TypedResults.NoContent();
    //     Func<ResultResponse, IResult> onSuccessDataT = response => TypedResults.Content("Test Data");
    //     Func<ResultResponse, IResult> onFailureT = response => TypedResults.BadRequest();


    //     var result = Result.Failure(Error.Failure("1", "Error")).Match(onSuccess, onFailure);
    //     var resultT = Result.Failure(Error.Failure("1", "Error")).Match(onSuccessT, onFailureT);
    //     var resultData = Result<string>.Failure(Error.Failure("1", "Error")).Match(onSuccessData, onFailure);
    //     var resultDataT = Result<string>.Failure(Error.Failure("1", "Error")).Match(onSuccessDataT, onFailureT);

    //     Assert.IsAssignableFrom<IResult>(result);
    //     Assert.IsType<BadRequest>(resultT);
    //     Assert.IsAssignableFrom<IResult>(resultData);
    //     Assert.IsType<BadRequest>(resultDataT);
    // }

    // [Fact]
    // public void MatchT_ResultResponseError_ShouldReturnProblemDetailsOnFailure()
    // {
    //     var result = _resultResponseError.Match(_ => Results.NoContent());
    //     var resultT = _resultResponseError.Match(_ => TypedResults.NoContent());
    //     var resultData = _resultResponseError.Match(_ => Results.Content("Test Data"));
    //     var resultDataT = _resultResponseError.Match(_ => TypedResults.Content("Test Data"));

    //     var objRes = Assert.IsAssignableFrom<IResult>(result);
    //     var objResT = Assert.IsType<ProblemHttpResult>(resultT);
    //     var probDetT = Assert.IsType<ProblemDetails>(objResT.ProblemDetails);
    //     Assert.Equal(_problemDetailsDetail, probDetT.Detail);

    //     var objResData = Assert.IsAssignableFrom<IValueHttpResult>(resultData);
    //     var probDet = Assert.IsType<ProblemDetails>(objResData.Value);
    //     Assert.Equal(_problemDetailsDetail, probDet.Detail);

    //     var objResDataT = Assert.IsAssignableFrom<ProblemHttpResult>(resultDataT);
    //     probDet = Assert.IsType<ProblemDetails>(objResDataT.ProblemDetails);
    //     Assert.Equal(_problemDetailsDetail, probDet.Detail);
    // }

    // [Fact]
    // public void MatchT_ResultError_ShouldReturnProblemDetailsOnFailure()
    // {
    //     var result = _resultError.Match(_ => Results.NoContent());
    //     var resultT = _resultError.Match(_ => TypedResults.NoContent());
    //     var resultData = _resultError.Match(_ => Results.Content("Test Data"));
    //     var resultDataT = _resultError.Match(_ => TypedResults.Content("Test Data"));

    //     var objRes = Assert.IsAssignableFrom<IResult>(result);
    //     var objResT = Assert.IsType<ProblemHttpResult>(resultT);
    //     var probDetT = Assert.IsType<ProblemDetails>(objResT.ProblemDetails);
    //     Assert.Equal(_problemDetailsDetail, probDetT.Detail);

    //     var objResData = Assert.IsAssignableFrom<IValueHttpResult>(resultData);
    //     var probDet = Assert.IsType<ProblemDetails>(objResData.Value);
    //     Assert.Equal(_problemDetailsDetail, probDet.Detail);

    //     var objResDataT = Assert.IsAssignableFrom<ProblemHttpResult>(resultDataT);
    //     probDet = Assert.IsType<ProblemDetails>(objResDataT.ProblemDetails);
    //     Assert.Equal(_problemDetailsDetail, probDet.Detail);
    // }
}
