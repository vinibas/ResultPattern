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
    private readonly ResultResponseError _resultResponse;
    private readonly string _problemDetailsDetail;

    public ActionResultToProblemDetailsExtensionsTests()
    {
        ErrorDetails[] _errorDetails = [
            new ErrorDetails("code1", "description1"),
            new ErrorDetails("code2", "description2"),
        ];

        _resultResponse = ResultResponseError.Create(_errorDetails, ErrorTypes.Validation);
        _problemDetailsDetail = string.Join("," + Environment.NewLine, _errorDetails.Select(d => d.ToString()));
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
    public void ToProblemDetails_Error_ShouldReturnProblemDetails()
    {
        var error = new Error("code", "description", ErrorTypes.Validation);

        var result = error.ToProblemDetails();

        var probDet = Assert.IsType<ProblemDetails>(result);
        Assert.Equal("code: description", probDet.Detail);
    }

    [Fact]
    public void ToProblemDetailsActionResult_Error_ShouldReturnProblemDetails()
    {
        var error = new Error("code", "description", ErrorTypes.Validation);

        var result = error.ToProblemDetailsActionResult();

        var objectResult =Assert.IsType<ObjectResult>(result);
        var probDet = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("code: description", probDet.Detail);
    }
}
