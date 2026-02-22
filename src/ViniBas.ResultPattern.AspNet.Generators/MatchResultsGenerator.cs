using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ViniBas.ResultPattern.AspNet.Generators.Builders;

namespace ViniBas.ResultPattern.AspNet.Generators;

[Generator]
public class MatchResultsGenerator : ISourceGenerator
{
    private readonly VariationsGenerator variationsGenerator = new VariationsGenerator();

    private readonly string _mvcClass;
    private readonly string _minimalApiClass;
    private readonly string _minimalApiClassTyped;

    private SourceText MvcClassSourceText => SourceText.From(_mvcClass, Encoding.UTF8);
    private SourceText MinimalApiClassSourceText => SourceText.From(_minimalApiClass, Encoding.UTF8);
    private SourceText MinimalApiClassTypedSourceText => SourceText.From(_minimalApiClassTyped, Encoding.UTF8);

    public MatchResultsGenerator()
    {
        _mvcClass = variationsGenerator.GenerateMvcClass();
        _minimalApiClass = variationsGenerator.GenerateMinimalApiClass();
        _minimalApiClassTyped = variationsGenerator.GenerateMinimalApiTypedClass();
    }

    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("MatchResultsExtensions_Mvc.g.cs", MvcClassSourceText);
        context.AddSource("MatchResultsExtensions_MinimalApi.g.cs", MinimalApiClassSourceText);
        context.AddSource("MatchResultsExtensions_MinimalApi_Typed.g.cs", MinimalApiClassTypedSourceText);
    }

}
