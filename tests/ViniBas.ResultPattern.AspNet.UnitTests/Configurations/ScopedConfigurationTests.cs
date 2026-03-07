/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.Configurations;

namespace ViniBas.ResultPattern.AspNet.UnitTests.Configurations;

public class ScopedConfigurationTests
{
    [Fact]
    public void Override_WhenCalled_SetsCurrent()
    {
        using (ScopedConfiguration.Override(useProblemDetails: false))
        {
            Assert.NotNull(ScopedConfiguration.Current);
            Assert.Equal(false, ScopedConfiguration.Current!.UseProblemDetails);
        }
    }

    [Fact]
    public void Override_WhenDisposed_RestoresPreviousNull()
    {
        var scope = ScopedConfiguration.Override(useProblemDetails: true);

        scope.Dispose();

        Assert.Null(ScopedConfiguration.Current);
    }

    [Fact]
    public void Override_WhenNested_InnerScopeTakesPrecedence()
    {
        using (ScopedConfiguration.Override(useProblemDetails: true))
        {
            using (ScopedConfiguration.Override(useProblemDetails: false))
            {
                Assert.Equal(false, ScopedConfiguration.Current!.UseProblemDetails);
            }

            Assert.Equal(true, ScopedConfiguration.Current!.UseProblemDetails);
        }

        Assert.Null(ScopedConfiguration.Current);
    }

    [Fact]
    public void Override_WhenDisposed_RestoringOuterScope()
    {
        using (ScopedConfiguration.Override(useProblemDetails: false))
        {
            var inner = ScopedConfiguration.Override(useProblemDetails: true);
            inner.Dispose();

            Assert.Equal(false, ScopedConfiguration.Current!.UseProblemDetails);
        }
    }

    [Fact]
    public async Task Override_AsyncLocal_DoesNotLeakToOtherTasks()
    {
        bool? outsideValue = null;

        using (ScopedConfiguration.Override(useProblemDetails: true))
        {
            await Task.Run(() =>
            {
                // AsyncLocal propagates downward to child tasks
                outsideValue = ScopedConfiguration.Current?.UseProblemDetails;
            });
        }

        // After the scope is disposed, a new independent task sees no scope
        bool? independentValue = null;
        await Task.Run(() =>
        {
            independentValue = ScopedConfiguration.Current?.UseProblemDetails;
        });

        Assert.Equal(true, outsideValue);
        Assert.Null(independentValue);
    }

    [Fact]
    public void Current_WhenNoScopeActive_ReturnsNull()
    {
        Assert.Null(ScopedConfiguration.Current);
    }

    [Fact]
    public void Override_WithNullUseProblemDetails_SetsCurrentWithNullValue()
    {
        using (ScopedConfiguration.Override(useProblemDetails: null))
        {
            Assert.NotNull(ScopedConfiguration.Current);
            Assert.Null(ScopedConfiguration.Current!.UseProblemDetails);
        }
    }

    [Fact]
    public void Override_WithUnwrapSuccessData_SetsCurrent()
    {
        using (ScopedConfiguration.Override(unwrapSuccessData: true))
        {
            Assert.NotNull(ScopedConfiguration.Current);
            Assert.Equal(true, ScopedConfiguration.Current!.UnwrapSuccessData);
        }
    }

    [Fact]
    public void Override_WithUnwrapSuccessDataNull_DefaultsToNull()
    {
        using (ScopedConfiguration.Override(useProblemDetails: false))
        {
            Assert.NotNull(ScopedConfiguration.Current);
            Assert.Null(ScopedConfiguration.Current!.UnwrapSuccessData);
        }
    }

    [Fact]
    public void Override_WithBothParameters_SetsBoth()
    {
        using (ScopedConfiguration.Override(useProblemDetails: true, unwrapSuccessData: true))
        {
            Assert.NotNull(ScopedConfiguration.Current);
            Assert.Equal(true, ScopedConfiguration.Current!.UseProblemDetails);
            Assert.Equal(true, ScopedConfiguration.Current!.UnwrapSuccessData);
        }
    }
}
