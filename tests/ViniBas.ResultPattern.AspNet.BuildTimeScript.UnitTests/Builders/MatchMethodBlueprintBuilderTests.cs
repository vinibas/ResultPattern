/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Builders;

public class MatchMethodBlueprintBuilderTests
{
    private readonly MatchMethodBlueprintBuilderTestHelper _helper = new();

    [Fact]
    public void ConfigureBlueprints_ShouldRegister8Blueprints()
    {
        Assert.Equal(8, _helper.RegisteredBlueprintKeys.Count);
    }

    [Theory]
    [InlineData("0", false, "Result", false)]
    [InlineData("1", false, "Result<>", true)]
    [InlineData("2", false, "ResultResponse", false)]
    [InlineData("3", false, "ResultResponse", true)]
    [InlineData("4", true, "Result", false)]
    [InlineData("5", true, "Result<>", true)]
    [InlineData("6", true, "ResultResponse", false)]
    [InlineData("7", true, "ResultResponse", true)]
    public void GetBlueprint_ShouldHaveCorrectFixedValues(
        string key, bool expectedIsAsync, string expectedExtendedType, bool expectedHasSuccessDataType)
    {
        var blueprint = _helper.GetBlueprint(key);

        Assert.Equal(expectedIsAsync, blueprint.IsAsync);
        Assert.Equal(expectedExtendedType, blueprint.ExtendedType);
        Assert.Equal(expectedHasSuccessDataType, blueprint.HasSuccessDataType);
    }

    [Fact]
    public void GetInstance_ShouldReturnMatchMethodBuilder()
    {
        var blueprint = _helper.GetBlueprint("0");

        var instance = _helper.BuildFromBlueprint(blueprint);

        Assert.NotNull(instance);
    }
}
