/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.AspNet.Configurations;

public static class ScopedConfiguration
{
    private static readonly AsyncLocal<ScopedConfigurationValues?> _current = new();

    internal static ScopedConfigurationValues? Current => _current.Value;

    public static IDisposable Override(bool? useProblemDetails = null)
    {
        var previous = _current.Value;
        _current.Value = new ScopedConfigurationValues(useProblemDetails);
        return new Scope(previous);
    }

    private sealed class Scope(ScopedConfigurationValues? previous) : IDisposable
    {
        public void Dispose() => _current.Value = previous;
    }
}

internal sealed class ScopedConfigurationValues(bool? useProblemDetails)
{
    public bool? UseProblemDetails { get; } = useProblemDetails;
}
