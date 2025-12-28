/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.ResultMinimalApi;

public static class ResultToProblemDetailsExtensions
{
    public static ProblemHttpResult ToProblemDetailsResult(this ResultResponse resultResponse)
        => ProblemDetailsToResult(resultResponse.ToProblemDetails());

    public static ProblemHttpResult ToProblemDetailsResult(this Error error)
        => ProblemDetailsToResult(error.ToProblemDetails());

    private static ProblemHttpResult ProblemDetailsToResult(ProblemDetails problemDetails)
        => TypedResults.Problem(problemDetails);
}