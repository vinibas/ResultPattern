/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Builders;

public class MatchResultsClassBlueprintBuilderTests
{
    private readonly MatchResultsClassBlueprintBuilderTestHelper _helper = new();

    [Fact]
    public void ConfigureBlueprints_ShouldRegister4Blueprints()
    {
        Assert.Equal(4, _helper.RegisteredBlueprintKeys.Count);
        Assert.Equal(
            ["mvc", "minimalApiAbstract", "minimalApiTyped", "minimalApiResults"],
            _helper.RegisteredBlueprintKeys);
    }

    [Fact]
    public void MvcBlueprint_ShouldHaveCorrectConfiguration()
    {
        var blueprint = _helper.GetBlueprint("mvc");

        Assert.Equal("ViniBas.ResultPattern.AspNet.SourceGenerator.ActionResultMvc", blueprint.Namespace);
        Assert.Equal("MatchResultsExtensions", blueprint.ClassName);
        Assert.Contains("Microsoft.AspNetCore.Mvc", blueprint.Imports);
        Assert.Contains("ISimpleResultMatcher<IActionResult>", blueprint.MatcherProperty);
        Assert.Equal(8, blueprint.Methods.Count());
    }

    [Fact]
    public void MinimalApiAbstractBlueprint_ShouldHaveCorrectConfiguration()
    {
        var blueprint = _helper.GetBlueprint("minimalApiAbstract");

        Assert.Equal("ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi", blueprint.Namespace);
        Assert.Equal("MatchResultsExtensions", blueprint.ClassName);
        Assert.Contains("Microsoft.AspNetCore.Http", blueprint.Imports);
        Assert.Contains("ISimpleResultMatcher<IResult>", blueprint.MatcherProperty);
        Assert.Equal(8, blueprint.Methods.Count());
    }

    [Fact]
    public void MinimalApiTypedBlueprint_ShouldHaveCorrectConfiguration()
    {
        var blueprint = _helper.GetBlueprint("minimalApiTyped");

        Assert.Equal("ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi", blueprint.Namespace);
        Assert.Equal("MatchTResultsExtensions", blueprint.ClassName);
        Assert.Contains("ITypedResultMatcher", blueprint.MatcherProperty);
        Assert.Equal(8, blueprint.Methods.Count());
    }

    [Fact]
    public void MinimalApiResultsBlueprint_ShouldHave40Methods()
    {
        var blueprint = _helper.GetBlueprint("minimalApiResults");

        Assert.Equal("ViniBas.ResultPattern.AspNet.SourceGenerator.ResultMinimalApi", blueprint.Namespace);
        Assert.Equal("MatchResultsResultsExtensions", blueprint.ClassName);
        Assert.Contains("ITypedResultMatcher", blueprint.MatcherProperty);
        // 5 variants (TResult1-TResult2 through TResult1-TResult6) x 8 overloads = 40
        Assert.Equal(40, blueprint.Methods.Count());
    }

    [Fact]
    public void AllBlueprints_ShouldContainCommonNamespaces()
    {
        foreach (var key in _helper.RegisteredBlueprintKeys)
        {
            var blueprint = _helper.GetBlueprint(key);

            Assert.Contains("ViniBas.ResultPattern.ResultResponses", blueprint.Imports);
            Assert.Contains("ViniBas.ResultPattern.ResultObjects", blueprint.Imports);
            Assert.Contains("ViniBas.ResultPattern.AspNet.ResultMatcher", blueprint.Imports);
        }
    }

    [Fact]
    public void GetInstance_ShouldReturnMatchResultsClassBuilder()
    {
        var blueprint = _helper.GetBlueprint("mvc");

        var instance = _helper.BuildFromBlueprint(blueprint);

        Assert.NotNull(instance);
    }
}
