/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;
using static ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchResultsClassBuilder;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

internal sealed class MatchResultsClassBlueprintBuilderTestHelper : MatchResultsClassBlueprintBuilder
{
    public new IReadOnlyList<string> RegisteredBlueprintKeys
        => base.RegisteredBlueprintKeys;

    public MatchResultsClassBuilderParameters GetBlueprint(string key)
        => CreateBlueprint(key);

    public MatchResultsClassBuilder BuildFromBlueprint(MatchResultsClassBuilderParameters blueprint)
        => GetInstance(blueprint);
}
