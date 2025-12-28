/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.ResultMatcher.Implementations;

namespace ViniBas.ResultPattern.AspNet.ResultMatcher;

internal sealed class ResultMatcherFactory
{
    internal static Lazy<ISimpleResultMatcher<IActionResult>> ActionResultMatcherFactory { private get; set; }
        = new(() => new ActionResultMatcher());
    
    internal static Lazy<ISimpleResultMatcher<IResult>> MinimalApiMatcherFactory { private get; set; }
        = new(() => new MinimalApiMatcher());
    
    internal static Lazy<ITypedResultMatcher> TypedMatcherFactory { private get; set; }
        = new(() => new TypedResultMatcher());

    public static ISimpleResultMatcher<IActionResult> GetActionResultMatcher
        => ActionResultMatcherFactory.Value;
    
    public static ISimpleResultMatcher<IResult> GetMinimalApiMatcher
        => MinimalApiMatcherFactory.Value;
    
    public static ITypedResultMatcher GetTypedMatcher
        => TypedMatcherFactory.Value;
    
    internal static void Reset()
    {
        ActionResultMatcherFactory = new(() => new ActionResultMatcher());
        MinimalApiMatcherFactory = new(() => new MinimalApiMatcher());
        TypedMatcherFactory = new(() => new TypedResultMatcher());
    }
}
