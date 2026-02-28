/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Text.RegularExpressions;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;

internal abstract class MatchMethodBuilderBase
{
    protected const string MethodBaseName = "Match";
    protected const string DataTypeParameterName = "TData";
    protected readonly MatchMethodBuilderParameters _params;

    internal record MatchMethodBuilderParameters(
        bool IsAsync,
        string ReturnTypeName,
        IEnumerable<string> ReturnTypeGenericParameters,
        bool IsGenericReturnType,
        string? MethodSufixName,
        bool HasSuccessDataType,
        string ExtendedType,
        IEnumerable<(string GenericTypeName, IEnumerable<string> GenericConstraints)> GenericConstraints,
        bool ShouldMatcherReceiveReturnGenericParameter);

    public MatchMethodBuilderBase(MatchMethodBuilderParameters matchMethodBuilderParameters)
        => _params = matchMethodBuilderParameters;

    protected abstract string Template { get; }

    protected string ReturnType
        => _params.IsAsync ?
        $"Task<{ReturnTypeWithGenericParameters}>" :
        ReturnTypeWithGenericParameters;

    protected string ReturnTypeWithGenericParameters
        => _params.ReturnTypeName.Replace("<>", "") + BuildGenericParamsTag(_params.ReturnTypeGenericParameters);

    protected string ExtendedType
        => _params.ExtendedType.Contains("<") ?
        $"{GetNameWithoutGenericArity(_params.ExtendedType)}<{DataTypeParameterName}>" :
        _params.ExtendedType;

    protected static string BuildGenericParamsTag(IEnumerable<string> genericParams)
    {
        if (!genericParams.Any())
            return string.Empty;

        return $"<{string.Join(", ", genericParams)}>";
    }

    private static string GetNameWithoutGenericArity(string name)
    {
        int index = name.IndexOf('<');
        return index == -1 ? name : name.Substring(0, index);
    }

    public abstract string Build();

    protected static string RemoveEmptyLines(string text)
        => Regex.Replace(text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

}
