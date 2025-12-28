/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ActionResultMvc;

public static class ActionResultToProblemDetailsExtensions
{
    public static IActionResult ToProblemDetailsActionResult(this ResultResponse resultResponse)
        => ProblemDetailsToActionResult(resultResponse.ToProblemDetails());

    public static IActionResult ToProblemDetailsActionResult(this Error error)
        => ProblemDetailsToActionResult(error.ToProblemDetails());

    private static IActionResult ProblemDetailsToActionResult(ProblemDetails problemDetails)
        => new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
        };
}
