/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.FluentBlueprintBuilder;
using static ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod.MatchMethodBuilderBase;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

internal sealed class MatchMethodParamsBuilder
    : BlueprintBuilder<MatchMethodParamsBuilder, MatchMethodBuilderParameters, MatchMethodBuilderParameters>
{
    protected override void ConfigureBlueprints(IDictionary<string, Func<MatchMethodBuilderParameters>> blueprints)
    {
        blueprints["0"] = () => new MatchMethodBuilderParameters(
            IsAsync: false,
            ReturnTypeName: "IActionResult",
            ReturnTypeGenericParameters: [],
            IsGenericReturnType: false,
            MethodSufixName: null,
            HasSuccessDataType: false,
            ExtendedType: "Result",
            GenericConstraints: [],
            ShouldMatcherReceiveReturnGenericParameter: false);
    }

    protected override MatchMethodBuilderParameters GetInstance(MatchMethodBuilderParameters blueprint)
        => blueprint;
}
