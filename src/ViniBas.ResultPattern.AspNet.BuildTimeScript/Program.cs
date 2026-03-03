/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

var outputPath = args[0];
Directory.CreateDirectory(outputPath);
Console.WriteLine($"Generating code in: {outputPath}");

var classBuilder = MatchResultsClassBlueprintBuilder.Create();

var mvcClass = classBuilder.Build(MatchResultsClassBlueprintBuilder.KeyMvc).Build();
var minimalApiClass = classBuilder.Build(MatchResultsClassBlueprintBuilder.KeyMinimalApiAbstract).Build();
var minimalApiTypedClass = classBuilder.Build(MatchResultsClassBlueprintBuilder.KeyMinimalApiTyped).Build();
var minimalApiUnionClass = classBuilder.Build(MatchResultsClassBlueprintBuilder.KeyMinimalApiUnion).Build();

File.WriteAllText(Path.Combine(outputPath, MatchResultsClassBlueprintBuilder.ClassNameMvc + ".g.cs"), mvcClass);
File.WriteAllText(Path.Combine(outputPath, MatchResultsClassBlueprintBuilder.ClassNameMinimalApiAbstract + ".g.cs"), minimalApiClass);
File.WriteAllText(Path.Combine(outputPath, MatchResultsClassBlueprintBuilder.ClassNameMinimalApiTyped + ".g.cs"), minimalApiTypedClass);
File.WriteAllText(Path.Combine(outputPath, MatchResultsClassBlueprintBuilder.ClassNameMinimalApiUnion + ".g.cs"), minimalApiUnionClass);
