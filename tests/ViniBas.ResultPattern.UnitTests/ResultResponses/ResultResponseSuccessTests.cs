/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.UnitTests.ResultResponses;

public class ResultResponseSuccessTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var resultResponseSuccess = ResultResponseSuccess.Create();

        Assert.True(resultResponseSuccess.IsSuccess);
    }

    [Fact]
    public void Constructor_Generic_ShouldInitializeCorrectly()
    {
        var @string = "Test Data";
        var date = DateTime.Now;

        var resultResponseSuccessStr = ResultResponseSuccess.Create(@string);
        var resultResponseSuccessDt = ResultResponseSuccess.Create(date);

        Assert.True(resultResponseSuccessStr.IsSuccess);
        Assert.Equal(@string, resultResponseSuccessStr.Data);

        Assert.True(resultResponseSuccessDt.IsSuccess);
        Assert.Equal(date, resultResponseSuccessDt.Data);
    }

    [Fact]
    public void StaticConstructor_Create_ShouldReturnInstance()
    {
        var resultResponseSuccess = ResultResponseSuccess.Create();

        Assert.True(resultResponseSuccess.IsSuccess);
    }

    [Fact]
    public void StaticConstructor_Generic_Create_ShouldReturnInstanceWithData()
    {
        var data = "Test Data";

        var resultResponseSuccess = ResultResponseSuccess.Create(data);

        Assert.True(resultResponseSuccess.IsSuccess);
        Assert.Equal(data, resultResponseSuccess.Data);
    }
}
