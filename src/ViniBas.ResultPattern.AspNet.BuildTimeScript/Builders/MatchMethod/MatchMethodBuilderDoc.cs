/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;

internal sealed class MatchMethodBuilderDoc : MatchMethodBuilderBase
{
    public MatchMethodBuilderDoc(MatchMethodBuilderParameters matchMethodBuilderParameters)
        : base(matchMethodBuilderParameters) { }

    protected override string Template =>
    """
        /// <summary>
        /// {DocSummaryStart} whether a {DocExtendedTypeRef} is a success or failure,
        /// and returns the result of the corresponding function.
        /// </summary>
        {DocTypeparamDocs}
        /// <param name="result">
        /// The {DocExtendedTypeRef} to evaluate.
        /// </param>
        /// <param name="onSuccess">
        /// Function to be executed in case of success.
        /// Receives a <see cref="ResultResponseSuccess{DocSuccessTypeRef}"/> and returns a {DocReturnRef}.
        /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnSuccessDefault().
        /// </param>
        /// <param name="onFailure">
        /// Function to be executed in case of failure.
        /// Receives a <see cref="ResultResponseError"/> and returns a {DocReturnRef}.
        /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnFailureDefault().
        /// </param>
        /// <returns>
        /// Returns the result of the <paramref name="onSuccess"/> function in case of success,
        /// or of <paramref name="onFailure"/> in case of failure.
        /// </returns>
    """;

    private string DocSummaryStart => _params.IsAsync ? "Asynchronously checks" : "Checks";

    private string DocExtendedTypeRef => DocRefTag(ExtendedType, TagType.CodeReference);

    private string DocSuccessTypeRef => _params.HasSuccessDataType ? "{TData}" : string.Empty;

    private string DocTypeparamDocs
    {
        get
        {
            var lines = new List<string>();

            if (_params.IsGenericReturnType)
                lines.Add(string.Format("""
                /// {0}
                /// The type of the expected result. Typically a
                /// {1}, but can be any type that implements
                /// {2} and {3}.
                /// </typeparam>
                """,
                DocRefTag(_params.ReturnTypeName, TagType.GenericParameterDeclaration),
                DocRefTag("Results{T1, T2}", TagType.CodeReference),
                DocRefTag("IResult", TagType.CodeReference),
                DocRefTag("IEndpointMetadataProvider", TagType.CodeReference)));

            foreach (var param in _params.ReturnTypeGenericParameters)
                lines.Add(string.Format("""
                /// {0}
                /// One of the expected result types. Must implement
                /// {1} and {2}.
                /// </typeparam>
                """,
                DocRefTag(param, TagType.GenericParameterDeclaration),
                DocRefTag("IResult", TagType.CodeReference),
                DocRefTag("IEndpointMetadataProvider", TagType.CodeReference)));

            if (_params.HasSuccessDataType)
                lines.Add($"""
                /// {DocRefTag(DataTypeParameterName, TagType.GenericParameterDeclaration)}
                /// Type of the success data value.
                /// </typeparam>
                """);

            if (!lines.Any())
                return string.Empty;


            var breakLineWithIndentation = '\n' + new string(' ', 4);
            return string.Join(breakLineWithIndentation, lines.Select(l => l.Replace(Environment.NewLine, breakLineWithIndentation)));
        }
    }

    private string DocReturnRef => DocRefTag(ReturnType,
        _params.IsAsync || !_params.IsGenericReturnType ? TagType.CodeReference : TagType.SelfGenericParameter);


    enum TagType
    {
        CodeReference,
        SelfParameter,
        SelfGenericParameter,
        GenericParameter,
        ParameterDeclaration,
        GenericParameterDeclaration,
    }

    private string DocRefTag(string typeName, TagType tagType)
        => tagType switch
        {
            TagType.CodeReference => $"<see cref=\"{typeName.Replace('<', '{').Replace('>', '}')}\"/>",
            TagType.SelfParameter => $"<paramref name=\"{typeName}\"/>",
            TagType.SelfGenericParameter => $"<typeparamref name=\"{typeName}\"/>",
            TagType.GenericParameter => $"<typeparamref name=\"{typeName}\"/>",
            TagType.ParameterDeclaration => $"<param name=\"{typeName}\">",
            TagType.GenericParameterDeclaration => $"<typeparam name=\"{typeName}\">",
            _ => string.Empty,
        };


    public override string Build()
    {
        var doc = Template
            .Replace("{DocSummaryStart}", DocSummaryStart)
            .Replace("{DocExtendedTypeRef}", DocExtendedTypeRef)
            .Replace("{DocSuccessTypeRef}", DocSuccessTypeRef)
            .Replace("{DocReturnRef}", DocReturnRef)
            .Replace("{DocTypeparamDocs}", DocTypeparamDocs);

        return RemoveEmptyLines(doc);
    }
}
