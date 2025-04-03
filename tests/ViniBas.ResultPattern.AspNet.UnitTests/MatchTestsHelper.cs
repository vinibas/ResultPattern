/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public static class MatchTestsHelper
{
    public record ResultArranges(
        Result Result,
        Result<string> ResultT,
        ResultResponse ResultResponse,
        ResultResponse ResultResponseT);

    public static TheoryData<bool, bool, bool?> AllScenariosTestData => new()
    {
        { true, true, null },
        { true, true, true },
        { true, true, false },
        { true, false, null },
        { true, false, true },
        { true, false, false },
        { false, true, null },
        { false, true, true },
        { false, true, false },
        { false, false, null },
        { false, false, true },
        { false, false, false },
    };


    public static ResultArranges CreateResultSuccessArranges()
        => new (
            Result.Success(),
            Result<string>.Success("Test Data"),
            ResultResponseSuccess.Create(),
            ResultResponseSuccess.Create("Test Data"));

    public static ResultArranges CreateResultFailureArranges()
        => new (
            Result.Failure(Error.Failure("1", "Error")),
            Result<string>.Failure(Error.Failure("1", "Error")),
            new ResultResponseError(["Error"], ErrorTypes.Failure),
            new ResultResponseError(["Error"], ErrorTypes.Failure));

}
