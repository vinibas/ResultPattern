/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Text.RegularExpressions;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

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
            => Matcher.{MatcherName}{MatcherGenericParameters}(
                result,
                onSuccess is not null ? rr => onSuccess((ResultResponseSuccess{SuccessDataType})rr) : null,
                onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
                null);
    """;

    public record MatchMethodBuilderParameters(
        bool IsAsync,
        string ReturnTypeName,
        IEnumerable<string> ReturnTypeGenericParameters,
        bool IsGenericReturnType,
        string? MethodSufixName,
        bool HasSuccessDataType,
        string ExtendedType,
        IEnumerable<(string GenericTypeName, IEnumerable<string> GenericConstraints)> GenericConstraints,
        bool ShouldMatcherReceiveReturnGenericParameter);

    private MatchMethodBuilderParameters _params;

    public MatchMethodBuilder(MatchMethodBuilderParameters matchMethodBuilderParameters)
        => _params = matchMethodBuilderParameters;

    private string ReturnType
    {
        get
        {
            var returnTypeName = _params.ReturnTypeName.Replace("<>", "")
                + BuildGenericParamsTag(_params.ReturnTypeGenericParameters);
            return _params.IsAsync ? $"Task<{returnTypeName}>" : returnTypeName;
        }
    }

    private string MethodName
    {
        get
        {
            var methodName = $"{MethodBaseName}{_params.MethodSufixName}";
            return _params.IsAsync ? $"{methodName}Async" : methodName;
        }
    }

    private string MethodGenericParameters
    {
        get
        {
            var genericParams = new List<string>();

            if (_params.IsGenericReturnType)
                genericParams.Add(_params.ReturnTypeName);

            if (_params.ReturnTypeGenericParameters.Any())
                genericParams.AddRange(_params.ReturnTypeGenericParameters);

            if (_params.HasSuccessDataType)
                genericParams.Add(DataTypeParameterName);

            return BuildGenericParamsTag(genericParams);
        }
    }

    private string ExtendedType
        => _params.ExtendedType.Contains("<") ?
        $"{GetNameWithoutGenericArity(_params.ExtendedType)}<{DataTypeParameterName}>" :
        _params.ExtendedType;

    private string SuccessDataType => BuildGenericParamsTag(_params.HasSuccessDataType ? [ DataTypeParameterName ] : []);

    private string GenericConstraints
    {
        get
        {
            var constraintLines = new List<string>();

            foreach (var (genericTypeName, genericConstraints) in _params.GenericConstraints)
                constraintLines.Add($"where {genericTypeName} : {string.Join(", ", genericConstraints)}");

            var breakLineWithIndentation = '\n' + new string(' ', 8);
            return string.Join(breakLineWithIndentation, constraintLines);
        }
    }

    private string MatcherName => _params.IsAsync ? "MatchAsync" : "Match";

    private string MatcherGenericParameters => _params.ShouldMatcherReceiveReturnGenericParameter ?
        $"<{_params.ReturnTypeName.Replace("<>", "") + BuildGenericParamsTag(_params.ReturnTypeGenericParameters)}>" :
        string.Empty;


    private static string BuildGenericParamsTag(IEnumerable<string> genericParams)
    {
        if (!genericParams.Any())
            return string.Empty;

        return $"<{string.Join(", ", genericParams)}>";
    }

    public static string GetNameWithoutGenericArity(string name)
    {
        int index = name.IndexOf('<');
        return index == -1 ? name : name.Substring(0, index);
    }

    public string Build()
    {
        var method = MethodTemplate
            .Replace("{ReturnType}", ReturnType)
            .Replace("{MethodName}", MethodName)
            .Replace("{MethodGenericParameters}", MethodGenericParameters)
            .Replace("{ExtendedType}", ExtendedType)
            .Replace("{SuccessDataType}", SuccessDataType)
            .Replace("{GenericConstraints}", GenericConstraints)
            .Replace("{MatcherName}", MatcherName)
            .Replace("{MatcherGenericParameters}", MatcherGenericParameters);

        return RemoveEmptyLines(method);
    }

    private static string RemoveEmptyLines(string text)
        => Regex.Replace(text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
}
