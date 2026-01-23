/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class FakeHelpers
{
    #region AspNet Results
    public IActionResult ActionResultSuccess { get; set; } = new ObjectResult("Success");
    public IActionResult ActionResultError { get; set; } = new ObjectResult("Error");

    public IResult IResultSuccess {get; set; } = Results.Ok("Success");
    public IResult IResultError {get; set; } = Results.BadRequest("Error");
    
    #endregion

    #region ViniBas Results
    public Result ResultSuccess { get; set; } = Result.Success();
    public Result ResultError { get; set; } = Result.Failure(Error.Conflict("Code", "An error occurred."));
    public Result<string> ResultStrSuccess { get; set; } = Result.Success("Data");
    public Result<string> ResultStrError { get; set; } = Result<string>.Failure(Error.Conflict("Code", "An error occurred."));
    #endregion

    #region ViniBas ResultResponses
    public ResultResponseSuccess ResultResponseSuccess { get; set; } = ResultResponseSuccess.Create();
    public ResultResponseSuccess<string> ResultResponseSuccessStr { get; set; } = ResultResponseSuccess.Create("Data");
    public ResultResponseError ResultResponseError { get; set; } = ResultResponseError.Create(
        [ new ("Code", "An error occurred.") ], ErrorTypes.Conflict);
    #endregion
}
