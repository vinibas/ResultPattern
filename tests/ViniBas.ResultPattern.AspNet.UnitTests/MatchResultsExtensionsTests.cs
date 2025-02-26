using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class MatchResultsExtensionsTests
{
    private readonly ResultResponseError _resultResponseError = new ([ "Error 1", "Error 2" ], ErrorType.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Fact]
    public void Match_ResultResponseSuccess_ShouldReturnOnSuccess()
    {
        var result = new ResultResponseSuccess().Match(_ => new CreatedResult());
        var resultData = new ResultResponseSuccess<string>("Test Data").Match(rr => new OkObjectResult(rr));

        Assert.IsType<CreatedResult>(result);
        AssertMatchSuccess(resultData);
    }

    [Fact]
    public void Match_ResultSuccess_ShouldReturnOnSuccess()
    {
        var result = Result.Success().Match(_ => new CreatedResult());
        var resultData = Result<string>.Success("Test Data").Match(rr => new OkObjectResult(rr));

        Assert.IsType<CreatedResult>(result);
        AssertMatchSuccess(resultData);
    }

    private void AssertMatchSuccess(IActionResult result)
    {
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    [Fact]
    public void Match_ResultResponseError_ShouldReturnOnFailure()
    {
        Func<ResultResponse, IActionResult> onSuccess = response => new NoContentResult();
        Func<ResultResponse, IActionResult> onSuccessData = response => new ObjectResult("Test Data");
        Func<ResultResponse, IActionResult> onFailure = response => new BadRequestResult();

        var result = _resultResponseError.Match(onSuccess, onFailure);
        var resultData = _resultResponseError.Match(onSuccessData, onFailure);

        Assert.IsType<BadRequestResult>(result);
        Assert.IsType<BadRequestResult>(resultData);
    }

    [Fact]
    public void Match_ResultResponseError_ShouldReturnProblemDetailsOnFailure()
    {
        var result = _resultResponseError.Match(_ => new NoContentResult());
        var resultData = _resultResponseError.Match(_ => new ObjectResult("Test Data"));

        var objRes = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
        
        objRes = Assert.IsType<ObjectResult>(resultData);
        probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }

    [Fact]
    public void Match_ResultError_ShouldReturnOnFailure()
    {
        Func<ResultResponse, IActionResult> onSuccess = response => new NoContentResult();
        Func<ResultResponse, IActionResult> onSuccessData = response => new ObjectResult("Test Data");
        Func<ResultResponse, IActionResult> onFailure = response => new BadRequestResult();

        var result = Result.Failure(Error.Failure("1", "Error")).Match(onSuccess, onFailure);
        var resultData = Result<string>.Failure(Error.Failure("1", "Error")).Match(onSuccessData, onFailure);

        Assert.IsType<BadRequestResult>(result);
        Assert.IsType<BadRequestResult>(resultData);
    }

    [Fact]
    public void Match_ResultError_ShouldReturnProblemDetailsOnFailure()
    {
        var result = _resultResponseError.Match(_ => new NoContentResult());
        var resultData = _resultResponseError.Match(_ => new ObjectResult("Test Data"));

        var objRes = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
        
        objRes = Assert.IsType<ObjectResult>(resultData);
        probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }

}
