/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class ProblemDetailsExtensionsTests
{
    private readonly ErrorDetails[] errorDetails =
    [
        new ErrorDetails("Code 1", "Error 1"),
        new ErrorDetails("Code 2", "Error 2")
    ];
    private readonly ResultResponseError _resultResponse;
    
    public ProblemDetailsExtensionsTests()
        => _resultResponse = ResultResponseError.Create(errorDetails, ErrorTypes.Validation);

    [Fact]
    public void ToProblemDetails_WithResultResponseError_ShouldReturnProblemDetails()
    {
        var problemDetails = _resultResponse.ToProblemDetails();

        Assert.IsType<ProblemDetails>(problemDetails);
        var expectedDetails = $"Code 1: Error 1,{Environment.NewLine}Code 2: Error 2";
        Assert.Equal(expectedDetails, problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal(false, problemDetails.Extensions["isSuccess"]);
        Assert.Equal(_resultResponse.Errors, problemDetails.Extensions["errors"]);
    }

    [Fact]
    public void ToProblemDetails_WithNewTypeMapped_ShouldReturnWithAssociatedStatusCode()
    {
        Error.ErrorTypes.AddTypes("NewType");
        GlobalConfiguration.ErrorTypeMaps.TryAdd("NewType", (304, "New Type"));
        
        try
        {
            var resultResponse = ResultResponseError.Create(errorDetails, "NewType");
            var problemDetails = resultResponse.ToProblemDetails();

            Assert.IsType<ProblemDetails>(problemDetails);
            Assert.Equal("New Type", problemDetails.Title);
            Assert.Equal(304, problemDetails.Status);
        }
        finally
        {
            GlobalConfiguration.ErrorTypeMaps.TryRemove("NewType", out _);
        }
    }

    [Fact]
    public void ToProblemDetails_WithNewTypeNotMapped_ShouldReturnWithDefaultStatusCode()
    {
        Error.ErrorTypes.AddTypes("NewType");
        
        var resultResponse = ResultResponseError.Create(errorDetails, "NewType");
        var problemDetails = resultResponse.ToProblemDetails();

        Assert.IsType<ProblemDetails>(problemDetails);
        Assert.Equal("Server Failure", problemDetails.Title);
        Assert.Equal(500, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_WithResultResponseSuccess_ThrowException()
    {
        var resultResponse = ResultResponseSuccess.Create();

        var ex = Assert.Throws<InvalidOperationException>(() => resultResponse.ToProblemDetails());
        Assert.Equal("Unable to convert a valid ResultResponse to a ProblemDetails.", ex.Message);
    }
}
