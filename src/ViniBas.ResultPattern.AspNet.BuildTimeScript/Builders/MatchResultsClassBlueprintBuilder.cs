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

internal class MatchResultsClassBlueprintBuilder
    : BlueprintBuilder<MatchResultsClassBlueprintBuilder, MatchResultsClassBuilderParameters, MatchResultsClassBuilder>
{
    public const string KeyMvc = "mvc";
    public const string KeyMinimalApiAbstract = "minimalApiAbstract";
    public const string KeyMinimalApiTyped = "minimalApiTyped";
    public const string KeyMinimalApiUnion = "minimalApiUnion";

    public const string ClassNameMvc = "MvcMatchResultsExtensions";
    public const string ClassNameMinimalApiAbstract = "MinimalApiMatchResultsExtensions";
    public const string ClassNameMinimalApiTyped = "MinimalApiTypedMatchResultsExtensions";
    public const string ClassNameMinimalApiUnion = "MinimalApiUnionTypesMatchResultsExtensions";

    public const string MvcNamespace = "ViniBas.ResultPattern.AspNet.Mvc";
    public const string MinimalApiNamespace = "ViniBas.ResultPattern.AspNet.MinimalApi";

    private IEnumerable<string> _commonNamespaces = [
        "ViniBas.ResultPattern.ResultResponses",
        "ViniBas.ResultPattern.ResultObjects",
        "ViniBas.ResultPattern.AspNet.ResultMatcher",
    ];

    protected override void ConfigureBlueprints(IDictionary<string, Func<MatchResultsClassBuilderParameters>> blueprints)
    {
        blueprints[KeyMvc] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Mvc",
                .._commonNamespaces,
            ],
            Namespace: MvcNamespace,
            ClassName: ClassNameMvc,
            Methods: GenerateMethods("IActionResult", null, false, null, null, false),
            MatcherProperty: "private static ISimpleResultMatcher<IActionResult> Matcher => ResultMatcherFactory.GetActionResultMatcher;"
        );

        blueprints[KeyMinimalApiAbstract] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Http",
                .._commonNamespaces,
            ],
            Namespace: MinimalApiNamespace,
            ClassName: ClassNameMinimalApiAbstract,
            Methods: GenerateMethods("IResult", null, false, null, null, false),
            MatcherProperty: "private static ISimpleResultMatcher<IResult> Matcher => ResultMatcherFactory.GetMinimalApiMatcher;"
        );

        blueprints[KeyMinimalApiTyped] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Http",
                "Microsoft.AspNetCore.Http.Metadata",
                "Microsoft.AspNetCore.Http.HttpResults",
                .._commonNamespaces,
            ],
            Namespace: MinimalApiNamespace,
            ClassName: ClassNameMinimalApiTyped,
            Methods: GenerateMethods("TResult", null, true, null, [("TResult", ["IResult", "IEndpointMetadataProvider"])], true),
            MatcherProperty: "private static ITypedResultMatcher Matcher => ResultMatcherFactory.GetTypedMatcher;"
        );

        blueprints[KeyMinimalApiUnion] = () => new MatchResultsClassBuilderParameters(
            Imports:
            [
                "Microsoft.AspNetCore.Http",
                "Microsoft.AspNetCore.Http.Metadata",
                "Microsoft.AspNetCore.Http.HttpResults",
                .._commonNamespaces,
            ],
            Namespace: MinimalApiNamespace,
            ClassName: ClassNameMinimalApiUnion,
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
