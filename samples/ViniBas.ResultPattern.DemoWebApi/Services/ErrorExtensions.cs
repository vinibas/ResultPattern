/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.DemoWebApi.Services;

public static class CustomErrorHelper
{
    public const string NotAcceptableKey = "NotAcceptable";
    public const string NotAcceptableValue = "Not Acceptable";
    public static Error NotAcceptable(string code, string description)
        => new(code, description, NotAcceptableKey);
}
