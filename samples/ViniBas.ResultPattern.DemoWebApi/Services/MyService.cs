using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.DemoWebApi.Services;

public interface IMyService
{
    Result Post(int value);
    Result<string> Get(int value);
    Result<object> Custom(int value);
}

public sealed class MyService : IMyService
{
    public Result Post(int value)
    {
        if (value < 0)
            return Error.Failure("Err1", "Value must be greater than or equal to 0");

        var errors = new List<Error>();

        if (value % 2 == 0)
            errors.Add(Error.Validation("Err2", "Value must be odd"));

        if (value % 3 == 0)
            errors.Add(Error.Validation("Err3", "Value must not be divisible by 3"));

        if (errors.Any())
            return errors;

        return Result.Success();
        // Less semantic, but also possible:
        // return Error.None;
    }

    public Result<string> Get(int value)
    {
        if (value < 0)
            return Error.Failure("Err1", "Value must be greater than or equal to 0");

        var errors = new List<Error>();

        if (value % 2 == 0)
            errors.Add(Error.NotFound("Err2", "Value must be odd"));

        if (value % 3 == 0)
            errors.Add(Error.NotFound("Err3", "Value must not be divisible by 3"));

        if (errors.Any())
            return errors;

        // It can be returned explicitly, like:
        // return Result<string>.Success("Test Data");
        // Or more directly:
        return "Test Data";
    }

    public Result<object> Custom(int value)
    {
        if (value <= 0)
            return new Error("Err1", "Value must be greater than 0", "NotAcceptable");

        return new { Message = ":P" };
    }
}