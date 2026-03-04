/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;
using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;
using static ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod.MatchMethodBuilderBase;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

internal sealed class MatchMethodBlueprintBuilderTestHelper : MatchMethodBlueprintBuilder
{
    public new IReadOnlyList<string> RegisteredBlueprintKeys
        => base.RegisteredBlueprintKeys;

    public MatchMethodBuilderParameters GetBlueprint(string key)
        => CreateBlueprint(key);

    public MatchMethodBuilder BuildFromBlueprint(MatchMethodBuilderParameters blueprint)
        => GetInstance(blueprint);
}
