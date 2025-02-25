using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.ResponseResults;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class ResultExtensionsTests
{
    private readonly ResultResponseError _resultResponse = new ([ "Error 1", "Error 2" ], ErrorType.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Fact]
    public void Match_ShouldReturnOnSuccess()
    {
        var result = new ResultResponseSuccess().Match(_ => new CreatedResult());
        var resultData = new ResultResponseSuccess<string>("Test Data").Match(rr => new OkObjectResult(rr));

        Assert.IsType<CreatedResult>(result);

        var okResult = Assert.IsType<OkObjectResult>(resultData);
        var resultResponseValue = Assert.IsType<ResultResponseSuccess<string>>(okResult.Value);
        Assert.True(resultResponseValue.IsSuccess);
        Assert.Equal("Test Data", resultResponseValue.Data);
    }

    [Fact]
    public void Match_ShouldReturnOnFailure()
    {
        Func<ResultResponse, IActionResult> onSuccess = response => new NoContentResult();
        Func<ResultResponse, IActionResult> onSuccessData = response => new ObjectResult("Test Data");
        Func<ResultResponse, IActionResult> onFailure = response => new BadRequestResult();

        var result = _resultResponse.Match(onSuccess, onFailure);
        var resultData = _resultResponse.Match(onSuccessData, onFailure);

        Assert.IsType<BadRequestResult>(result);
        Assert.IsType<BadRequestResult>(resultData);
    }

    [Fact]
    public void Match_ShouldReturnProblemDetailsOnFailure()
    {
        var result = _resultResponse.Match(_ => new NoContentResult());
        var resultData = _resultResponse.Match(_ => new ObjectResult("Test Data"));

        var objRes = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
        
        objRes = Assert.IsType<ObjectResult>(resultData);
        probDet = Assert.IsType<ProblemDetails>(objRes.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }

    [Fact]
    public void ToProblemDetails_ShouldReturnProblemDetails()
    {
        var result = _resultResponse.ToProblemDetails();

        var probDet = Assert.IsType<ProblemDetails>(result);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }

    [Fact]
    public void ToProblemDetailsActionResult_ShouldReturnProblemDetails()
    {
        var result = _resultResponse.ToProblemDetailsActionResult();

        var objectResult = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }

    [Fact]
    public void ToProblemDetails_ModelState_ShouldReturnProblemDetails()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("key", "error");

        var result = modelState.ToProblemDetailsActionResult();

        var objectResult = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("error", probDet.Detail);
    }

    [Fact]
    public void ToProblemDetails_Error_ShouldReturnProblemDetails()
    {
        var error = new Error("code", "description", ErrorType.Validation);

        var result = error.ToProblemDetails();

        var probDet = Assert.IsType<ProblemDetails>(result);
        Assert.Equal("description", probDet.Detail);
    }

    [Fact]
    public void ToProblemDetailsActionResult_Error_ShouldReturnProblemDetails()
    {
        var error = new Error("code", "description", ErrorType.Validation);

        var result = error.ToProblemDetailsActionResult();

        var objectResult =Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("description", probDet.Detail);
    }
}
