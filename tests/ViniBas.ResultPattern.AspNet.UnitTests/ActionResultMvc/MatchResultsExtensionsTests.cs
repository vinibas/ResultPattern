using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class MatchResultsExtensionsTests
{
    private readonly Result _resultError = Result.Failure(new Error([ new ("err1", "Error 1"), new ("err2", "Error 2") ], ErrorTypes.Validation));
    private readonly ResultResponseError _resultResponseError = new ([ "Error 1", "Error 2" ], ErrorTypes.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_ResultResponseSuccess_ShouldReturnOnSuccess(bool isAResponseType)
    {
        var result = Result.Success();
        var resultT = Result<string>.Success("Test Data");
        var resultResponse = new ResultResponseSuccess();
        var resultResponseT = new ResultResponseSuccess<string>("Test Data");

        var resultMatched = isAResponseType ?
            resultResponse.Match(_ => new CreatedResult()) :
            result.Match(_ => new CreatedResult());
        var resultMatchedT = isAResponseType ?
            resultResponseT.Match(rr => new OkObjectResult(rr)) :
            resultT.Match(rr => new OkObjectResult(rr));

        Assert.IsType<CreatedResult>(result);
        AssertMatchSuccess(resultMatchedT);
    }

    private void AssertMatchSuccess(IActionResult result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_ResultResponseError_ShouldReturnOnFailure(bool isAResponseType)
    {
        var result = Result.Failure(Error.Failure("1", "Error"));
        var resultT = Result<string>.Failure(Error.Failure("1", "Error"));

        Func<ResultResponse, IActionResult> onSuccess = response => new NoContentResult();
        Func<ResultResponse, IActionResult> onSuccessData = response => new ObjectResult("Test Data");
        Func<ResultResponse, IActionResult> onFailure = response => new BadRequestResult();

        var resultMatched = isAResponseType ?
            _resultResponseError.Match(onSuccess, onFailure) :
            result.Match(onSuccess, onFailure);
        var resultMatchedT = isAResponseType ?
            _resultResponseError.Match(onSuccessData, onFailure) :
            resultT.Match(onSuccessData, onFailure);

        Assert.IsType<BadRequestResult>(resultMatched);
        Assert.IsType<BadRequestResult>(resultMatchedT);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Match_ResultResponseError_ShouldReturnProblemDetailsOnFailure(bool isAResponseType)
    {
        var result = isAResponseType ?
            _resultResponseError.Match(_ => new NoContentResult()) :
            _resultError.Match(_ => new NoContentResult());
        var resultT = isAResponseType ?
            _resultResponseError.Match(_ => new ObjectResult("Test Data")) :
            _resultError.Match(_ => new ObjectResult("Test Data"));

        var objRes = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
        
        objRes = Assert.IsType<ObjectResult>(resultT);
        probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }
}
