/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.Configurations;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class FilterMappingsTests
{
    [Fact]
    public void MapToResultResponse_ShouldReturnSuccessResult_WhenReceivesResultBase()
    {
        var mvcFilter = new FilterMvcMappings();
        var minimalFilter = new FilterMinimalApiMappings();

        var successParam = Result.Success("Success");

        var mvcResult = mvcFilter.MapToResultResponse(successParam);
        var mvcObjectResult = Assert.IsType<OkObjectResult>(mvcResult);
        var mvcTyped = Assert.IsType<ResultResponseSuccess<string>>(mvcObjectResult.Value);
        Assert.Equal("Success", mvcTyped.Data);

        var minimalResult = minimalFilter.MapToResultResponse(successParam);
        var minimalOk = Assert.IsType<Ok<ResultResponseSuccess>>(minimalResult);
        var minimalTyped = Assert.IsType<ResultResponseSuccess<string>>(minimalOk.Value);
        Assert.Equal("Success", minimalTyped.Data);
    }

    [Fact]
    public void MapToResultResponse_ShouldReturnErrorResult_WhenReceivesErrorTypeAndProblemDetailsIsFalse()
    {
        GlobalConfiguration.UseProblemDetails = false;

        var mvcFilter = new FilterMvcMappings();
        var minimalFilter = new FilterMinimalApiMappings();

        var errorParam = Error.NotFound("001", "Not Found Error");

        foreach (var originalResult in new object?[]
        {
            errorParam,
            (Result)errorParam,
        })
        {
            var mvcResult = mvcFilter.MapToResultResponse(originalResult);
            var mvcObjectResult = Assert.IsType<ObjectResult>(mvcResult);
            Assert.Equal(StatusCodes.Status404NotFound, mvcObjectResult.StatusCode);
            var mvcTyped = Assert.IsType<ResultResponseError>(mvcObjectResult.Value);
            Assert.Equal("NotFound", mvcTyped.Type);
            Assert.Contains("001: Not Found Error", mvcTyped.Errors.Select(e => e.ToString()).Single());

            var minimalResult = minimalFilter.MapToResultResponse(originalResult);
            var minimalTyped = Assert.IsType<NotFound<ResultResponseError>>(minimalResult);
            Assert.Equal("NotFound", minimalTyped.Value?.Type);
            Assert.Contains("001: Not Found Error", minimalTyped.Value!.Errors.Select(e => e.ToString()).Single());
        }

        GlobalConfiguration.UseProblemDetails = true;
    }

    [Fact]
    public void MapToResultResponse_ShouldReturnProblemDetails_WhenReceivesErrorTypeAndProblemDetailsIsTrue()
    {
        GlobalConfiguration.UseProblemDetails = true;

        var mvcFilter = new FilterMvcMappings();
        var minimalFilter = new FilterMinimalApiMappings();

        var errorParam = Error.NotFound("001", "Not Found Error");

        foreach (var originalResult in new object?[]
        {
            errorParam,
            (Result)errorParam,
        })
        {
            var mvcResult = mvcFilter.MapToResultResponse(originalResult);
            var mvcObjectResult = Assert.IsType<ObjectResult>(mvcResult);
            Assert.Equal(StatusCodes.Status404NotFound, mvcObjectResult.StatusCode);
            var mvcProblem = Assert.IsType<ProblemDetails>(mvcObjectResult.Value);
            Assert.Equal("Not Found", mvcProblem.Title);
            Assert.Equal("001: Not Found Error", mvcProblem.Detail);
            var mvcErrors = Assert.IsType<ErrorDetails[]>(mvcProblem.Extensions["errors"]);
            Assert.Contains("001: Not Found Error", mvcErrors.Select(e => e.ToString()));

            var minimalResult = minimalFilter.MapToResultResponse(originalResult);
            var minimalTypedProblem = Assert.IsType<NotFound<ProblemDetails>>(minimalResult);
            Assert.Equal("Not Found", minimalTypedProblem.Value?.Title);
            Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.5", minimalTypedProblem.Value?.Type);
            Assert.Equal("001: Not Found Error", minimalTypedProblem.Value?.Detail);
            var minimalTypedErrors = Assert.IsType<ErrorDetails[]>(minimalTypedProblem.Value?.Extensions["errors"]);
            Assert.Contains("001: Not Found Error", minimalTypedErrors.Select(e => e.ToString()));
        }
    }

    [Fact]
    public void MapToResultResponse_ShouldReturnTheSameObject_WhenReceivesUnknownType()
    {
        var mvcFilter = new FilterMvcMappings();
        var minimalFilter = new FilterMinimalApiMappings();
        var unknownObject = new { Message = "Unknown" };

        Assert.Same(unknownObject, mvcFilter.MapToResultResponse(unknownObject));
        Assert.Same(unknownObject, minimalFilter.MapToResultResponse(unknownObject));
    }

    [Fact]
    public void MapToResultResponse_ShouldReturnTheSameObject_WhenReceivesResultResponse()
    {
        var mvcFilter = new FilterMvcMappings();
        var minimalFilter = new FilterMinimalApiMappings();

        var successResponse = Result.Success("Success").ToResponse();
        var errorResponse = ((Result)Error.NotFound("001", "Error")).ToResponse();

        Assert.Same(successResponse, mvcFilter.MapToResultResponse(successResponse));
        Assert.Same(successResponse, minimalFilter.MapToResultResponse(successResponse));
        Assert.Same(errorResponse, mvcFilter.MapToResultResponse(errorResponse));
        Assert.Same(errorResponse, minimalFilter.MapToResultResponse(errorResponse));
    }
}
