/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMatcher;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class DefaultMatchHelperTests
{
    public DefaultMatchHelperTests()
    {
        GlobalConfiguration.UseProblemDetails = true;
    }
    
    [Fact]
    public void OnSuccessDefault_WhenResultResponseTyped_ReturnsOkObjectResult()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create("Test Value");

        // Act
        var matcherResultMvc = DefaultMvcMatchHelper.OnSuccessDefault(resultResponse);
        var matcherResultMinimal = DefaultMinimalMatchHelper.OnSuccessDefault(resultResponse);

        // Assert Mvc
        var okResultMvc = Assert.IsType<OkObjectResult>(matcherResultMvc);
        Assert.Equal(resultResponse, okResultMvc.Value);

        // Assert Minimal Api
        var okResultMinimal = Assert.IsType<Ok<ResultResponse>>(matcherResultMinimal);
        Assert.Equal(resultResponse, okResultMinimal.Value);
    }
    
    [Fact]
    public void OnSuccessDefault_WhenResultResponseNotTyped_ReturnsOkResult()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create();

        // Act
        var matcherResultMvc = DefaultMvcMatchHelper.OnSuccessDefault(resultResponse);
        var matcherResultMinimal = DefaultMinimalMatchHelper.OnSuccessDefault(resultResponse);

        // Assert Mvc
        var okResultMvc = Assert.IsType<OkObjectResult>(matcherResultMvc);
        Assert.Equal(resultResponse, okResultMvc.Value);

        // Assert Minimal Api
        var okResultMinimal = Assert.IsType<Ok<ResultResponse>>(matcherResultMinimal);
        Assert.Equal(resultResponse, okResultMinimal.Value);
    }

    [Fact]
    public void OnSuccessDefault_WhenResultResponseIsNotSuccess_ThrowsInvalidOperationException()
    {
        // Arrange
        var resultResponse = ResultResponseError.Create(["Test Error"], ErrorTypes.Validation);

        // Act
        var exceptionMvc = Record.Exception(() => DefaultMvcMatchHelper.OnSuccessDefault(resultResponse));
        var exceptionMinimal = Record.Exception(() => DefaultMinimalMatchHelper.OnSuccessDefault(resultResponse));

        // Assert
        Assert.IsType<InvalidOperationException>(exceptionMvc);
        Assert.IsType<InvalidOperationException>(exceptionMinimal);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnFailureDefault_WhenDontUseProblemDetails_ReturnsErrorAsResultResponseError(bool useProblemDetailsGlobally)
    {
        // Arrange
        var resultResponse = ResultResponseError.Create(["Test Error"], ErrorTypes.Validation);

        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobally ? false : true;
        var useProblemDetailsParam = useProblemDetailsGlobally ? (bool?)null : false;

        // Act
        var matcherResultMvc = DefaultMvcMatchHelper.OnFailureDefault(resultResponse, useProblemDetailsParam);
        var matcherResultMinimal = DefaultMinimalMatchHelper.OnFailureDefault(resultResponse, useProblemDetailsParam);

        // Assert
        var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
        var resultResponseValueMvc = Assert.IsType<ResultResponseError>(objectResultMvc.Value);
        Assert.Equivalent(resultResponse.Errors, resultResponseValueMvc.Errors);
        Assert.Equal(GlobalConfiguration.GetStatusCode(resultResponse.Type), objectResultMvc.StatusCode);

        var jsonResultMinimal = Assert.IsType<JsonHttpResult<ResultResponseError>>(matcherResultMinimal);
        var resultResponseValueMinimal = Assert.IsType<ResultResponseError>(jsonResultMinimal.Value);
        Assert.Equivalent(resultResponse.Errors, resultResponseValueMinimal.Errors);
        Assert.Equal(GlobalConfiguration.GetStatusCode(resultResponse.Type), jsonResultMinimal.StatusCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnFailureDefault_WhenUseProblemDetails_ReturnsErrorAsProblemDetails(bool useProblemDetailsGlobally)
    {
        // Arrange
        var resultResponse = ResultResponseError.Create(["Test Error"], ErrorTypes.Validation);

        GlobalConfiguration.UseProblemDetails = useProblemDetailsGlobally ? true : false;
        var useProblemDetailsParam = useProblemDetailsGlobally ? (bool?)null : true;

        // Act
        var matcherResultMvc = DefaultMvcMatchHelper.OnFailureDefault(resultResponse, useProblemDetailsParam);
        var matcherResultMinimal = DefaultMinimalMatchHelper.OnFailureDefault(resultResponse, useProblemDetailsParam);

        // Assert
        var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
        var resultResponseValueMvc = Assert.IsType<ProblemDetails>(objectResultMvc.Value);
        Assert.Equivalent(resultResponse.Errors, resultResponseValueMvc.Extensions["errors"]);
        Assert.Equal(GlobalConfiguration.GetStatusCode(resultResponse.Type), objectResultMvc.StatusCode);

        var problemResultMinimal = Assert.IsType<ProblemHttpResult>(matcherResultMinimal);
        var resultResponseValueMinimal = Assert.IsType<ProblemDetails>(problemResultMinimal.ProblemDetails);
        Assert.Equivalent(resultResponse.Errors, resultResponseValueMinimal.Extensions["errors"]);
        Assert.Equal(GlobalConfiguration.GetStatusCode(resultResponse.Type), problemResultMinimal.StatusCode);
    }
    

    [Fact]
    public void OnFailureDefault_WhenResultResponseIsNotResultResponseError_ThrowsInvalidOperationException()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create();

        // Act
        var exceptionMvc = Record.Exception(() => DefaultMvcMatchHelper.OnFailureDefault(resultResponse, null));
        var exceptionMinimal = Record.Exception(() => DefaultMinimalMatchHelper.OnFailureDefault(resultResponse, null));

        // Assert
        Assert.IsType<InvalidOperationException>(exceptionMvc);
        Assert.IsType<InvalidOperationException>(exceptionMinimal);
    }

}
