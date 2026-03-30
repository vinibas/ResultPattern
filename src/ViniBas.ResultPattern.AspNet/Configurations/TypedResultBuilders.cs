/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http;

namespace ViniBas.ResultPattern.AspNet.Configurations;

/// <summary>
/// Creates a typed <see cref="IResult"/> for a specific HTTP status code.
/// Implementations preserve the generic <typeparamref name="TBody"/> so that
/// the returned result (e.g. <c>NotFound&lt;TBody&gt;</c>) carries the correct
/// type argument for <c>Results&lt;...&gt;</c> union type compatibility and OpenAPI metadata.
/// </summary>
public interface ITypedResultBuilder
{
    /// <summary>
    /// Creates an <see cref="IResult"/> wrapping the given <paramref name="body"/>.
    /// </summary>
    IResult Build<TBody>(TBody body);

    /// <summary>
    /// Creates an <see cref="IResult"/> without a body payload.
    /// </summary>
    IResult Build();
}

/// <summary>
/// Provides pre-built <see cref="ITypedResultBuilder"/> instances for common HTTP status codes.
/// Use these to populate or customize <see cref="GlobalConfiguration.TypedResultMaps"/>.
/// </summary>
public static class TypedResultBuilders
{
    public static ITypedResultBuilder Ok { get; } = new OkBuilder();
    public static ITypedResultBuilder Created { get; } = new CreatedBuilder();
    public static ITypedResultBuilder NoContent { get; } = new NoContentBuilder();
    public static ITypedResultBuilder BadRequest { get; } = new BadRequestBuilder();
    public static ITypedResultBuilder NotFound { get; } = new NotFoundBuilder();
    public static ITypedResultBuilder Conflict { get; } = new ConflictBuilder();
    public static ITypedResultBuilder UnprocessableEntity { get; } = new UnprocessableEntityBuilder();
    public static ITypedResultBuilder Unauthorized { get; } = new UnauthorizedBuilder();
    public static ITypedResultBuilder Forbid { get; } = new ForbidBuilder();
    #if NET9_0_OR_GREATER
    public static ITypedResultBuilder Failure { get; } = new FailureBuilder();
    #endif

    /// <summary>
    /// Creates a builder that wraps the body in a <c>JsonHttpResult</c> with the specified status code.
    /// Use this for status codes that don't have a dedicated <c>TypedResults</c> method.
    /// </summary>
    public static ITypedResultBuilder Json(int statusCode) => new JsonBuilder(statusCode);

    private sealed class OkBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.Ok(body);
        public IResult Build() => TypedResults.Ok();
    }

    private sealed class CreatedBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.Created(null as string, body);
        public IResult Build() => TypedResults.Created();
    }

    private sealed class NoContentBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.NoContent();
        public IResult Build() => TypedResults.NoContent();
    }

    private sealed class BadRequestBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.BadRequest(body);
        public IResult Build() => TypedResults.BadRequest();
    }

    private sealed class NotFoundBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.NotFound(body);
        public IResult Build() => TypedResults.NotFound();
    }

    private sealed class ConflictBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.Conflict(body);
        public IResult Build() => TypedResults.Conflict();
    }

    private sealed class UnprocessableEntityBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.UnprocessableEntity(body);
        public IResult Build() => TypedResults.UnprocessableEntity();
    }

    private sealed class UnauthorizedBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.Unauthorized();
        public IResult Build() => TypedResults.Unauthorized();
    }

    private sealed class ForbidBuilder : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.Forbid();
        public IResult Build() => TypedResults.Forbid();
    }

    private sealed class JsonBuilder(int statusCode) : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.Json(body, statusCode: statusCode);
        public IResult Build() => TypedResults.StatusCode(statusCode);
    }

    #if NET9_0_OR_GREATER
    private sealed class FailureBuilder() : ITypedResultBuilder
    {
        public IResult Build<TBody>(TBody body) => TypedResults.InternalServerError(body);
        public IResult Build() => TypedResults.InternalServerError();
    }
    #endif
}
