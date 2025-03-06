/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.ResultResponses;

public record ResultResponseSuccess : ResultResponse
{
    protected ResultResponseSuccess()
        => IsSuccess = true;

    public static ResultResponseSuccess Create() => new();
    public static ResultResponseSuccess<T> Create<T>(T data) => ResultResponseSuccess<T>.Create(data);
}

public record ResultResponseSuccess<T> : ResultResponseSuccess
{
    private ResultResponseSuccess(T data) : base()
        => Data = data;

    public T Data { get; set; }

    public static ResultResponseSuccess<T> Create(T data) => new(data);
}
