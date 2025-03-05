namespace ViniBas.ResultPattern.ResultResponses;

public abstract record ResultResponse
{
    public bool IsSuccess { get; protected set; }
}
