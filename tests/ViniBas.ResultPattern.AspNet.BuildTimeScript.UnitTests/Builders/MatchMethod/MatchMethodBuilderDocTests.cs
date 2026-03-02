/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;
using ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Helpers;

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.UnitTests.Builders.MatchMethod;

public class MatchMethodBuilderDocTests
{
    private static MatchMethodParamsBuilder Params() => MatchMethodParamsBuilder.Create();

    [Fact]
    public void Build_SyncSimpleReturn_Result()
    {
        var result = new MatchMethodBuilderDoc(Params().Build()).Build();

        var expected =
"""
    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result"/> to evaluate.
    /// </param>
    /// <param name="onSuccess">
    /// Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="IActionResult"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnSuccessDefault().
    /// </param>
    /// <param name="onFailure">
    /// Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="IActionResult"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnFailureDefault().
    /// </param>
    /// <returns>
    /// Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.
    /// </returns>
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_AsyncSimpleReturn_Result()
    {
        var @params = Params().Set(p => p.IsAsync, true).Build();
        var result = new MatchMethodBuilderDoc(@params).Build();

        Assert.Contains("Asynchronously checks", result);
        Assert.Contains("""returns a <see cref="Task{IActionResult}"/>.""", result);
    }

    [Fact]
    public void Build_SyncSimpleReturn_ResultWithTData()
    {
        var @params = Params()
            .Set(p => p.HasSuccessDataType, true)
            .Set(p => p.ExtendedType, "Result<>")
            .Build();
        var result = new MatchMethodBuilderDoc(@params).Build();

        var expected =
"""
    /// <summary>
    /// Checks whether a <see cref="Result{TData}"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TData">
    /// Type of the success data value.
    /// </typeparam>
    /// <param name="result">
    /// The <see cref="Result{TData}"/> to evaluate.
    /// </param>
    /// <param name="onSuccess">
    /// Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess{TData}"/> and returns a <see cref="IActionResult"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnSuccessDefault().
    /// </param>
    /// <param name="onFailure">
    /// Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="IActionResult"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnFailureDefault().
    /// </param>
    /// <returns>
    /// Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.
    /// </returns>
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
                [("TResult", ["IResult", "IEndpointMetadataProvider"])])
            .Set(p => p.ShouldMatcherReceiveReturnGenericParameter, true)
            .Build();
        var result = new MatchMethodBuilderDoc(@params).Build();

        var expected =
"""
    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the expected result. Typically a
    /// <see cref="Results{T1, T2}"/>, but can be any type that implements
    /// <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.
    /// </typeparam>
    /// <param name="result">
    /// The <see cref="Result"/> to evaluate.
    /// </param>
    /// <param name="onSuccess">
    /// Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <typeparamref name="TResult"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnSuccessDefault().
    /// </param>
    /// <param name="onFailure">
    /// Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <typeparamref name="TResult"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnFailureDefault().
    /// </param>
    /// <returns>
    /// Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.
    /// </returns>
""";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Build_SyncGenericReturn_ResultWithTDataAndConstraints()
    {
        var @params = Params()
            .Set(p => p.ReturnTypeName, "TResult")
            .Set(p => p.IsGenericReturnType, true)
            .Set(p => p.HasSuccessDataType, true)
            .Set(p => p.ExtendedType, "Result<>")
            .Set(p => p.GenericConstraints,
                (IEnumerable<(string, IEnumerable<string>)>)
                [("TResult", ["IResult", "IEndpointMetadataProvider"])])
            .Set(p => p.ShouldMatcherReceiveReturnGenericParameter, true)
            .Build();
        var result = new MatchMethodBuilderDoc(@params).Build();

        Assert.Contains("""<see cref="Result{TData}"/>""", result);
        Assert.Contains("""<typeparam name="TResult">""", result);
        Assert.Contains("""<typeparam name="TData">""", result);
        Assert.Contains("""<see cref="ResultResponseSuccess{TData}"/>""", result);
        Assert.Contains("""returns a <typeparamref name="TResult"/>.""", result);
    }

    [Fact]
    public void Build_SyncResultsReturn_ResultWithMultipleTypeParams()
    {
        var @params = Params()
            .Set(p => p.ReturnTypeName, "Results<>")
            .Set(p => p.ReturnTypeGenericParameters, ["TResult1", "TResult2"])
            .Set(p => p.MethodSufixName, (string?)"Results")
            .Set(p => p.GenericConstraints,
                (IEnumerable<(string, IEnumerable<string>)>)
                [
                    ("TResult1", ["IResult", "IEndpointMetadataProvider"]),
                    ("TResult2", ["IResult", "IEndpointMetadataProvider"]),
                ])
            .Set(p => p.ShouldMatcherReceiveReturnGenericParameter, true)
            .Build();
        var result = new MatchMethodBuilderDoc(@params).Build();

        var expected =
"""
    /// <summary>
    /// Checks whether a <see cref="Result"/> is a success or failure,
    /// and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TResult1">
    /// One of the expected result types. Must implement
    /// <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.
    /// </typeparam>
    /// <typeparam name="TResult2">
    /// One of the expected result types. Must implement
    /// <see cref="IResult"/> and <see cref="IEndpointMetadataProvider"/>.
    /// </typeparam>
    /// <param name="result">
    /// The <see cref="Result"/> to evaluate.
    /// </param>
    /// <param name="onSuccess">
    /// Function to be executed in case of success.
    /// Receives a <see cref="ResultResponseSuccess"/> and returns a <see cref="Results{TResult1, TResult2}"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnSuccessDefault().
    /// </param>
    /// <param name="onFailure">
    /// Function to be executed in case of failure.
    /// Receives a <see cref="ResultResponseError"/> and returns a <see cref="Results{TResult1, TResult2}"/>.
    /// If null, it will execute a default function, configurable in GlobalConfiguration.SetOnFailureDefault().
    /// </param>
    /// <returns>
    /// Returns the result of the <paramref name="onSuccess"/> function in case of success,
    /// or of <paramref name="onFailure"/> in case of failure.
    /// </returns>
""";
        Assert.Equal(expected, result);
    }
}
