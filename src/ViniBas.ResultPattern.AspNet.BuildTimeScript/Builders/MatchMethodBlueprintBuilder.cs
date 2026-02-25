/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.FluentBlueprintBuilder;
using static ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethodBuilder;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

public class MatchMethodBlueprintBuilder : BlueprintBuilder<MatchMethodBlueprintBuilder, MatchMethodBuilderParameters, MatchMethodBuilder>
{
    protected override void ConfigureBlueprints(IDictionary<string, Func<MatchMethodBuilderParameters>> blueprints)
    {
        (string ExtendedType, bool HasSuccessDataType)[] extendedWithDataTypes =
        [
            ("Result", false),
            ("Result<>", true),
            ("ResultResponse", false),
            ("ResultResponse", true),
        ];
        var isAsyncOptions = new[] { false, true };
        var counter = 0;

        foreach (var isAsync in isAsyncOptions)
        foreach (var (extendedType, hasSuccessDataType) in extendedWithDataTypes)
            blueprints.Add(counter++.ToString(), () => new MatchMethodBuilderParameters(
                // Fixed values ​​will be overwritten by the Set method.
                isAsync, "ReturnTypeName", [], false, null, hasSuccessDataType, extendedType, [], false));
    }

    protected override MatchMethodBuilder GetInstance(MatchMethodBuilderParameters blueprint)
        => new (blueprint);
}
