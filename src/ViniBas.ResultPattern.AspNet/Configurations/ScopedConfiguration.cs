/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.AspNet.Configurations;

/// <summary>
/// Allows per-async-scope overrides of <see cref="GlobalConfiguration"/> settings.
/// Uses <see cref="AsyncLocal{T}"/> so scopes are isolated per async context and support nesting.
/// </summary>
public static class ScopedConfiguration
{
    private static readonly AsyncLocal<ScopedConfigurationValues?> _current = new();

    internal static ScopedConfigurationValues? Current => _current.Value;

    /// <summary>
    /// Starts a configuration scope that overrides one or more <see cref="GlobalConfiguration"/> settings
    /// for the current async context. Dispose the returned <see cref="IDisposable"/> to restore the previous values.
    /// </summary>
    /// <param name="useProblemDetails">
    /// Overrides <see cref="GlobalConfiguration.UseProblemDetails"/> for this scope.
    /// Pass <see langword="null"/> to leave the global value in effect.
    /// </param>
    /// <param name="unwrapSuccessData">
    /// Overrides <see cref="GlobalConfiguration.UnwrapSuccessData"/> for this scope.
    /// Pass <see langword="null"/> to leave the global value in effect.
    /// </param>
    /// <returns>An <see cref="IDisposable"/> that restores the previous configuration on disposal.</returns>
    public static IDisposable Override(bool? useProblemDetails = null, bool? unwrapSuccessData = null)
    {
        var previous = _current.Value;
        _current.Value = new ScopedConfigurationValues(useProblemDetails, unwrapSuccessData);
        return new Scope(previous);
    }

    private sealed class Scope(ScopedConfigurationValues? previous) : IDisposable
    {
        public void Dispose() => _current.Value = previous;
    }
}

internal sealed class ScopedConfigurationValues(bool? useProblemDetails, bool? unwrapSuccessData = null)
{
    public bool? UseProblemDetails { get; } = useProblemDetails;
    public bool? UnwrapSuccessData { get; } = unwrapSuccessData;
}
