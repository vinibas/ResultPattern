using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders;

var outputPath = args[0];
Directory.CreateDirectory(outputPath);
Console.WriteLine($"Generating code in: {outputPath}");

var variationsGenerator = new VariationsGenerator();

var mvcClass = variationsGenerator.GenerateMvcClass();
var minimalApiClass = variationsGenerator.GenerateMinimalApiClass();
var minimalApiClassTyped = variationsGenerator.GenerateMinimalApiTypedClass();

File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_Mvc.g.cs"), mvcClass);
File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_MinimalApi.g.cs"), minimalApiClass);
File.WriteAllText(Path.Combine(outputPath, "MatchResultsExtensions_MinimalApi_Typed.g.cs"), minimalApiClassTyped);
