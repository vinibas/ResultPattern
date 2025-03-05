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
