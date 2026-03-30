/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher;

internal static class TypeCastHelper
{
    private static readonly ConcurrentDictionary<TypePair, MethodInfo?> _implicitOperatorCache = new();

    public static TResult TreatCast<TResult>(IResult iresult)
        where TResult : IResult
    {
        // First check the type, to try to avoid Reflection, for performance reasons
        if (iresult is TResult tresult)
            return tresult;

        var sourceType = iresult.GetType();
        var targetType = typeof(TResult);

        var implicitOperator = GetImplicitOperator(targetType, sourceType);
        if (implicitOperator != null)
            return (TResult)implicitOperator.Invoke(null, new object[] { iresult })!;

        try
        {
            // Attempt to cast to classes with explicit conversion operators
            return (TResult)iresult;
        }
        catch (InvalidCastException)
        {
            throw new InvalidOperationException(
                $"The type provided for T_Result ({GetFriendlyName(typeof(TResult))}) is not compatible " +
                $"with the result ({GetFriendlyName(iresult.GetType())}). " + Environment.NewLine +
                "T_Result must be a type that can accept the result or a compatible interface.");
        }
    }

    private static MethodInfo? GetImplicitOperator(Type targetType, Type sourceType)
    {
        var key = new TypePair(targetType, sourceType);
        return _implicitOperatorCache.GetOrAdd(key, _ => FindImplicitOperator(targetType, sourceType));
    }

    private static MethodInfo? FindImplicitOperator(Type targetType, Type sourceType)
    {
        var operatorInTarget = targetType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "op_Implicit" &&
                m.ReturnType == targetType &&
                m.GetParameters() is { Length: 1 } parameters &&
                parameters[0].ParameterType.IsAssignableFrom(sourceType));

        if (operatorInTarget != null)
            return operatorInTarget;

        return sourceType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "op_Implicit" &&
                targetType.IsAssignableFrom(m.ReturnType) &&
                m.GetParameters() is { Length: 1 } parameters &&
                parameters[0].ParameterType == sourceType);
    }

    private static string GetFriendlyName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var name = type.Name[..type.Name.IndexOf('`')];
        var args = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName));
        return $"{name}<{args}>";
    }

    private readonly record struct TypePair(Type Target, Type Source);
}
