/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.ResultResponses;

public record ResultResponseError : ResultResponse
{
    public ResultResponseError(IEnumerable<string> errors, string type)
    {
        IsSuccess = false;
        Errors = errors;
        Type = type;
    }

    public IEnumerable<string> Errors { get; private set; }
    public string Type { get; private set; }
}
