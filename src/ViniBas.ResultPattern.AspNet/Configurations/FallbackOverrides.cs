/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.Configurations;

/// <summary>
/// Contextual information passed to custom fallback handlers registered in <see cref="FallbackOverrides"/>.
/// Values reflect the active configuration at the time of the <c>Match</c> call,
/// respecting any <see cref="ScopedConfiguration"/> overrides.
/// </summary>
public sealed record FallbackContext(bool UnwrapSuccessData, bool UseProblemDetails);

/// <summary>
/// Holds optional custom fallback handlers that replace the built-in <c>Match</c> behavior
/// when no <c>onSuccess</c> or <c>onFailure</c> callback is provided by the caller.
/// Configure via <see cref="GlobalConfiguration.FallbackOverrides"/>.
/// </summary>
public sealed class FallbackOverrides
{
    /// <summary>
    /// Custom fallback for MVC (<see cref="IActionResult"/>)-based endpoints.
    /// Receives the <see cref="ResultResponse"/> (check <see cref="ResultResponse.IsSuccess"/> to distinguish success from failure)
    /// and a <see cref="FallbackContext"/> with the current configuration values.
    /// When <see langword="null"/>, the built-in default behavior is used.
    /// </summary>
    public Func<ResultResponse, FallbackContext, IActionResult>? Mvc { get; set; }

    /// <summary>
    /// Custom fallback for Minimal API (<see cref="IResult"/>)-based endpoints, including typed results.
    /// Receives the <see cref="ResultResponse"/> (check <see cref="ResultResponse.IsSuccess"/> to distinguish success from failure)
    /// and a <see cref="FallbackContext"/> with the current configuration values.
    /// When <see langword="null"/>, the built-in default behavior is used.
    /// </summary>
    public Func<ResultResponse, FallbackContext, IResult>? MinimalApi { get; set; }
}
