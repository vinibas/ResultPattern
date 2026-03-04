/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using System.Reflection;
using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Snapshots;

public class GeneratedCodeSnapshotTests
{
    private static readonly string GeneratedDir = typeof(MatchResultsClassBlueprintBuilder).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .First(a => a.Key == "GeneratedOutputDir").Value!;

    private readonly MatchResultsClassBlueprintBuilder _builder = MatchResultsClassBlueprintBuilder.Create();

    [Theory]
    [InlineData(MatchResultsClassBlueprintBuilder.KeyMvc, MatchResultsClassBlueprintBuilder.ClassNameMvc + ".g.cs")]
    [InlineData(MatchResultsClassBlueprintBuilder.KeyMinimalApiAbstract, MatchResultsClassBlueprintBuilder.ClassNameMinimalApiAbstract + ".g.cs")]
    [InlineData(MatchResultsClassBlueprintBuilder.KeyMinimalApiTyped, MatchResultsClassBlueprintBuilder.ClassNameMinimalApiTyped + ".g.cs")]
    [InlineData(MatchResultsClassBlueprintBuilder.KeyMinimalApiUnion, MatchResultsClassBlueprintBuilder.ClassNameMinimalApiUnion + ".g.cs")]
    public void GeneratedOutput_ShouldMatchCommittedFile(string blueprintKey, string fileName)
    {
        var generated = _builder.Build(blueprintKey).Build();
        var committedFilePath = Path.Combine(GeneratedDir, fileName);
        var committed = File.ReadAllText(committedFilePath);

        Assert.Equal(committed, generated);
    }
}
