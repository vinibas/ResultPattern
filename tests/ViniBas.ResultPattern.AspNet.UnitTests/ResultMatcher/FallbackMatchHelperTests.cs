/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
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
        GlobalConfiguration.UnwrapSuccessData = false;
        GlobalConfiguration.FallbackOverrides.Mvc = null;
        GlobalConfiguration.FallbackOverrides.MinimalApi = null;
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

    [Fact]
    public void OnSuccessFallback_WhenFallbackOverrideIsSet_CallsOverrideWithContext()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create("Test Value");
        FallbackContext? capturedMvcContext = null;
        FallbackContext? capturedMinimalContext = null;

        var expectedMvcResult = new OkResult();
        var expectedMinimalResult = TypedResults.NoContent();

        GlobalConfiguration.FallbackOverrides.Mvc = (response, ctx) =>
        {
            capturedMvcContext = ctx;
            return expectedMvcResult;
        };
        GlobalConfiguration.FallbackOverrides.MinimalApi = (response, ctx) =>
        {
            capturedMinimalContext = ctx;
            return expectedMinimalResult;
        };

        // Act
        var resultMvc = FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        var resultMinimal = FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert
        Assert.Same(expectedMvcResult, resultMvc);
        Assert.NotNull(capturedMvcContext);
        Assert.False(capturedMvcContext.UnwrapSuccessData);
        Assert.True(capturedMvcContext.UseProblemDetails);

        Assert.Same(expectedMinimalResult, resultMinimal);
        Assert.NotNull(capturedMinimalContext);
        Assert.False(capturedMinimalContext.UnwrapSuccessData);
        Assert.True(capturedMinimalContext.UseProblemDetails);
    }

    [Fact]
    public void OnFailureFallback_WhenFallbackOverrideIsSet_CallsOverride()
    {
        // Arrange
        var expectedMvcResult = new BadRequestResult();
        var expectedMinimalResult = TypedResults.BadRequest();

        GlobalConfiguration.FallbackOverrides.Mvc = (response, ctx) => expectedMvcResult;
        GlobalConfiguration.FallbackOverrides.MinimalApi = (response, ctx) => expectedMinimalResult;

        // Act
        var resultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
        var resultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

        // Assert
        Assert.Same(expectedMvcResult, resultMvc);
        Assert.Same(expectedMinimalResult, resultMinimal);
    }

    [Fact]
    public void OnSuccessFallback_WhenFallbackOverrideIsSet_PassesUnwrapSuccessDataFromGlobal()
    {
        // Arrange
        GlobalConfiguration.UnwrapSuccessData = true;
        var resultResponse = ResultResponseSuccess.Create("Test");
        FallbackContext? capturedMvcContext = null;
        FallbackContext? capturedMinimalContext = null;

        GlobalConfiguration.FallbackOverrides.Mvc = (response, ctx) =>
        {
            capturedMvcContext = ctx;
            return new OkResult();
        };
        GlobalConfiguration.FallbackOverrides.MinimalApi = (response, ctx) =>
        {
            capturedMinimalContext = ctx;
            return TypedResults.Ok(response);
        };

        // Act
        FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert
        Assert.NotNull(capturedMvcContext);
        Assert.True(capturedMvcContext.UnwrapSuccessData);
        Assert.NotNull(capturedMinimalContext);
        Assert.True(capturedMinimalContext.UnwrapSuccessData);
    }

    [Fact]
    public void OnSuccessFallback_WhenFallbackOverrideIsSet_PassesUnwrapSuccessDataFromScoped()
    {
        // Arrange
        GlobalConfiguration.UnwrapSuccessData = false;
        var resultResponse = ResultResponseSuccess.Create("Test");
        FallbackContext? capturedMvcContext = null;
        FallbackContext? capturedMinimalContext = null;

        GlobalConfiguration.FallbackOverrides.Mvc = (response, ctx) =>
        {
            capturedMvcContext = ctx;
            return new OkResult();
        };
        GlobalConfiguration.FallbackOverrides.MinimalApi = (response, ctx) =>
        {
            capturedMinimalContext = ctx;
            return TypedResults.Ok(response);
        };

        using (ScopedConfiguration.Override(unwrapSuccessData: true))
        {
            // Act
            FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
            FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);
        }

        // Assert
        Assert.NotNull(capturedMvcContext);
        Assert.True(capturedMvcContext.UnwrapSuccessData);
        Assert.NotNull(capturedMinimalContext);
        Assert.True(capturedMinimalContext.UnwrapSuccessData);
    }

    [Fact]
    public void OnSuccessFallback_WhenUnwrapSuccessDataIsFalse_ReturnsWrappedResultResponse()
    {
        // Arrange
        var resultResponse = ResultResponseSuccess.Create("Test Value");

        // Act
        var resultMvc = FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        var resultMinimal = FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert
        var okResultMvc = Assert.IsType<OkObjectResult>(resultMvc);
        Assert.Equal(resultResponse, okResultMvc.Value);

        var okResultMinimal = Assert.IsType<Ok<ResultResponse>>(resultMinimal);
        Assert.Equal(resultResponse, okResultMinimal.Value);
    }

    [Fact]
    public void OnSuccessFallback_WhenUnwrapSuccessDataIsTrueAndHasData_ReturnsDataDirectly()
    {
        // Arrange
        GlobalConfiguration.UnwrapSuccessData = true;
        var data = "Test Value";
        var resultResponse = ResultResponseSuccess.Create(data);

        // Act
        var resultMvc = FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        var resultMinimal = FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert
        var okResultMvc = Assert.IsType<OkObjectResult>(resultMvc);
        Assert.Equal(data, okResultMvc.Value);

        var okResultMinimal = Assert.IsType<Ok<object>>(resultMinimal);
        Assert.Equal(data, okResultMinimal.Value);
    }

    [Fact]
    public void OnSuccessFallback_WhenUnwrapSuccessDataIsTrueAndNoData_ReturnsOkEmpty()
    {
        // Arrange
        GlobalConfiguration.UnwrapSuccessData = true;
        var resultResponse = ResultResponseSuccess.Create();

        // Act
        var resultMvc = FallbackMvcMatchHelper.OnSuccessFallback(resultResponse);
        var resultMinimal = FallbackMinimalMatchHelper.OnSuccessFallback(resultResponse);

        // Assert
        Assert.IsType<OkResult>(resultMvc);
        Assert.IsType<Ok>(resultMinimal);
    }

    [Fact]
    public void OnFailureFallback_WhenUnwrapSuccessDataIsTrueAndNotProblemDetails_ReturnsErrorsCollection()
    {
        // Arrange
        GlobalConfiguration.UseProblemDetails = false;
        GlobalConfiguration.UnwrapSuccessData = true;

        // Act
        var resultMvc = FallbackMvcMatchHelper.OnFailureFallback(_resultResponseError);
        var resultMinimal = FallbackMinimalMatchHelper.OnFailureFallback(_resultResponseError);

        // Assert
        var objectResultMvc = Assert.IsType<ObjectResult>(resultMvc);
        Assert.Equivalent(_resultResponseError.Errors, objectResultMvc.Value);
        Assert.Equal(GlobalConfiguration.GetStatusCode(_resultResponseError.Type), objectResultMvc.StatusCode);

        var jsonResultMinimal = Assert.IsType<JsonHttpResult<IEnumerable<ErrorDetails>>>(resultMinimal);
        Assert.Equivalent(_resultResponseError.Errors, jsonResultMinimal.Value);
        Assert.Equal(GlobalConfiguration.GetStatusCode(_resultResponseError.Type), jsonResultMinimal.StatusCode);
    }
}
