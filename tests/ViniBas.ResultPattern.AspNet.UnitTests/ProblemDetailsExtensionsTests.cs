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
    private readonly ResultResponseError _resultResponse = new ([ "Error 1", "Error 2" ], ErrorTypes.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Fact]
    public void ToProblemDetails_WithResultResponseError_ShouldReturnProblemDetails()
    {
        var problemDetails = _resultResponse.ToProblemDetails();

        Assert.IsType<ProblemDetails>(problemDetails);
        Assert.Equal(_problemDetailsDetail, problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal(false, problemDetails.Extensions["isSuccess"]);
        Assert.Equal(_resultResponse.Errors, problemDetails.Extensions["errors"]);
    }

    [Fact]
    public void ToProblemDetails_WithNewTypeMapped_ShouldReturnWithAssociatedStatusCode()
    {
        Error.ErrorTypes.AddTypes("NewType");
        ErrorTypeMaps.Maps.Add("NewType", (304, "New Type"));
        
        var resultResponse = new ResultResponseError([ "Error 1", "Error 2" ], "NewType");
        var problemDetails = resultResponse.ToProblemDetails();

        Assert.IsType<ProblemDetails>(problemDetails);
        Assert.Equal("New Type", problemDetails.Title);
        Assert.Equal(304, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_WithNewTypeNotMapped_ShouldReturnWithDefaultStatusCode()
    {
        Error.ErrorTypes.AddTypes("NewType");
        
        var resultResponse = new ResultResponseError([ "Error 1", "Error 2" ], "NewType");
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
