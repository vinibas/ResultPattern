/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

// using ViniBas.ResultPattern.ResultObjects;
// using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.AspNet.Generators.Builders;

public class VariationsGenerator
{
    private MatchResultsClassBuilder _mvc;
    private MatchResultsClassBuilder _minimalApiAbstract;
    private MatchResultsClassBuilder _minimalApiTyped;

    public VariationsGenerator()
    {
        _mvc = new MatchResultsClassBuilder(
            imports:
            [
                "Microsoft.AspNetCore.Mvc",
                "ViniBas.ResultPattern.ResultResponses",
                "ViniBas.ResultPattern.ResultObjects",
                "ViniBas.ResultPattern.AspNet.ResultMatcher",
            ],
            @namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ActionResultMvc",
            className: "MatchResultsExtensions",
            // methods: GenerateMethods(typeof(IActionResult).Name),
            methods: GenerateMethods("IActionResult"),
            matcherProperty: "private static ISimpleResultMatcher<IActionResult> Matcher => ResultMatcherFactory.GetActionResultMatcher;"
        );

        _minimalApiAbstract = new MatchResultsClassBuilder(
            imports:
            [
                "ViniBas.ResultPattern.ResultResponses",
                "ViniBas.ResultPattern.ResultObjects",
                "Microsoft.AspNetCore.Http",
                "ViniBas.ResultPattern.AspNet.ResultMatcher",
            ],
            @namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi",
            className: "MatchResultsExtensions",
            // methods: GenerateMethods(typeof(IResult).Name),
            methods: GenerateMethods("IResult"),
            matcherProperty: "private static ISimpleResultMatcher<IResult> Matcher => ResultMatcherFactory.GetMinimalApiMatcher;"
        );

        _minimalApiTyped = new MatchResultsClassBuilder(
            imports:
            [
                "ViniBas.ResultPattern.ResultResponses",
                "ViniBas.ResultPattern.ResultObjects",
                "Microsoft.AspNetCore.Http",
                "ViniBas.ResultPattern.AspNet.ResultMatcher",
                "Microsoft.AspNetCore.Http.Metadata",
                "Microsoft.AspNetCore.Http.HttpResults",
            ],
            @namespace: "ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi",
            className: "MatchTResultsExtensions",
            methods: GenerateMethods("TResult", true, "where TResult : IResult, IEndpointMetadataProvider"),
            matcherProperty: "private static ITypedResultMatcher Matcher => ResultMatcherFactory.GetTypedMatcher;"
        );
    }

    private static IEnumerable<MatchMethodBuilder> GenerateMethods(string returnTypeName, bool isReturnTypeGeneric = false, string? genericConstraints = null)
    {
        (string ExtendedType, bool HasSuccessDataType)[] extendedWithDataTypes =
        [
            // (typeof(Result), false),
            // (typeof(Result<>), true),
            // (typeof(ResultResponse), false),
            // (typeof(ResultResponse), true),
            ("Result", false),
            ("Result<>", true),
            ("ResultResponse", false),
            ("ResultResponse", true),
        ];
        var isAsyncOptions = new[] { false, true };

        var methods = new List<MatchMethodBuilder>();

        foreach (var isAsync in isAsyncOptions)
        foreach (var (extendedType, hasSuccessDataType) in extendedWithDataTypes)
            methods.Add(new MatchMethodBuilder(extendedType, returnTypeName, hasSuccessDataType, isAsync, isReturnTypeGeneric, genericConstraints));

        return methods;
    }

    public string GenerateMvcClass() => _mvc.Build();
    public string GenerateMinimalApiClass() => _minimalApiAbstract.Build();
    public string GenerateMinimalApiTypedClass() => _minimalApiTyped.Build();
}
