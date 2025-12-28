/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

[Collection("No parallelism because of GlobalConfiguration.UseProblemDetails")]
public class FilterMappingsTests
{
    [Fact]
    public void MapToResultResponse_ShouldReturnResultResponse_WhenReicivesSomeResultTypeAndProblemDetailsIsFalse()
    {
        var filterMappings = new FilterMappings();
        GlobalConfiguration.UseProblemDetails = false;

        var successParam = Result.Success("Success");
        var errorParam = Error.NotFound("001", "Not Found Error");
        
        foreach (var originalResult in new object?[]
        {
            successParam,
            successParam.ToResponse(),
        })
        {
            var result = filterMappings.MapToResultResponse(originalResult);

            var resultTyped = Assert.IsType<ResultResponseSuccess<string>>(result);
            Assert.Equal("Success", resultTyped.Data);
        }
        
        foreach (var originalResult in new object?[]
        {
            errorParam,
            (Result)errorParam,
            ((Result)errorParam).ToResponse(),
        })
        {
            var result = filterMappings.MapToResultResponse(originalResult);

            var resultTyped = Assert.IsType<ResultResponseError>(result);
            Assert.Equal("NotFound", resultTyped.Type);
            Assert.Contains("Not Found Error", resultTyped.Errors);
        }
    }

    [Fact]
    public void MapToResultResponse_ShouldReturnResultResponse_WhenReicivesSomeResultTypeAndProblemDetailsIsTrue()
    {
        var filterMappings = new FilterMappings();
        GlobalConfiguration.UseProblemDetails = true;

        var successParam = Result.Success("Success");
        var errorParam = Error.NotFound("001", "Not Found Error");
        
        foreach (var originalResult in new object?[]
        {
            successParam,
            successParam.ToResponse(),
        })
        {
            var result = filterMappings.MapToResultResponse(originalResult);

            var resultTyped = Assert.IsType<ResultResponseSuccess<string>>(result);
            Assert.Equal("Success", resultTyped.Data);
        }
        
        foreach (var originalResult in new object?[]
        {
            errorParam,
            (Result)errorParam,
            ((Result)errorParam).ToResponse(),
        })
        {
            var result = filterMappings.MapToResultResponse(originalResult);

            var resultTyped = Assert.IsType<ProblemDetails>(result);
            Assert.Equal("Not Found", resultTyped.Title);
            Assert.Equal("Not Found Error", resultTyped.Detail);
            var errorsExtensions = Assert.IsType<List<string>>(resultTyped.Extensions["errors"]);
            Assert.Contains("Not Found Error", errorsExtensions);
        }
    }

    [Fact]
    public void MapToResultResponse_ShouldReturnTheSameObject_WhenReceivesUnknownType()
    {
        var filterMappings = new FilterMappings();
        var unknownObject = new { Message = "Unknown" };

        var result = filterMappings.MapToResultResponse(unknownObject);

        Assert.Same(unknownObject, result);
    }
}