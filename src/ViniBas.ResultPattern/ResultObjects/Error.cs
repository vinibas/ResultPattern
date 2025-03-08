/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

namespace ViniBas.ResultPattern.ResultObjects;

public sealed record Error
{
    public ICollection<ErrorDetails> Details { get; private set; }
    public static ErrorTypes ErrorTypes { get; } = new ErrorTypes();
    public string Type { get; private set; }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorTypes.Failure);

    public Error(string code, string description, string type)
    {
        if (!ErrorTypes.Types.Contains(type))
            throw new ArgumentException("Type not defined in ErrorTypes.", nameof(type));
        
        Details = new ErrorDetails[] { new (code, description) };
        Type = type;
    }

    public Error(ICollection<ErrorDetails> details, string type)
    {
        if (!details.Any()) throw new ArgumentException("Details cannot be empty", nameof(details));

        if (!ErrorTypes.Types.Contains(type))
            throw new ArgumentException("Type not defined in ErrorTypes.", nameof(type));
        
        Details = details;
        Type = type;
    }

    public static implicit operator Result(Error error) 
        => error == None ? Result.Success() : Result.Failure(error);
    
    public static implicit operator Error(List<Error> errors)
    {
        if (!errors.Any()) throw new ArgumentException("Errors cannot be empty", nameof(errors));
        var type = errors.First().Type;
        if (errors.Any(e => e.Type != type)) throw new ArgumentException("All errors must be of the same type", nameof(errors));

        return new Error(errors.SelectMany(e => e.Details).ToList(), type);
    }

    public static Error Failure(string code, string description)
        => new(code, description, ErrorTypes.Failure);
    public static Error Validation(string code, string description)
        => new(code, description, ErrorTypes.Validation);
    public static Error NotFound(string code, string description)
        => new(code, description, ErrorTypes.NotFound);
    public static Error Conflict(string code, string description)
        => new(code, description, ErrorTypes.Conflict);

    public IEnumerable<string> ListDescriptions()
        => Details.Select(d => d.Description).ToList();

    public override string ToString() => string.Join(Environment.NewLine, Details.Select(d => d.ToString()));

    public sealed record ErrorDetails(string Code, string Description)
    {
        public override string ToString() => $"({Code}): {Description}";
    };
}

public sealed class ErrorTypes
{
    public const string Failure = "Failure";
    public const string Validation = "Validation";
    public const string NotFound = "NotFound";
    public const string Conflict = "Conflict";
    
    public HashSet<string> _types = new ()
    {
        Failure,
        Validation,
        NotFound,
        Conflict,
    };
    public IReadOnlySet<string> Types { get => _types; }

    internal ErrorTypes() { }

    public void AddTypes(params string[] newTypes)
    {
        foreach (var newType in newTypes)
            _types.Add(newType);
    }
}
