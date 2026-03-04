/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.Configurations;
using ViniBas.ResultPattern.AspNet.ResultMatcher;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMatcher;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class FallbackMatchHelperTests
{
    private readonly ResultResponseError _resultResponseError =
        ResultResponseError.Create([new ErrorDetails("Code", "Test Error")], ErrorTypes.Validation);

    public FallbackMatchHelperTests()
    {
        GlobalConfiguration.UseProblemDetails = true;
    }

    [Fact]
    public void OnSuccessFallback_WhenResultResponseTyped_ReturnsOkObjectResult()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create("Test Value");

        // Act
        var matcherResultMvc = FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        var matcherResultMinimal = FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert Mvc
        var okResultMvc = Assert.IsType<OkObjectResult>(matcherResultMvc);
        Assert.Equal(resultResponse, okResultMvc.Value);

        // Assert Minimal Api
        var okResultMinimal = Assert.IsType<Ok<ResultResponse>>(matcherResultMinimal);
        Assert.Equal(resultResponse, okResultMinimal.Value);
    }

    [Fact]
    public void OnSuccessFallback_WhenResultResponseNotTyped_ReturnsOkResult()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create();

        // Act
        var matcherResultMvc = FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        var matcherResultMinimal = FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert Mvc
        var okResultMvc = Assert.IsType<OkObjectResult>(matcherResultMvc);
        Assert.Equal(resultResponse, okResultMvc.Value);

        // Assert Minimal Api
        var okResultMinimal = Assert.IsType<Ok<ResultResponse>>(matcherResultMinimal);
        Assert.Equal(resultResponse, okResultMinimal.Value);
    }

    [Fact]
    public void OnSuccessFallback_WhenResultResponseIsNotSuccess_ThrowsInvalidOperationException()
    {
        // Arrange
        var resultResponse = ResultResponseError.Create([new ErrorDetails("Code", "Test Error")], ErrorTypes.Validation);

        // Act
        var exceptionMvc = Record.Exception(() => FallbackMvcMatchHelper.OnSuccessFallback(resultResponse));
        var exceptionMinimal = Record.Exception(() => FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse));

        // Assert
        Assert.IsType<InvalidOperationException>(exceptionMvc);
        Assert.IsType<InvalidOperationException>(exceptionMinimal);
    }

    [Fact]
    public void OnFailureFallback_WhenGlobalConfigIsFalse_ReturnsResultResponseError()
    {
        // Arrange
        GlobalConfiguration.UseProblemDetails = false;

        // Act
        var matcherResultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
        var matcherResultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

        // Assert
        var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
        var resultResponseValueMvc = Assert.IsType<ResultResponseError>(objectResultMvc.Value);
        Assert.Equivalent(_resultResponseError.Errors, resultResponseValueMvc.Errors);
        Assert.Equal(GlobalConfiguration.GetStatusCode(_resultResponseError.Type), objectResultMvc.StatusCode);

        var jsonResultMinimal = Assert.IsType<JsonHttpResult<ResultResponseError>>(matcherResultMinimal);
        var resultResponseValueMinimal = Assert.IsType<ResultResponseError>(jsonResultMinimal.Value);
        Assert.Equivalent(_resultResponseError.Errors, resultResponseValueMinimal.Errors);
        Assert.Equal(GlobalConfiguration.GetStatusCode(_resultResponseError.Type), jsonResultMinimal.StatusCode);
    }

    [Fact]
    public void OnFailureFallback_WhenGlobalConfigIsTrue_ReturnsProblemDetails()
    {
        // Arrange
        GlobalConfiguration.UseProblemDetails = true;

        // Act
        var matcherResultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
        var matcherResultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

        // Assert
        var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
        var resultResponseValueMvc = Assert.IsType<ProblemDetails>(objectResultMvc.Value);
        Assert.Equivalent(_resultResponseError.Errors, resultResponseValueMvc.Extensions["errors"]);
        Assert.Equal(GlobalConfiguration.GetStatusCode(_resultResponseError.Type), objectResultMvc.StatusCode);

        var problemResultMinimal = Assert.IsType<ProblemHttpResult>(matcherResultMinimal);
        var resultResponseValueMinimal = Assert.IsType<ProblemDetails>(problemResultMinimal.ProblemDetails);
        Assert.Equivalent(_resultResponseError.Errors, resultResponseValueMinimal.Extensions["errors"]);
        Assert.Equal(GlobalConfiguration.GetStatusCode(_resultResponseError.Type), problemResultMinimal.StatusCode);
    }

    [Fact]
    public void OnFailureFallback_WhenScopedConfigIsFalse_OverridesGlobalTrue()
    {
        // Arrange
        GlobalConfiguration.UseProblemDetails = true;

        using (ScopedConfiguration.Override(useProblemDetails: false))
        {
            // Act
            var matcherResultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
            var matcherResultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

            // Assert
            var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
            Assert.IsType<ResultResponseError>(objectResultMvc.Value);

            Assert.IsType<JsonHttpResult<ResultResponseError>>(matcherResultMinimal);
        }
    }

    [Fact]
    public void OnFailureFallback_WhenScopedConfigIsTrue_OverridesGlobalFalse()
    {
        // Arrange
        GlobalConfiguration.UseProblemDetails = false;

        using (ScopedConfiguration.Override(useProblemDetails: true))
        {
            // Act
            var matcherResultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
            var matcherResultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

            // Assert
            var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
            Assert.IsType<ProblemDetails>(objectResultMvc.Value);

            Assert.IsType<ProblemHttpResult>(matcherResultMinimal);
        }
    }

    [Fact]
    public void OnFailureFallback_WhenScopedConfigDisposed_FallsBackToGlobal()
    {
        // Arrange
        GlobalConfiguration.UseProblemDetails = false;

        using (ScopedConfiguration.Override(useProblemDetails: true)) { }

        // Act — scope already disposed, should use global (false)
        var matcherResultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
        var matcherResultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

        // Assert
        var objectResultMvc = Assert.IsType<ObjectResult>(matcherResultMvc);
        Assert.IsType<ResultResponseError>(objectResultMvc.Value);

        Assert.IsType<JsonHttpResult<ResultResponseError>>(matcherResultMinimal);
    }

    [Fact]
    public void OnFailureFallback_WhenResultResponseIsNotResultResponseError_ThrowsInvalidOperationException()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create();

        // Act
        var exceptionMvc = Record.Exception(() => FallbackMvcMatchHelper.OnFailureFallback(resultResponse));
        var exceptionMinimal = Record.Exception(() => FallbackMinimalMatchHelper.OnFailureFallback(resultResponse));

        // Assert
        Assert.IsType<InvalidOperationException>(exceptionMvc);
        Assert.IsType<InvalidOperationException>(exceptionMinimal);
    }
}
