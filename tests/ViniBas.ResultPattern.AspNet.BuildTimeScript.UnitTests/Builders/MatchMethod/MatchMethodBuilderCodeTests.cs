/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;
using ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Builders.MatchMethod;

public class MatchMethodBuilderCodeTests
{
    private static MatchMethodParamsBuilder Params() => MatchMethodParamsBuilder.Create();

    [Fact]
    public void Build_SyncSimpleReturn_Result()
    {
        var result = new MatchMethodBuilderCode(Params().Build()).Build();

        var expected =
"""
    public static IActionResult Match(
        this Result result,
        Func<ResultResponseSuccess, IActionResult>? onSuccess = null,
        Func<ResultResponseError, IActionResult>? onFailure = null)
        => Matcher.Match(
            result,
            onSuccess is not null ? rr => onSuccess((ResultResponseSuccess)rr) : null,
            onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
            null);
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_AsyncSimpleReturn_Result()
    {
        var @params = Params().Set(p => p.IsAsync, true).Build();
        var result = new MatchMethodBuilderCode(@params).Build();

        var expected =
"""
    public static Task<IActionResult> MatchAsync(
        this Result result,
        Func<ResultResponseSuccess, Task<IActionResult>>? onSuccess = null,
        Func<ResultResponseError, Task<IActionResult>>? onFailure = null)
        => Matcher.MatchAsync(
            result,
            onSuccess is not null ? rr => onSuccess((ResultResponseSuccess)rr) : null,
            onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
            null);
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_SyncSimpleReturn_ResultWithTData()
    {
        var @params = Params()
            .Set(p => p.HasSuccessDataType, true)
            .Set(p => p.ExtendedType, "Result<>")
            .Build();
        var result = new MatchMethodBuilderCode(@params).Build();

        var expected =
"""
    public static IActionResult Match<TData>(
        this Result<TData> result,
        Func<ResultResponseSuccess<TData>, IActionResult>? onSuccess = null,
        Func<ResultResponseError, IActionResult>? onFailure = null)
        => Matcher.Match(
            result,
            onSuccess is not null ? rr => onSuccess((ResultResponseSuccess<TData>)rr) : null,
            onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
            null);
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_SyncSimpleReturn_ResultResponse()
    {
        var @params = Params()
            .Set(p => p.ExtendedType, "ResultResponse")
            .Build();
        var result = new MatchMethodBuilderCode(@params).Build();

        var expected =
"""
    public static IActionResult Match(
        this ResultResponse result,
        Func<ResultResponseSuccess, IActionResult>? onSuccess = null,
        Func<ResultResponseError, IActionResult>? onFailure = null)
        => Matcher.Match(
            result,
            onSuccess is not null ? rr => onSuccess((ResultResponseSuccess)rr) : null,
            onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
            null);
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_SyncGenericReturn_ResultWithConstraints()
    {
        var @params = Params()
            .Set(p => p.ReturnTypeName, "TResult")
            .Set(p => p.IsGenericReturnType, true)
            .Set(p => p.GenericConstraints,
                (IEnumerable<(string, IEnumerable<string>)>)
                [("TResult", (IEnumerable<string>)["IResult", "IEndpointMetadataProvider"])])
            .Set(p => p.ShouldMatcherReceiveReturnGenericParameter, true)
            .Build();
        var result = new MatchMethodBuilderCode(@params).Build();

        var expected =
"""
    public static TResult Match<TResult>(
        this Result result,
        Func<ResultResponseSuccess, TResult>? onSuccess = null,
        Func<ResultResponseError, TResult>? onFailure = null)
        where TResult : IResult, IEndpointMetadataProvider
        => Matcher.Match<TResult>(
            result,
            onSuccess is not null ? rr => onSuccess((ResultResponseSuccess)rr) : null,
            onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
            null);
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_SyncResultsReturn_ResultWithMultipleConstraints()
    {
        var @params = Params()
            .Set(p => p.ReturnTypeName, "Results<>")
            .Set(p => p.ReturnTypeGenericParameters, ["TResult1", "TResult2"])
            .Set(p => p.MethodSufixName, "Results")
            .Set(p => p.GenericConstraints,
                (IEnumerable<(string, IEnumerable<string>)>)
                [
                    ("TResult1", ["IResult", "IEndpointMetadataProvider"]),
                    ("TResult2", ["IResult", "IEndpointMetadataProvider"]),
                ])
            .Set(p => p.ShouldMatcherReceiveReturnGenericParameter, true)
            .Build();
        var result = new MatchMethodBuilderCode(@params).Build();

        var expected =
"""
    public static Results<TResult1, TResult2> MatchResults<TResult1, TResult2>(
        this Result result,
        Func<ResultResponseSuccess, Results<TResult1, TResult2>>? onSuccess = null,
        Func<ResultResponseError, Results<TResult1, TResult2>>? onFailure = null)
        where TResult1 : IResult, IEndpointMetadataProvider
        where TResult2 : IResult, IEndpointMetadataProvider
        => Matcher.Match<Results<TResult1, TResult2>>(
            result,
            onSuccess is not null ? rr => onSuccess((ResultResponseSuccess)rr) : null,
            onFailure is not null ? rr => onFailure((ResultResponseError)rr) : null,
            null);
""";
        Assert.Equal(expected, result);
    }
}
