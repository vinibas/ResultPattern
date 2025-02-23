namespace ViniBas.ResultPattern.ResponseResults;

public record ResultResponseSuccess : ResultResponse
{
    public ResultResponseSuccess()
        => IsSuccess = true;

    public static ResultResponseSuccess Create() => new();
    public static ResultResponseSuccess<T> Create<T>(T data) => new(data);

}

public record ResultResponseSuccess<T> : ResultResponseSuccess
{
    public ResultResponseSuccess(T data) : base()
        => Data = data;

    public T Data { get; set; }
}
