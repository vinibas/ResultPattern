namespace ViniBas.ResultPattern.ResultResponses;

public record ResultResponseSuccess : ResultResponse
{
    protected ResultResponseSuccess()
        => IsSuccess = true;

    public static ResultResponseSuccess Create() => new();
    public static ResultResponseSuccess<T> Create<T>(T data) => ResultResponseSuccess<T>.Create(data);
}

public record ResultResponseSuccess<T> : ResultResponseSuccess
{
    private ResultResponseSuccess(T data) : base()
        => Data = data;

    public T Data { get; set; }

    public static ResultResponseSuccess<T> Create(T data) => new(data);
}
