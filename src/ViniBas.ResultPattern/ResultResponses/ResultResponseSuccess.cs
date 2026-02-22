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
    public static ResultResponseSuccess<TData> Create<TData>(TData data) => ResultResponseSuccess<TData>.Create(data);
}

public record ResultResponseSuccess<TData> : ResultResponseSuccess
{
    private ResultResponseSuccess(TData data) : base()
        => Data = data;

    public TData Data { get; set; }

    public static ResultResponseSuccess<TData> Create(TData data) => new(data);
}
