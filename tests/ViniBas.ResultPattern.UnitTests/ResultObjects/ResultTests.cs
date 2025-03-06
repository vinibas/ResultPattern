/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.UnitTests.ResultObjects;

public abstract class ResultTestsBase
{
    protected Error _error => _errors.First();
    protected readonly List<Error> _errors = new()
    {
        new("Code1", "Description1", ErrorTypes.Failure),
        new("Code2", "Description2", ErrorTypes.Failure),
    };
}

public class ResultTests : ResultTestsBase
{
    [Fact]
    public void StaticConstructor_Success_ShouldBeSuccess()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void StaticConstructor_Failure_ShouldBeFailure()
    {
        var result = Result.Failure(_error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(_error, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromListOfErrors_ShouldBeFailure()
    {
        Result result = _errors;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(_errors.SelectMany(e => e.Details).ToList(), result.Error.Details);
    }

    [Fact]
    public void ToActionResponse_ShouldReturnSuccessResponse()
    {
        var result = Result.Success();
        var response = result.ToActionResponse();

        Assert.IsType<ResultResponseSuccess>(response);
    }

    [Fact]
    public void ToActionResponse_ShouldReturnErrorResponse()
    {
        var result = Result.Failure(_error);
        var response = result.ToActionResponse();

        Assert.IsType<ResultResponseError>(response);
        var errorResponse = (ResultResponseError)response;
        Assert.Equal(_error.ListDescriptions(), errorResponse.Errors);
        Assert.Equal(_error.Type, errorResponse.Type);
    }
}

public class ResultTTests : ResultTestsBase
{
    [Fact]
    public void Success_ShouldBeSuccess()
    {
        var @string = "TestData";
        var resultStr = Result<string>.Success(@string);
        
        var date = DateTime.Now;
        var resultDt = Result<DateTime>.Success(date);

        Assert.True(resultStr.IsSuccess);
        Assert.False(resultStr.IsFailure);
        Assert.Equal(@string, resultStr.Data);

        Assert.True(resultDt.IsSuccess);
        Assert.False(resultDt.IsFailure);
        Assert.Equal(Error.None, resultDt.Error);
    }

    [Fact]
    public void Failure_ShouldBeFailure()
    {
        var resultStr = Result<string>.Failure(_error);
        var resultDt = Result<DateTime>.Failure(_error);
        
        Assert.False(resultStr.IsSuccess);
        Assert.True(resultStr.IsFailure);
        Assert.Equal(_error, resultStr.Error);
        Assert.Null(resultStr.Data);

        Assert.False(resultDt.IsSuccess);
        Assert.True(resultDt.IsFailure);
        Assert.Equal(_error, resultDt.Error);
        Assert.Equal(resultDt.Data, default);
    }

    [Fact]
    public void ImplicitConversion_FromListOfErrors_ShouldBeFailure()
    {
        Result<object> result = _errors;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(_errors.SelectMany(e => e.Details).ToList(), result.Error.Details);
        Assert.Null(result.Data);
    }

    [Fact]
    public void ImplicitConversion_FromItem_ShouldBeSuccess()
    {
        var @string = "TestData";
        Result<string> resultStr = @string;
        
        var date = DateTime.Now;
        Result<DateTime> resultDt = date;

        Assert.True(resultStr.IsSuccess);
        Assert.False(resultStr.IsFailure);
        Assert.Equal(@string, resultStr.Data);
        Assert.Equal(Error.None, resultStr.Error);

        Assert.True(resultDt.IsSuccess);
        Assert.False(resultDt.IsFailure);
        Assert.Equal(date, resultDt.Data);
        Assert.Equal(Error.None, resultDt.Error);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldBeFailure()
    {
        Result<string> result = _error;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(_error, result.Error);
        Assert.Null(result.Data);
    }

    [Fact]
    public void ToActionResponse_ShouldReturnSuccessResponse()
    {
        var data = "TestData";
        var result = Result<string>.Success(data);
        var response = result.ToActionResponse();

        Assert.IsType<ResultResponseSuccess<string>>(response);
        var successResponse = (ResultResponseSuccess<string>)response;
        Assert.Equal(data, successResponse.Data);
    }

    [Fact]
    public void ToActionResponse_ShouldReturnErrorResponse()
    {
        var result = Result<string>.Failure(_error);
        var response = result.ToActionResponse();

        Assert.IsType<ResultResponseError>(response);
        var errorResponse = (ResultResponseError)response;
        Assert.Equal(_error.ListDescriptions(), errorResponse.Errors);
        Assert.Equal(_error.Type, errorResponse.Type);
    }
}