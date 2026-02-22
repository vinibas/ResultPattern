/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Text.RegularExpressions;

namespace ViniBas.ResultPattern.AspNet.Generators.Builders;

public sealed class MatchMethodBuilder
{
    public readonly struct Nothing { }

    private const string MethodBaseName = "Match";
    private const string DataTypeParameterName = "TData";
    private const string MethodTemplate =
    """
        public static {ReturnType} {MethodName}{MethodGenericParameters}(
            this {ExtendedType} result,
            Func<ResultResponseSuccess{SuccessDataType}, {ReturnType}>? onSuccess = null,
            Func<ResultResponseError, {ReturnType}>? onFailure = null)
            {GenericConstraints}
            => Matcher.{MethodName}{MatcherGenericParameters}(
                result,
                onSuccess is not null ? rr => onSuccess((ResultResponseSuccess{SuccessDataType})rr) : null,
                onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
                null);
    """;

    private string _extendedType;
    private bool _hasSuccessDataType;
    private string _returnTypeName;
    private bool _isReturnTypeGeneric;
    private bool _isAsync;
    private string? _genericConstraints;

    private string MethodName => _isAsync ? $"{MethodBaseName}Async" : MethodBaseName;
    private string FinalExtendedType => _extendedType.Contains("<") ?
        $"{GetNameWithoutGenericArity(_extendedType)}<{DataTypeParameterName}>" :
        _extendedType;
    private string FinalReturnType => _isAsync ? $"Task<{_returnTypeName}>" : _returnTypeName;
    private string FinalSuccessDataType => _hasSuccessDataType ? $"<{DataTypeParameterName}>" : string.Empty;
    private string MethodGenericParameters
    {
        get
        {
            var genericParams = new List<string>();

            if (_isReturnTypeGeneric)
                genericParams.Add(_returnTypeName);

            if (_hasSuccessDataType)
                genericParams.Add(DataTypeParameterName);

            if (genericParams.Count == 0)
                return string.Empty;

            return $"<{string.Join(", ", genericParams)}>";
        }
    }
    private string MatcherGenericParameters => _isReturnTypeGeneric ?
        $"<{_returnTypeName}>" : string.Empty;

    public static string GetNameWithoutGenericArity(string name)
    {
        int index = name.IndexOf('<');
        return index == -1 ? name : name.Substring(0, index);
    }

    public MatchMethodBuilder(
        string extendedType,
        string returnTypeName,
        bool hasSuccessDataType,
        bool isAsync,
        bool isReturnTypeGeneric,
        string? genericConstraints = null)
    {
        _extendedType = extendedType;
        _returnTypeName = returnTypeName;
        _hasSuccessDataType = hasSuccessDataType;
        _isAsync = isAsync;
        _isReturnTypeGeneric = isReturnTypeGeneric;
        _genericConstraints = genericConstraints;
    }

    public string Build()
    {
        var method = MethodTemplate
            .Replace("{ReturnType}", FinalReturnType)
            .Replace("{MethodName}", MethodName)
            .Replace("{MethodGenericParameters}", MethodGenericParameters)
            .Replace("{SuccessDataType}", FinalSuccessDataType)
            .Replace("{ExtendedType}", FinalExtendedType)
            .Replace("{GenericConstraints}", _genericConstraints ?? string.Empty)
            .Replace("{MatcherGenericParameters}", MatcherGenericParameters);

        return RemoveEmptyLines(method);
    }

    private static string RemoveEmptyLines(string text)
        => Regex.Replace(text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
}
