/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.AspNet.Generators.Builders;

public sealed class MatchResultsClassBuilder
{
    private const string ClassTemplate =
    """
    #nullable enable
    {Imports}

    namespace {Namespace};

    public static class {ClassName}
    {
        {MatcherProperty}

    {Methods}
    }
    """;

    public IEnumerable<string> Imports { get; init; }
    public string Namespace { get; init; }
    public string ClassName { get; init; }
    public IEnumerable<MatchMethodBuilder> Methods { get; init; } = Enumerable.Empty<MatchMethodBuilder>();
    public string MatcherProperty { get; init; }

    public MatchResultsClassBuilder(
        IEnumerable<string> imports,
        string @namespace,
        string className,
        IEnumerable<MatchMethodBuilder> methods,
        string matcherProperty)
    {
        Imports = imports;
        Namespace = @namespace;
        ClassName = className;
        Methods = methods;
        MatcherProperty = matcherProperty;
    }

    public string Build()
    {
        var methodsCode = string.Join("\n\n", Methods.Select(m => m.Build()));
        return ClassTemplate
            .Replace("{Imports}", string.Join("\n", Imports.Select(i => $"using {i};")))
            .Replace("{Namespace}", Namespace)
            .Replace("{ClassName}", ClassName)
            .Replace("{MatcherProperty}", MatcherProperty)
            .Replace("{Methods}", methodsCode);
    }
}
