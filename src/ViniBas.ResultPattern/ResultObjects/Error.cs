namespace ViniBas.ResultPattern.ResultObjects;

public sealed record Error
{
    public ICollection<ErrorDetails> Details { get; private set; }
    public ErrorType Type { get; private set; }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    // public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Details = [new (code, description)];
        Type = type;
    }

    public Error(ICollection<ErrorDetails> details, ErrorType type)
    {
        if (!details.Any()) throw new ArgumentException("Details cannot be empty", nameof(details));

        Details = details;
        Type = type;
    }

    public static implicit operator Result(Error error) => Result.Failure(error);
    public static implicit operator Error(List<Error> errors)
    {
        if (!errors.Any()) throw new ArgumentException("Errors cannot be empty", nameof(errors));
        var type = errors.First().Type;
        if (errors.Any(e => e.Type != type)) throw new ArgumentException("All errors must be of the same type", nameof(errors));

        return new Error(errors.SelectMany(e => e.Details).ToList(), type);
    }

    public static Error Failure(string code, string description)
        => new(code, description, ErrorType.Failure);
    public static Error Validation(string code, string description)
        => new(code, description, ErrorType.Validation);
    public static Error NotFound(string code, string description)
        => new(code, description, ErrorType.NotFound);
    public static Error Conflict(string code, string description)
        => new(code, description, ErrorType.Conflict);

    public IEnumerable<string> ListDescriptions()
        => Details.Select(d => d.Description).ToList();

    public sealed record ErrorDetails(string Code, string Description);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
}