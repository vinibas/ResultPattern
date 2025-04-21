# ViniBas.ResultPattern

Copyright (c) Vinícius Bastos da Silva 2025
Licensed under the GNU Lesser General Public License v3 (LGPL v3).
See the LICENSE.txt file for details.

# Introduction

This project consists of two .NET libraries, available for use via NuGet, aimed at implementing the Result Pattern in a simple, clean, and effective way.

Usage is free, and the code is open-source under the LGPL v3 license. Check the LICENSE.txt file in the project root for more information.

The two libraries that make up this project are: ResultPattern, which is the core of the project, and ResultPattern.AspNet, which contains resources for use in ASP.NET projects. Below are instructions for each.

## ViniBas.ResultPattern

The ResultPattern library is the main library of the project. It contains the `Result` and `ResultResponse` objects and is independent of the project type: ASP.NET, Console, Class Library, etc. Since it has no external dependencies, it can even be used in business logic layers without contaminating your domain with infrastructure details.

The ResultPattern package contains two main object types: `Result` and `ResultResponse`. Let's discuss the importance of each:

### Result

The `Result` class is the most important class in the entire project. This is the class we actually use as the return type in our service methods, indicating success or failure. The `Result` class may or may not have an associated object. In this case, we use `Result<T>`, where `T` is the type of the return object, which will be available in the `Data` property in case of success.

Some implementations of the Result Pattern force verbose and hard-to-read returns, requiring object construction and redundant type declarations. In this library, you have more flexibility, allowing you to return either an `Error` object (or a collection of them) or the expected value in case of success, as in the following example:

```csharp
public Result<string> Get()
{
    if (somethingWrong)
        return Error.Failure("ErrorCode", "Error message!");

    if (severalThingsWrong)
        return new List<Error>()
        {
            Error.Validation("Error1", "Some validation 1"),
            Error.Validation("Error2", "Some validation 2"),
            Error.Validation("Error3", "Some validation 3"),
        };

    return "Return value on success.";
}
```

In the example above, we have a situation where we return a list with multiple errors. All of them will be combined into one; the only condition is that they must all have the same type, in this case, `Validation`.

There are four predefined error types, which can be created using static constructors: `Failure`, `Validation`, `NotFound`, and `Conflict`. You can create your own custom types by registering them once using `Error.ErrorTypes.AddTypes("MyNewErrorType");`, and then creating the error using the default constructor: `new Error("code", "message", "MyNewErrorType");`.

A method calling the example above could validate the result as follows:

```csharp
var result = Get();

if (result.IsSuccess)
    Console.WriteLine(result.Data);

if (result.IsFailure)
    foreach (var description in result.Error.ListDescriptions())
        Console.WriteLine(description);
```

If your method does not need to return any value besides the `Result` itself, you can use `Result` without `T`. In this case, since there is no value to return, you can simply return `Result.Success();`, which would also work with `T`: `return Result.Success("Return value on success.");`. Note that `Result` and `Result<T>` are different types, so you must return the type declared in your method signature.

### ResultResponse

The `ResultResponse` classes exist to make responses cleaner and more explicit, particularly in cases of JSON serialization. While the `Result` class is more complete for internal handling, `ResultResponse` classes represent the final result that can be returned to the user without implementation details. They consist of three classes: `ResultResponseError`, `ResultResponseSuccess`, and `ResultResponseSuccess<T>`, and can be automatically generated from `Result.ToResponse()`.

The structure of the `ResultResponse` classes always includes an `IsSuccess` property. In case of an error, there is a list of `Errors` (strings) and a `Type` (string) indicating the error type. In case of success, there may be a `Data` property containing the return value.

## ViniBas.ResultPattern.AspNet

The `ResultPattern.AspNet` library provides additional features for both ASP.NET Web API and Minimal API. The main feature is the `Match` method, which allows mapping API return functions for success (`onSuccess`) or failure (`onFailure`) cases. It is even possible to omit the failure method, which results in a `ProblemDetails` response with the appropriate status code for the error type in `Result`. This way, the developer does not need to manually map the status code for each error type in `Result`. For example, if `Result` contains `NotFound` errors, the returned status code will be `404`. For `Failure`, it will be `500`.

If the `OnFailure` parameter is omitted, the return may or may not be of the ProblemDetails type, depending on the configuration performed through `GlobalConfiguration.UseProblemDetails`, which can be done in your `program.cs`, with the value `true` by default. This configuration can be overridden when calling the Match method through the optional `useProblemDetails` parameter.

For custom error types, just as we can create new error types with `Error.ErrorTypes.AddTypes("MyNewErrorType");`, we can also create new mappings by modifying the `Maps` dictionary, for example:  
`GlobalConfiguration.ErrorTypeMaps.Add("MyNewErrorType", (StatusCodes.Status406NotAcceptable, "My New Error Type"));`, which can also be configured in `program.cs`.  
This way, the new type will always return the mapped status code, and the corresponding title will be used. It is also possible to modify existing mappings. If an error type is not pre-mapped, the default status code returned is `500`.

See an example of usage in an MVC action:

```csharp
[HttpGet("{value}")]
public IActionResult Get()
    => _myService.GetResult().Match(Ok);
```

In the example below, using Minimal API, we can return an `IResult` or `Results<>` for typed responses:

```C#
app.MapGet("IResult", ()
    => myService.Get().Match(Results.Ok));

app.MapGet("Results<>", ()
    => myService.Get().Match<Results<Ok, ProblemHttpResult>, Ok>(TypedResults.Ok));
```

Again, the second parameter is optional, so it is omitted in the examples, and may or may not return a ProblemDetails depending on the global configuration and the `useProblemDetails` parameter.

The `Problem Details` standard is based on [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807), and the returned object is native to ASP.NET, as described in the [official documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails).  
The returned `ProblemDetails` object includes the following additional properties:
- `isSuccess`: a fixed boolean with the value `false`;
- `errors`: the list of error descriptions.

Two filters are also included: `ActionResultFilter` for ASP.NET MVC and `ResultsResultFilter` for Minimal API. These filters ensure that if objects such as `Result`, `Error`, or `ResultResponse` are returned directly, they are properly converted, if necessary, into a `ResultResponseSuccess`, a `ResultResponseError`, or a `ProblemDetails`.

To facilitate validation in MVC applications, it is also possible to easily convert a `ModelState` into an `Error` object or an `IActionResult` with a `ProblemDetails` using the extension methods `ModelStateToError` and `ToProblemDetailsActionResult`.

# Demo

We have a demonstration ASP.NET project with some simple examples, which can be found in the `samples` folder of the repository at [https://github.com/vinibas/ResultPattern/](https://github.com/vinibas/ResultPattern/tree/master/samples).
