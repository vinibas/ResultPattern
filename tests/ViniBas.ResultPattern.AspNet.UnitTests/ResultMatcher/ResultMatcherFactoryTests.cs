/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.UnitTests.ResultMatcher;

public class ResultMatcherFactoryTests
{
    [Fact]
    public void GetMatcherMethods_ShouldReturnDefaultInstances()
    {
        var actionResultMatcher = ResultMatcherFactory.GetActionResultMatcher;
        var minimalApiMatcher = ResultMatcherFactory.GetMinimalApiMatcher;
        var typedMatcher = ResultMatcherFactory.GetTypedMatcher;
        
        Assert.NotNull(actionResultMatcher);
        Assert.NotNull(minimalApiMatcher);
        Assert.NotNull(typedMatcher);
    }
    
    [Fact]
    public void Reset_ShouldResetFactoriesToDefault()
    {
        // Arrange
        ResultMatcherFactory.ActionResultMatcherFactory = 
            new(() => throw new Exception("Custom ActionResultMatcherFactory should not be used after reset."));
        ResultMatcherFactory.MinimalApiMatcherFactory = 
            new(() => throw new Exception("Custom MinimalApiMatcherFactory should not be used after reset."));
        ResultMatcherFactory.TypedMatcherFactory = 
            new(() => throw new Exception("Custom TypedMatcherFactory should not be used after reset."));

        // Act
        ResultMatcherFactory.Reset();

        // Assert
        var actionResultMatcher = ResultMatcherFactory.GetActionResultMatcher;
        var minimalApiMatcher = ResultMatcherFactory.GetMinimalApiMatcher;
        var typedMatcher = ResultMatcherFactory.GetTypedMatcher;

        Assert.NotNull(actionResultMatcher);
        Assert.NotNull(minimalApiMatcher);
        Assert.NotNull(typedMatcher);
    }
}
