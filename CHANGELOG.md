# Changelog

## 2.0.0 - 2025-04-21

### Added
- Introduced the `GlobalConfiguration` class to centralize library-wide settings.

### Changed
- Moved `ErrorTypeMaps` into the `GlobalConfiguration` class for better organization and encapsulation.

### Improved
- Added the `GlobalConfiguration.UseProblemDetails` flag to control whether `ProblemDetails` should be returned on failures.
- Extended the `Match` methods that omit the `onFailure` parameter to accept a `useProblemDetails` boolean, allowing per-call customization while still honoring the global default.


## 1.0.0 - Initial release