# Changelog

## 3.0.0 - 2026-04-03

Starting from this version, both packages (`ViniBas.ResultPattern` and `ViniBas.ResultPattern.AspNet`) follow the same versioning.

### Added

#### Core
- New error types: `Unauthorized` and `Forbidden`, with factory methods on `Error`.
- `ResultResponseError` now stores a list of `ErrorDetails` instead of plain strings.
- Support for `.NET 10`. Dropped `.NET 7`.

#### AspNet
- `ScopedConfiguration`: per-async-scope overrides for `UseProblemDetails` and `UnwrapSuccessData` (replaces the `useProblemDetails` parameter on Match methods).
- `GlobalConfiguration.UnwrapSuccessData` flag: when `true`, success fallbacks return `Data` directly instead of the `ResultResponseSuccess` wrapper.
- `GlobalConfiguration.ProblemDetailsOverride`: custom factory for `ProblemDetails` generation.
- `GlobalConfiguration.FallbackOverrides`: fully replace the built-in Match fallback logic for MVC and/or Minimal API.
- `GlobalConfiguration.TypedResultMaps`: maps HTTP status codes to `ITypedResultBuilder` for typed result fallback responses.
- `TypedResultBuilders`: pre-built builders (`Ok`, `Created`, `NoContent`, `BadRequest`, `NotFound`, `Conflict`, `UnprocessableEntity`, `Unauthorized`, `Forbid`, `Failure`) and `Json(statusCode)` for custom status codes.
- Typed Match extensions for Minimal API: `Match<TResult>` (single typed result) and `MatchResults` (union `Results<T1, T2, ...>`), with automatic OpenAPI metadata inference.
- `MatchAsync` methods for all Match variants.
- `TypedResultCastException` with descriptive messages for typed result cast mismatches.
- `TypedResultCastExceptionHandler` (`IExceptionHandler`): catches `TypedResultCastException` in production and returns the original `IResult` as fallback.
- `ProblemDetails` extensions now include `descriptions` in the response extensions.
- Build-time code generation via `BuildTimeScript` for generating Match extension classes.

### Changed
- `ResultResponseError` public constructor removed; creation only via `Create` method.
- `ErrorTypeMaps` changed from `Dictionary` to `ConcurrentDictionary`.
- Filters renamed: `ActionResultFilter` → `ResponseMappingFilter`, `ResultsResultFilter` → `ResponseMappingEndpointFilter`, `WithResultsResultFilter` → `WithResponseMappingFilter`.
- Match extension classes regenerated via `BuildTimeScript` with both `onSuccess` and `onFailure` as optional parameters (previously `onSuccess` was required).
- Solution migrated to `.slnx` format.

## 2.0.0 - 2025-04-21 (AspNet only)

> This version was released only for `ViniBas.ResultPattern.AspNet`. The core package remained at 1.0.0.

### Added
- `GlobalConfiguration` class to centralize library-wide settings.
- `GlobalConfiguration.UseProblemDetails` flag to control whether `ProblemDetails` should be returned on failures.

### Changed
- Moved `ErrorTypeMaps` into `GlobalConfiguration`.
- `Match` methods that omit `onFailure` accept a `useProblemDetails` parameter for per-call customization.

## 1.0.0 - 2025-03-05

Initial release of both packages.
