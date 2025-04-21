/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

public class ResultsToTestDataBuilder
{
    public List<ResultErrorToTest> _resultErrorsToTest { get; }
    public List<ResultSuccessToTest> _resultSuccessesToTest { get; }

    public ResultsToTestDataBuilder()
    {
        var error1 = new Error("code1", "description1", ErrorTypes.Validation);
        var error2 = new Error("code2", "description2", ErrorTypes.Validation);
        var error3 = new Error("code3", "description3", ErrorTypes.NotFound);

        _resultErrorsToTest =
        [
            new (error1, 400, ["description1"], ErrorTypes.Validation),
            new (new List<Error>() { error1, error2 }, 400, ["description1", "description2"], ErrorTypes.Validation),
            new (Result.Failure(error3), 404, ["description3"], ErrorTypes.NotFound),
            new (new ResultResponseError([ "error 1", "error 2" ], ErrorTypes.Conflict), 409, ["error 1", "error 2"], ErrorTypes.Conflict),
        ];

        var successData = new object();

        _resultSuccessesToTest =
        [
            new (Result.Success()),
            new (Result<object>.Success(successData), successData),
            new (ResultResponseSuccess.Create()),
            new (ResultResponseSuccess<object>.Create(successData), successData),
        ];

    }
}

public abstract class ResultToTestBase
{
    public object ResultToTest { get; set; }

    public ResultToTestBase(object resultToTest) => ResultToTest = resultToTest;
    
    public ObjectResult ResultToTestAsObjectResult()
        => new (ResultToTest);
}


public class ResultErrorToTest : ResultToTestBase
{
    public int ExpectedStatusCode { get; set; }
    public string[] ExpectedMessages { get; set; }
    public string ExpectedType { get; set; }

    public ResultErrorToTest(object resultToTest, int expectedStatusCode, string[] expectedMessages, string expectedType)
        :base(resultToTest)
    {
        ExpectedStatusCode = expectedStatusCode;
        ExpectedMessages = expectedMessages;
        ExpectedType = expectedType;
    }
}

public class ResultSuccessToTest : ResultToTestBase
{
    public object? Data { get; set; }

    public ResultSuccessToTest(object resultToTest, object? data = null)
        :base(resultToTest)
    {
        Data = data;
    }
}
