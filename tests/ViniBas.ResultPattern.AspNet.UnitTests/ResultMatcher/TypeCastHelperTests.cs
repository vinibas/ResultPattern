/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using ViniBas.ResultPattern.AspNet.ResultMatcher;

namespace ViniBas.ResultPattern.AspNet.UnitTests;

public class TypeCastHelperTests
{
    [Fact]
    public void TreatCast_WithDirectCast_ShouldSucceed()
    {
        IResult result = TypedResults.Ok("test");
        
        var casted = TypeCastHelper.TreatCast<Ok<string>>(result);
        
        Assert.IsType<Ok<string>>(casted);
    }

    [Fact]
    public void TreatCast_WithImplicitOperator_ShouldUseCache()
    {
        IResult result = TypedResults.Ok("test");
        
        var callWithReflection = TypeCastHelper.TreatCast<Results<Ok<string>, NotFound>>(result);
        var callWithCache0 = TypeCastHelper.TreatCast<Results<Ok<string>, NotFound>>(result);
        
        Assert.IsType<Results<Ok<string>, NotFound>>(callWithReflection);
        Assert.IsType<Results<Ok<string>, NotFound>>(callWithCache0);
    }

    [Fact]
    public void TreatCast_WithIncompatibleTypes_ShouldThrow()
    {
        IResult result = TypedResults.Ok("test");
        
        var ex = Assert.Throws<InvalidOperationException>(
            () => TypeCastHelper.TreatCast<BadRequest<string>>(result));

        Assert.Contains("The type provided for T_Result (BadRequest`1) is not compatible with the result (Ok`1).", ex.Message);
    }
}