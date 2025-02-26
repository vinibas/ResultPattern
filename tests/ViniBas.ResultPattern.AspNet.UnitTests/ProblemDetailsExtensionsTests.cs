using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.ResponseResults;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class ProblemDetailsExtensionsTests
{
    private readonly ResultResponseError _resultResponse = new ([ "Error 1", "Error 2" ], ErrorType.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

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
