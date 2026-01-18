/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.UnitTests.ResultResponses;

public class ResultResponseErrorTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var errors = new List<ErrorDetails> { new ("Code 1", "Error 1"), new ("Code 2", "Error 2") };
        var errorType = ErrorTypes.Validation;

        var resultResponseError = ResultResponseError.Create(errors, errorType);

        Assert.False(resultResponseError.IsSuccess);
        Assert.Equal(errors, resultResponseError.Errors);
        Assert.Equal(errorType, resultResponseError.Type);
    }
}
