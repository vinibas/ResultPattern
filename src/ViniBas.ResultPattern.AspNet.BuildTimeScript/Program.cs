using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

var outputPath = args[0];
Directory.CreateDirectory(outputPath);
Console.WriteLine($"Generating code in: {outputPath}");

var classBuilder = MatchResultsClassBlueprintBuilder.Create();

var mvcClass = classBuilder.Build("mvc").Build();
var minimalApiClass = classBuilder.Build("minimalApiAbstract").Build();
var minimalApiTypedClass = classBuilder.Build("minimalApiTyped").Build();
var minimalApiResultClass = classBuilder.Build("minimalApiResults").Build();

File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_Mvc.g.cs"), mvcClass);
File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_MinimalApi.g.cs"), minimalApiClass);
File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_MinimalApi_Typed.g.cs"), minimalApiTypedClass);
File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_MinimalApi_Results.g.cs"), minimalApiResultClass);
