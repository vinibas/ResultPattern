using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.ResponseResults;

public record ResultResponseError : ResultResponse
{
    public ResultResponseError(IEnumerable<string> errors, ErrorType type)
    {
        IsSuccess = false;
        Errors = errors;
        Type = type;
    }

    public IEnumerable<string> Errors { get; private set; }
    public ErrorType Type { get; private set; }
}
