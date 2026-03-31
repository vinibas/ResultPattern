# ViniBas.ResultPattern

Copyright (c) Vinícius Bastos da Silva 2025-2026  
Licensed under the GNU Lesser General Public License v3 (LGPL v3).  
See the LICENSE.txt file for details.

## What is the Result Pattern?

The Result Pattern is a functional approach to error handling where methods return a `Result` object instead of throwing exceptions. This object explicitly indicates success or failure, carrying either the expected value or error details. It leads to cleaner, more predictable code — no hidden control flows, no swallowed exceptions, no guessing what a method can return.

## About this library

**ViniBas.ResultPattern** brings the Result Pattern to .NET with two packages:

- **ViniBas.ResultPattern** — Core library with `Result`, `Result<T>`, `Error`, and `ResultResponse` types. No external dependencies, so it can be used in any layer (domain, application, infrastructure) and in any type of project — ASP.NET, Console, WPF, etc.
- **ViniBas.ResultPattern.AspNet** — ASP.NET integration with `Match` extensions that automatically map results to HTTP responses (`IActionResult`, `IResult`, or typed `Results<>`), filters, ProblemDetails support, and more.

The library is **free and open-source** (LGPL v3). The goal is to be **simple, clean, and effective**: reduce verbosity, eliminate manual status code mapping with the `Match` API, and handle edge cases like ProblemDetails formatting and error type registration out of the box.

## Installation

```bash
dotnet add package ViniBas.ResultPattern --version 3.0.0
dotnet add package ViniBas.ResultPattern.AspNet --version 3.0.0
```

> **Important:** Starting from version 3.0.0, both packages follow the same versioning. When installing or updating, always use the same version for both packages, since `ViniBas.ResultPattern.AspNet` depends on `ViniBas.ResultPattern`.

---

## ViniBas.ResultPattern (Core)

### Result and Result\<TData\>

`Result` and `Result<TData>` are the types you use as return values in your service methods. They indicate success or failure, and in the generic case, carry a `Data` property with the return value.

You can return an `Error`, a `List<Error>`, or the value directly — no need for verbose construction:

```csharp
public Result<UserModel> GetUser(string name)
{
    var user = _users.FirstOrDefault(u => u.Name == name);

    if (user == null)
        return Error.NotFound("UserNotFound", "User not found");

    return user; // Implicit conversion to Result<UserModel>
}
```

For methods that don't return a value, use `Result` (non-generic):

```csharp
public Result SaveNewUser(UserModel user)
{
    if (string.IsNullOrWhiteSpace(user.Name))
        return Error.Validation("Err1", "Name cannot be empty");

    _users.Add(user);
    return Result.Success();
}
```

You can also accumulate multiple errors of the same type:

```csharp
public Result SaveNewUser(UserModel user)
{
    var validationErrors = new List<Error>();

    if (string.IsNullOrWhiteSpace(user.Name))
        validationErrors.Add(Error.Validation("Err1", "Name cannot be empty"));

    if (user.Age < 18)
        validationErrors.Add(Error.Validation("Err2", "Age must be at least 18"));

    if (validationErrors.Any())
        return validationErrors; // Implicit conversion — errors are merged

    _users.Add(user);
    return Result.Success();
}
```

Checking the result:

```csharp
var result = GetUser("Alice");

if (result.IsSuccess)
    Console.WriteLine(result.Data.Name);

if (result.IsFailure)
    foreach (var description in result.Error.ListDescriptions())
        Console.WriteLine(description);
```

### Error

The `Error` type encapsulates a list of `ErrorDetails` (code + description) and a `Type` string. Built-in error types and their factory methods:

| Factory Method | Type | Default HTTP Status |
|---|---|---|
| `Error.Failure(code, description)` | `Failure` | 500 |
| `Error.Validation(code, description)` | `Validation` | 400 |
| `Error.NotFound(code, description)` | `NotFound` | 404 |
| `Error.Conflict(code, description)` | `Conflict` | 409 |
| `Error.Unauthorized(code, description)` | `Unauthorized` | 401 |
| `Error.Forbidden(code, description)` | `Forbidden` | 403 |

You can create custom error types by registering them and, optionally, mapping them to an HTTP status code:

```csharp
Error.ErrorTypes.AddTypes("NotAcceptable");
GlobalConfiguration.ErrorTypeMaps.TryAdd("NotAcceptable", (StatusCodes.Status406NotAcceptable, "Not Acceptable"));
```

Then create instances using the constructor:

```csharp
var error = new Error("ErrCode", "Description", "NotAcceptable");
```

### ResultResponse

The `ResultResponse` family (`ResultResponseSuccess`, `ResultResponseSuccess<TData>`, `ResultResponseError`) are DTOs designed for HTTP transport. They are generated automatically from `result.ToResponse()` and contain a clean, serializable representation of the result — without implementation details.

