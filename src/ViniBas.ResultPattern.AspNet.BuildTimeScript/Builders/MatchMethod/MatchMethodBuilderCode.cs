/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;

internal sealed class MatchMethodBuilderCode : MatchMethodBuilderBase
{
    public MatchMethodBuilderCode(MatchMethodBuilderParameters matchMethodBuilderParameters)
        : base(matchMethodBuilderParameters) { }

    protected override string Template =>
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
        $"<{ReturnTypeWithGenericParameters}>" :
        string.Empty;

    public override string Build()
    {
        var method = Template
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
}
