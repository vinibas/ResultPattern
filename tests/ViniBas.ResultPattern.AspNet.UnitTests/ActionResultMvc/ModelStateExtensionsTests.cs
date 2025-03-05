using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class ModelStateExtensionsTests
{
    private readonly ModelStateDictionary _modelStateValid;
    private readonly ModelStateDictionary _modelStateInvalid;

    public ModelStateExtensionsTests()
    {
        _modelStateValid = new ModelStateDictionary();

        _modelStateInvalid = new ModelStateDictionary();
        _modelStateInvalid.AddModelError("key1", "Error 1");
        _modelStateInvalid.AddModelError("key2", "Error 2");
    }
    
    [Fact]
    public void ModelStateToError_WithErrors_ReturnError()
    {
        var error = _modelStateInvalid.ModelStateToError();
        
        var errorDetails = error.Details.ToArray();
        Assert.False(_modelStateInvalid.IsValid);
        Assert.Equal(ErrorTypes.Validation, error.Type);
        Assert.Equal(2, errorDetails.Length);
        Assert.Equal("key1", errorDetails[0].Code);
        Assert.Equal("Error 1", errorDetails[0].Description);
        Assert.Equal("key2", errorDetails[1].Code);
        Assert.Equal("Error 2", errorDetails[1].Description);
    }

    [Fact]
    public void ModelStateToError_WithoutErrors_ThrowException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _modelStateValid.ModelStateToError());
        Assert.Equal("Unable to convert a valid ModelState to a ProblemDetails.", ex.Message);
    }

    [Fact]
    public void ToProblemDetailsActionResult_WithErrors_ReturnActionResultError()
    {
        var error = _modelStateInvalid.ToProblemDetailsActionResult();
        
        var objectResult = Assert.IsType<ObjectResult>(error);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);

        Assert.False(_modelStateInvalid.IsValid);
        Assert.Equal(400, objectResult.StatusCode);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal("Bad Request", problemDetails.Title);
        Assert.Equal(string.Join(Environment.NewLine, ["Error 1", "Error 2"]), problemDetails.Detail);
    }

    [Fact]
    public void ToProblemDetailsActionResult_WithoutErrors_ThrowException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _modelStateValid.ToProblemDetailsActionResult());
        Assert.Equal("Unable to convert a valid ModelState to a ProblemDetails.", ex.Message);
    }

}
