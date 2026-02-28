/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.FluentBlueprintBuilder;
using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;
using static ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchResultsClassBuilder;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

internal sealed class MatchResultsClassBlueprintBuilder
    : BlueprintBuilder<MatchResultsClassBlueprintBuilder, MatchResultsClassBuilderParameters, MatchResultsClassBuilder>
{
    private IEnumerable<string> _commonNamespaces = [
        "ViniBas.ResultPattern.ResultResponses",
        "ViniBas.ResultPattern.ResultObjects",
        "ViniBas.ResultPattern.AspNet.ResultMatcher",
    ];

    protected override void ConfigureBlueprints(IDictionary<string, Func<MatchResultsClassBuilderParameters>> blueprints)
    {
        blueprints["mvc"] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Mvc",
                .._commonNamespaces,
            ],
            Namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ActionResultMvc",
            ClassName: "MatchResultsExtensions",
            Methods: GenerateMethods("IActionResult", null, false, null, null, false),
            MatcherProperty: "private static ISimpleResultMatcher<IActionResult> Matcher => ResultMatcherFactory.GetActionResultMatcher;"
        );

        blueprints["minimalApiAbstract"] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Http",
                .._commonNamespaces,
            ],
            Namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi",
            ClassName: "MatchResultsExtensions",
            Methods: GenerateMethods("IResult", null, false, null, null, false),
            MatcherProperty: "private static ISimpleResultMatcher<IResult> Matcher => ResultMatcherFactory.GetMinimalApiMatcher;"
        );

        blueprints["minimalApiTyped"] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Http",
                "Microsoft.AspNetCore.Http.Metadata",
                "Microsoft.AspNetCore.Http.HttpResults",
                .._commonNamespaces,
            ],
            Namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi",
            ClassName: "MatchTResultsExtensions",
            Methods: GenerateMethods("TResult", null, true, null, [("TResult", ["IResult", "IEndpointMetadataProvider"])], true),
            MatcherProperty: "private static ITypedResultMatcher Matcher => ResultMatcherFactory.GetTypedMatcher;"
        );

        blueprints["minimalApiResults"] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Http",
                "Microsoft.AspNetCore.Http.Metadata",
                "Microsoft.AspNetCore.Http.HttpResults",
                .._commonNamespaces,
            ],
            Namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi",
            ClassName: "MatchResultsResultsExtensions",
            Methods: GenerateResultsMethods(),
            MatcherProperty: "private static ITypedResultMatcher Matcher => ResultMatcherFactory.GetTypedMatcher;"
        );
    }

    private static IEnumerable<MatchMethodBuilder> GenerateMethods(
        string returnTypeName,
        IEnumerable<string>? returnTypeGenericParameters,
        bool isGenericReturnType,
        string? methodSufixName = null,
        IEnumerable<(string GenericTypeName, IEnumerable<string> GenericConstraints)>? genericConstraints = null,
        bool ShouldMatcherReceiveReturnGenericParameter = false)
        => MatchMethodBlueprintBuilder.Create()
            .Set(m => m.ReturnTypeName, returnTypeName)
            .Set(m => m.ReturnTypeGenericParameters, returnTypeGenericParameters ?? [])
            .Set(m => m.IsGenericReturnType, isGenericReturnType)
            .Set(m => m.MethodSufixName, methodSufixName)
            .Set(m => m.GenericConstraints, genericConstraints ?? [])
            .Set(m => m.ShouldMatcherReceiveReturnGenericParameter, ShouldMatcherReceiveReturnGenericParameter)
            .BuildMany(size: null);

    private static IEnumerable<MatchMethodBuilder> GenerateResultsMethods()
    {
        var methods = new List<MatchMethodBuilder>();

        List<string> genericResultTypes = ["TResult1"];

        for (var i = 2; i <= 6; i++)
        {
            var genericResultType = $"TResult{i}";
            genericResultTypes.Add(genericResultType);

            var constraints = genericResultTypes
                .Select(r => (r, new [] {"IResult", "IEndpointMetadataProvider"}.AsEnumerable()))
                .ToList();

            var method = GenerateMethods("Results<>", genericResultTypes.ToList(), false, "Results", constraints, true);
            methods.AddRange(method);
        }

        return methods;
    }

    protected override MatchResultsClassBuilder GetInstance(MatchResultsClassBuilderParameters blueprint)
        => new (blueprint);
}