- **Success**: `IsSuccess = true`, optionally with `Data`.
- **Error**: `IsSuccess = false`, with `Errors` (list of `ErrorDetails`) and `Type`.

---

## ViniBas.ResultPattern.AspNet

### Match Extensions

The core feature of the ASP.NET package is the `Match` method, available as extension methods on `Result`, `Result<TData>`, and `ResultResponse`. It maps the result to the appropriate HTTP response by accepting two optional callbacks: `onSuccess` and `onFailure`. Both can be omitted — when they are, the library uses built-in fallback behavior (see [Fallback Behavior](#fallback-behavior)).

#### MVC (IActionResult)

```csharp
using ViniBas.ResultPattern.AspNet.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    // Provide both onSuccess and onFailure
    [HttpGet("health/{alive}")]
    public IActionResult Health(bool alive)
        => _userService.Health(alive).Match(Ok, BadRequest);

    // Omit onFailure — the library returns the appropriate status automatically
    [HttpGet("{name}")]
    public IActionResult Get(string name)
        => _userService.GetUserByName(name).Match(r => Ok(r.Data));

    // Omit both — defaults to Ok on success, automatic error handling on failure
    [HttpPost]
    public IActionResult Create(UserModel user)
        => _userService.SaveNewUser(user).Match();
}
```

#### Minimal API — Generic (IResult)

```csharp
using ViniBas.ResultPattern.AspNet.MinimalApi;

app.MapGet("health/{alive}",
    (bool alive) => _userService.Health(alive).Match(Results.Ok, Results.BadRequest));

app.MapGet("{name}",
    (string name) => _userService.GetUserByName(name).Match(r => Results.Ok(r.Data)));

app.MapPost("",
    (UserModel user) => _userService.SaveNewUser(user).Match());
```

When using the generic `Match` in Minimal API, the return type is `IResult`. This means ASP.NET **does not infer the response metadata automatically** — you need to add it manually with `.Produces()` and `.ProducesProblem()` for OpenAPI documentation:

```csharp
app.MapGet("{name}",
    (string name) => _userService.GetUserByName(name).Match(r => Results.Ok(r.Data)))
    .Produces<UserModel>(200)
    .ProducesProblem(404);
```

#### Minimal API — Typed

For endpoints where you want ASP.NET to **automatically infer response metadata** for OpenAPI, use the typed match extensions. The framework knows all possible response types at build time, so documentation is generated automatically.

There are two variants:

**`Match<TResult>`** — Returns a single typed result. Useful when you know only one response type will be returned (e.g., `Ok<UserModel>`):

```csharp
app.MapGet("{name}", (string name) =>
    _userService.GetUserByName(name)
        .Match<Ok<UserModel>, UserModel>(r => TypedResults.Ok(r.Data)));
```

**`MatchResults`** — Returns a `Results<T1, T2, ...>` union type, for endpoints with multiple possible response types:

```csharp
using ViniBas.ResultPattern.AspNet.MinimalApi;

app.MapGet("health/{alive}", (bool alive) =>
    _userService.Health(alive)
        .MatchResults<Ok<ResultResponseSuccess>, BadRequest<ResultResponseError>>
            (r => TypedResults.Ok(r), r => TypedResults.BadRequest(r)));

app.MapGet("{name}", (string name) =>
    _userService.GetUserByName(name)
        .MatchResults<Ok<UserModel>, NotFound<ProblemDetails>, UserModel>
            (r => TypedResults.Ok(r.Data)));

// Omit both callbacks — the library maps the result to the appropriate typed response
app.MapPost("", (UserModel user) =>
    _userService.SaveNewUser(user)
        .MatchResults<Ok<ResultResponseSuccess>, BadRequest<ResultResponseError>, Conflict<ResultResponseError>>());
```

**Trade-off:** With typed results, you lose compile-time validation of return types (since the library internally casts `IResult` to the declared `Results<>` type). In exchange, you get **runtime validation**: if a result doesn't match any of the declared types, a `TypedResultCastException` is thrown with a descriptive message showing the expected vs. actual types.

To ensure this doesn't cause errors in production, register the `TypedResultCastExceptionHandler`. It's an `IExceptionHandler` that catches `TypedResultCastException`, logs the issue, and returns the original `IResult` instead of letting it propagate as a 500 error:

```csharp
// In Program.cs — register the handler and the exception handling middleware
builder.Services.AddExceptionHandler<TypedResultCastExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsProduction())
    app.UseExceptionHandler();
```

All `Match` and `MatchResults` extensions also have async variants (`MatchAsync` / `MatchResultsAsync`).

### Fallback Behavior

When `onSuccess` or `onFailure` callbacks are omitted (or passed as `null`), the library uses built-in fallback logic:

**Success fallback (`onSuccess` omitted):**

- If `UnwrapSuccessData` is `false` (default): returns `200 OK` with the full `ResultResponseSuccess` wrapper.
- If `UnwrapSuccessData` is `true`: returns `200 OK` with `Data` directly for `Result<TData>`, or an empty `200 OK` / `204 No Content` for `Result`.

**Failure fallback (`onFailure` omitted):**

- If `UseProblemDetails` is `true` (default): returns an RFC 7807 `ProblemDetails` response with the HTTP status code mapped from the error type.
- If `UseProblemDetails` is `false`: returns the raw `ResultResponseError` (or just the `Errors` list when `UnwrapSuccessData` is `true`) with the mapped status code.

For typed match extensions (`Match<TResult>` and `MatchResults`), the fallback uses `GlobalConfiguration.TypedResultMaps` to determine which typed result wrapper to produce for each HTTP status code. The default mappings cover common status codes (`Ok`, `Created`, `NoContent`, `BadRequest`, `NotFound`, `Conflict`, `UnprocessableEntity`, `Unauthorized`, `Forbid`, and `Failure` on .NET 9+). You can add or replace entries to customize the typed result for any status code:

```csharp
GlobalConfiguration.TypedResultMaps[StatusCodes.Status418ImATeapot] = TypedResultBuilders.Json(418);
```

If you need full control, you can replace the entire fallback logic per return type using `FallbackOverrides`:

```csharp
GlobalConfiguration.FallbackOverrides.Mvc = (resultResponse, context) =>
{
    // Custom fallback logic for MVC (IActionResult) endpoints
    // context.UseProblemDetails and context.UnwrapSuccessData reflect the active configuration
};

GlobalConfiguration.FallbackOverrides.MinimalApi = (resultResponse, context) =>
{
    // Custom fallback logic for Minimal API (IResult) endpoints
};
```

### Global Configuration

Configure the library behavior globally in `Program.cs`:

```csharp
using ViniBas.ResultPattern.AspNet.Configurations;

// Return ProblemDetails on failure (default: true)
GlobalConfiguration.UseProblemDetails = true;

// When true, success responses return Data directly instead of the ResultResponseSuccess wrapper (default: false)
GlobalConfiguration.UnwrapSuccessData = false;
```

You can also provide a custom ProblemDetails factory:

```csharp
GlobalConfiguration.ProblemDetailsOverride = error => new ProblemDetails
{
    Title = "Custom title",
    Status = 500,
    Detail = string.Join(", ", error.Errors.Select(e => e.Description))
};
```

### Scoped Configuration

Override global settings for a specific scope using `ScopedConfiguration`:

```csharp
using ViniBas.ResultPattern.AspNet.Configurations;

[HttpPost]
public IActionResult Create(UserModel user)
{
    using (ScopedConfiguration.Override(useProblemDetails: false, unwrapSuccessData: true))
    {
        return _userService.SaveNewUser(user).Match();
    }
}
```

The override applies only within the `using` block and is automatically restored afterward. It works per async context, so concurrent requests are not affected.

### ProblemDetails

When `UseProblemDetails` is `true` (default), failure responses follow the [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) standard using ASP.NET's native [ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails). The response includes:

- `title`: Mapped from `GlobalConfiguration.ErrorTypeMaps` (e.g., "Not Found")
- `status`: HTTP status code mapped from the error type
- `detail`: Error descriptions
- `extensions.isSuccess`: `false`
- `extensions.errors`: List of `ErrorDetails` (code + description)
- `extensions.descriptions`: List of error description strings

### Filters

Two filters are included to automatically convert `Result`, `Error`, or `ResultResponse` objects returned directly from endpoints into proper HTTP responses:

**MVC:**
```csharp
builder.Services.AddControllers(opt => opt.Filters.Add<ResponseMappingFilter>());
```

**Minimal API:**
```csharp
app.MapDelete("{name}", (string name) => _userService.Delete(name))
    .WithResponseMappingFilter();

// Or without the extension method:
app.MapDelete("{name}", handler)
    .AddEndpointFilter<ResponseMappingEndpointFilter>();
```

With the filter registered, you can return `Result` directly from your endpoint without calling `Match`:

```csharp
[HttpDelete("{userName}")]
public Result Delete(string userName)
    => _userService.HardDeleteUser(userName);
```

### ModelState Extensions (MVC)

Convert `ModelStateDictionary` to `Error` or directly to an `IActionResult` with ProblemDetails:

```csharp
if (!ModelState.IsValid)
    return ModelState.ToProblemDetailsActionResult();

// Or get the Error object for further handling
Error error = ModelState.ModelStateToError();
```

---

## Demo

A demonstration ASP.NET project with examples for MVC, Minimal API (generic), and Minimal API (typed) can be found in the [`samples`](https://github.com/vinibas/ResultPattern/tree/master/samples) folder of the repository.

## Target Frameworks

Both packages support: `net8.0`, `net9.0`, `net10.0`.

## License

LGPL v3 — see [LICENSE.txt](LICENSE.txt) for details.
