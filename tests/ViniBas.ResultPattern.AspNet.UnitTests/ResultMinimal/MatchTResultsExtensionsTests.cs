using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ResultMinimal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        var resultResponse = ResultResponseSuccess.Create();
        var resultResponseT = ResultResponseSuccess.Create("Test Data");
        var result = Result.Success();
        var resultT = Result<string>.Success("Test Data");
        
        var resultMatched = isAResponseType ?
            resultResponse.Match<IResult, IResult>(_ => Results.Created()) :
            result.Match<IResult, IResult>(_ => Results.Created());

        var resultMatchedT = isAResponseType ?
            resultResponse.Match<Results<Created, ProblemHttpResult>, Created>(_ => TypedResults.Created()) :
            result.Match<Results<Created, ProblemHttpResult>, Created>(_ => TypedResults.Created());
            
        var resultTDataMatched = isAResponseType ?
            resultResponseT.Match<IResult, IResult>(rr => Results.Ok(rr)) :
            resultT.Match<IResult, IResult>(rr => Results.Ok(rr));

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MatchT_ResultResponseError_ShouldReturnOnFailure(bool isAResponseType)
    {
        var result = Result.Failure(Error.Failure("1", "Error"));
        var resultT = Result<string>.Failure(Error.Failure("1", "Error"));

        Func<ResultResponse, IResult> onSuccess = response => Results.NoContent();
        Func<ResultResponse, IResult> onSuccessData = response => Results.Content("Test Data");
        Func<ResultResponse, IResult> onFailure = response => Results.BadRequest();

        Func<ResultResponse, NoContent> onSuccessT = response => TypedResults.NoContent();
        Func<ResultResponse, ContentHttpResult> onSuccessDataT = response => TypedResults.Content("Test Data");
        Func<ResultResponse, BadRequest> onFailureT = response => TypedResults.BadRequest();

        var resultMatched = isAResponseType ?
            _resultResponseError.Match<IResult, IResult, IResult>(onSuccess, onFailure) :
            result.Match<IResult, IResult, IResult>(onSuccess, onFailure);
        var resultTMatchedT = isAResponseType ?
            _resultResponseError.Match<Results<NoContent, BadRequest>, NoContent, BadRequest>(onSuccessT, onFailureT) :
            result.Match<Results<NoContent, BadRequest>, NoContent, BadRequest>(onSuccessT, onFailureT);
        var resultDataMatched = isAResponseType ?
            _resultResponseError.Match<IResult, IResult, IResult>(onSuccessData, onFailure) :
            resultT.Match<IResult, IResult, IResult>(onSuccessData, onFailure);
        var resultDataMatchedT = isAResponseType ?
            _resultResponseError.Match<Results<ContentHttpResult, BadRequest>, ContentHttpResult, BadRequest>(onSuccessDataT, onFailureT) :
            resultT.Match<Results<ContentHttpResult, BadRequest>, ContentHttpResult, BadRequest>(onSuccessDataT, onFailureT);

        Assert.IsAssignableFrom<IResult>(resultMatched);
        var objRes = Assert.IsType<Results<NoContent, BadRequest>>(resultTMatchedT);
        Assert.IsType<BadRequest>(objRes.Result);
        
        Assert.IsAssignableFrom<IResult>(resultDataMatched);
        var objResD = Assert.IsType<Results<ContentHttpResult, BadRequest>>(resultDataMatchedT);
        Assert.IsType<BadRequest>(objResD.Result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MatchT_ResultResponseError_ShouldReturnProblemDetailsOnFailure(bool isAResponseType)
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

        foreach (var resultItem in new [] { result, resultT, resultData, resultDataT })
        {
            var objRes = Assert.IsType<ProblemHttpResult>(resultItem);
            var probDet = Assert.IsType<ProblemDetails>(objRes.ProblemDetails);
            Assert.Equal(_problemDetailsDetail, probDet.Detail);
        }
    }
}
