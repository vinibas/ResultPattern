/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.ResultObjects;

public abstract record ResultBase
{
    public bool IsSuccess { get => Error == Error.None; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected ResultBase(Error error) => Error = error;
    
    public abstract ResultResponse ToActionResponse();
}

public record Result : ResultBase
{
    private Result(Error error) : base(error) { }

    public static Result Success() => new(Error.None);
    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(List<Error> errors) => Failure(errors);

    public override ResultResponse ToActionResponse()
        => IsSuccess ? ResultResponseSuccess.Create() : new ResultResponseError(Error.ListDescriptions(), Error.Type);
}

public record Result<T> : ResultBase
{
    private Result(T data) : base(Error.None)
        => Data = data;
    private Result(Error error) : base(error) { }

    public T? Data { get; }

    public static Result<T> Success(T item) => new(item);
    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(List<Error> errors) => Failure(errors);

    public override ResultResponse ToActionResponse()
        => IsSuccess ? ResultResponseSuccess.Create(Data!) : new ResultResponseError(Error.ListDescriptions(), Error.Type);

    public static implicit operator Result<T>(T item) => Success(item);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
