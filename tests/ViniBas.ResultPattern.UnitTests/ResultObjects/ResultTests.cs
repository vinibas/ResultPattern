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
        var date = DateTime.Now;

        var result = Result.Success();
        var resultStr = Result<string>.Success("TestData");
        var resultDt = Result<DateTime>.Success(date);

        foreach (var resultItem in new ResultBase[]
        {
            result,
            resultStr,
            resultDt,
        })
        {
            Assert.True(resultItem.IsSuccess);
            Assert.False(resultItem.IsFailure);
            Assert.Equal(Error.None, resultItem.Error);
        }

        Assert.Equal("TestData", resultStr.Data);
        Assert.Equal(date, resultDt.Data);
    }

    [Fact]
    public void StaticConstructor_Failure_ShouldBeFailure()
    {
        var result = Result.Failure(_error);
        var resultStr = Result<string>.Failure(_error);
        var resultDt = Result<DateTime>.Failure(_error);

        foreach (var resultItem in new ResultBase[]
        {
            result,
            resultStr,
            resultDt,
        })
        {
            Assert.False(resultItem.IsSuccess);
            Assert.True(resultItem.IsFailure);
            Assert.Equal(_error, resultItem.Error);
        }

        Assert.Null(resultStr.Data);
        Assert.Equal(resultDt.Data, default);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldBeFailure()
    {
        Result result = _error;
        Result<string> resultT = _error;

        foreach (var resultItem in new ResultBase[]
        {
            result,
            resultT,
        })
        {
            Assert.False(resultItem.IsSuccess);
            Assert.True(resultItem.IsFailure);
            Assert.Equal(_error, resultItem.Error);
        }
        Assert.Null(resultT.Data);
    }

    [Fact]
    public void ImplicitConversion_FromListOfErrors_ShouldBeFailure()
    {
        Result result = _errors;
        Result<object> resultT = _errors;

        foreach (var resultItem in new ResultBase[]
        {
            result,
            resultT,
        })
        {
            var expectedErrorDetails = _errors.SelectMany(e => e.Details).ToList();
            
            Assert.False(resultItem.IsSuccess);
            Assert.True(resultItem.IsFailure);
            Assert.Equal(expectedErrorDetails, resultItem.Error.Details);
        }

        Assert.Null(resultT.Data);
    }

    [Fact]
    public void ToResponse_ShouldReturnSuccessResponse()
    {
        var result = Result.Success();
        var response = result.ToResponse();

        var data = "TestData";
        var resultT = Result<string>.Success(data);
        var responseT = resultT.ToResponse();

        Assert.IsType<ResultResponseSuccess>(response);

        Assert.IsType<ResultResponseSuccess<string>>(responseT);
        var successResponseT = (ResultResponseSuccess<string>)responseT;
        Assert.Equal(data, successResponseT.Data);
    }

    [Fact]
    public void ToResponse_ShouldReturnErrorResponse()
    {
        var result = Result.Failure(_error);
        var response = result.ToResponse();

        var resultT = Result<string>.Failure(_error);
        var responseT = resultT.ToResponse();

        foreach (var responseItem in new [] { response, responseT })
        {
            Assert.IsType<ResultResponseError>(responseItem);
            var errorResponse = (ResultResponseError)responseItem;
            Assert.Equal(_error.ListDescriptions(), errorResponse.Errors);
            Assert.Equal(_error.Type, errorResponse.Type);
        }
    }

    [Fact]
    public void ImplicitOperatorResultT_FromError_ShouldCreateFailureResult()
    {
        Error error = Error.NotFound("Code1", "Description1");
        
        Result result = error;
        Result<object> resultT = error;

        foreach (var resultItem in new ResultBase[]
        {
            result,
            resultT,
        })
        {
            Assert.False(resultItem.IsSuccess);
            Assert.True(resultItem.IsFailure);
            Assert.Equal(ErrorTypes.NotFound, resultItem.Error.Type);
            Assert.Equal("(Code1): Description1", resultItem.Error.ToString());
        }
    }

    [Fact]
    public void ImplicitOperatorResult_FromListOfErrors_ShouldCreateFailureResult()
    {
        List<Error> errors = [ Error.NotFound("Code1", "Description1"), Error.NotFound("Code2", "Description2") ];
        
        Result result = errors;
        Result<object> resultT = errors;

        foreach (var resultItem in new ResultBase[]
        {
            result,
            resultT,
        })
        {
            var expectedErrorStr = "(Code1): Description1" + Environment.NewLine + "(Code2): Description2";
            
            Assert.False(resultItem.IsSuccess);
            Assert.True(resultItem.IsFailure);
            Assert.Equal(ErrorTypes.NotFound, resultItem.Error.Type);
            Assert.Equal(expectedErrorStr, resultItem.Error.ToString());
        }
    }

    [Fact]
    public void AddError_SameTypeToResultFailure_ShouldAddToPreExistingErrors()
    {
        var preExistingErrors = new List<Error> { Error.Conflict("code1", "description1"), Error.Conflict("code2", "description2") };
        var result = Result.Failure(preExistingErrors);
        var newError = Error.Conflict("code3", "description3");

        result.AddError(newError);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorTypes.Conflict, result.Error.Type);
        var errorExpected = string.Join(Environment.NewLine, new[]
        {
            "(code1): description1",
            "(code2): description2",
            "(code3): description3",
        });
        Assert.Equal(errorExpected, result.Error.ToString());
    }

    [Fact]
    public void AddError_DifferentTypeToResultFailure_ThrowInvalidOperationException()
    {
        var result = Result.Failure(Error.NotFound("code1", "description1"));
        var newError = Error.Validation("code2", "description2");

        var ex = Assert.Throws<InvalidOperationException>(() => result.AddError(newError));
        Assert.Equal("The new error must be of the same type as the existing one.", ex.Message);
    }

    [Fact]
    public void AddError_ToResultSuccess_ShouldConvertSuccessIntoFailure()
    {
        var result = Result.Success();
        var resultT = Result<object>.Success(new object());
        var newError = Error.Validation("code1", "description1");

        result.AddError(newError);
        resultT.AddError(newError);

        foreach (var resultItem in new ResultBase[] { result, resultT })
        {
            Assert.False(resultItem.IsSuccess);
            Assert.True(resultItem.IsFailure);
            Assert.Equal(ErrorTypes.Validation, resultItem.Error.Type);
            Assert.Equal("(code1): description1", resultItem.Error.ToString());
        }
    }

    [Fact]
    public void ImplicitOperatorResultT_FromT_ShouldCreateSuccessResult()
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
}
