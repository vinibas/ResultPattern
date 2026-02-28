/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.AspNet.BuildTimeScript.Builders.MatchMethod;

internal sealed class MatchMethodBuilder
{
    private readonly MatchMethodBuilderCode _builderCode;
    private readonly MatchMethodBuilderDoc _builderDoc;

    public MatchMethodBuilder(MatchMethodBuilderBase.MatchMethodBuilderParameters matchMethodBuilderParameters)
    {
        _builderCode = new MatchMethodBuilderCode(matchMethodBuilderParameters);
        _builderDoc = new MatchMethodBuilderDoc(matchMethodBuilderParameters);
    }
    public string Build()
        => _builderDoc.Build() + Environment.NewLine + _builderCode.Build();
}
