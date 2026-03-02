/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.FluentBlueprintBuilder;
using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;
using static ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchResultsClassBuilder;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

internal sealed class MatchResultsClassParamsBuilder
    : BlueprintBuilder<MatchResultsClassParamsBuilder, MatchResultsClassBuilderParameters, MatchResultsClassBuilderParameters>
{
    protected override void ConfigureBlueprints(IDictionary<string, Func<MatchResultsClassBuilderParameters>> blueprints)
    {
        blueprints["0"] = () => new MatchResultsClassBuilderParameters(
            Imports: ["Microsoft.AspNetCore.Mvc"],
            Namespace: "TestNamespace",
            ClassName: "TestExtensions",
            Methods: [new MatchMethodBuilder(MatchMethodParamsBuilder.Create().Build())],
            MatcherProperty: "private static IMatcher Matcher => Factory.GetMatcher;");
    }

    protected override MatchResultsClassBuilderParameters GetInstance(MatchResultsClassBuilderParameters blueprint)
        => blueprint;
}
