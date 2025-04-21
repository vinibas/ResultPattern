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

    public static TheoryData<bool, bool?> AllScenariosTestData => new()
    {
        { true, null },
        { true, true },
        { true, false },
        { false, null },
        { false, true },
        { false, false },
    };

    public const string SuccessValue = "Success";
    public const string ErrorCode = "Erro Code";
    public const string ErrorDescription = "Error Description";

    public static Result ResultSuccess = Result.Success();
    public static Result<string> ResultTSuccess = Result.Success(SuccessValue);
    public static Result ResultFailure = Result.Failure(Error.Conflict(ErrorCode, ErrorDescription));
    public static Result<string> ResultTFailure = Result<string>.Failure(Error.Conflict(ErrorCode, ErrorDescription));
}
