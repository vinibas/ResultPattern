/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ActionResultMvc;

public class ActionResultToProblemDetailsExtensionsTests
{
    private readonly ResultResponseError _resultResponse = new ([ "Error 1", "Error 2" ], ErrorTypes.Validation);
    private readonly string _problemDetailsDetail = string.Join(Environment.NewLine, [ "Error 1", "Error 2" ]);

    [Fact]
    public void ToProblemDetailsActionResult_ShouldReturnProblemDetails()
    {
        var result = _resultResponse.ToProblemDetailsActionResult();

        var objectResult = Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(_problemDetailsDetail, probDet.Detail);
    }

    [Fact]
    public void ToProblemDetails_Error_ShouldReturnProblemDetails()
    {
        var error = new Error("code", "description", ErrorTypes.Validation);

        var result = error.ToProblemDetails();

        var probDet = Assert.IsType<ProblemDetails>(result);
        Assert.Equal("description", probDet.Detail);
    }

    [Fact]
    public void ToProblemDetailsActionResult_Error_ShouldReturnProblemDetails()
    {
        var error = new Error("code", "description", ErrorTypes.Validation);

        var result = error.ToProblemDetailsActionResult();

        var objectResult =Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("description", probDet.Detail);
    }
}
