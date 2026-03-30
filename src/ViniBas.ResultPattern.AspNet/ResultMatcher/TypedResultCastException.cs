/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher;

/// <summary>
/// Thrown when an <see cref="IResult"/> cannot be cast to the expected
/// <c>TResult</c> type in a typed Match operation.
/// Carries the original <see cref="IResult"/> so that a fallback filter
/// can still return a valid HTTP response instead of a 500 error.
/// </summary>
public sealed class TypedResultCastException : InvalidOperationException
{
    /// <summary>
    /// The <see cref="IResult"/> that could not be cast to the requested type.
    /// </summary>
    public IResult OriginalResult { get; }

    internal TypedResultCastException(IResult originalResult, Type targetType, Type sourceType)
        : base(
            $"The type provided for T_Result ({GetFriendlyName(targetType)}) is not compatible " +
            $"with the result ({GetFriendlyName(sourceType)}). " + Environment.NewLine +
            "T_Result must be a type that can accept the result or a compatible interface.")
    {
        OriginalResult = originalResult;
    }

    private static string GetFriendlyName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var name = type.Name[..type.Name.IndexOf('`')];
        var args = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName));
        return $"{name}<{args}>";
    }
}
