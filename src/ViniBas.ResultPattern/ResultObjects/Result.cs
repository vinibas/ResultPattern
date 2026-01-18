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
    public Error Error { get; private set; }

    protected ResultBase(Error error) => Error = error;
    
    public abstract ResultResponse ToResponse();

    public void AddError(Error newError)
    {
        try
        {
            Error = Error == Error.None ? newError : new List<Error>() { Error, newError };
        }
        catch (ArgumentException ex) when (ex.Message == "All errors must be of the same type (Parameter 'errors')")
        {
            throw new InvalidOperationException("The new error must be of the same type as the existing one.", ex);
        }
    }
}

public record Result : ResultBase
{
    private Result(Error error) : base(error) { }

    public static Result Success() => new(Error.None);
    public static Result<T> Success<T>(T item) => Result<T>.Success(item);
    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(List<Error> errors) => (Error)errors;

    public override ResultResponse ToResponse()
        => IsSuccess ? ResultResponseSuccess.Create() : ResultResponseError.Create(Error.Details, Error.Type);
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

    public override ResultResponse ToResponse()
        => IsSuccess ? ResultResponseSuccess.Create(Data!) : ResultResponseError.Create(Error.Details, Error.Type);
    
    public static implicit operator Result<T>(T item) => Success(item);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
