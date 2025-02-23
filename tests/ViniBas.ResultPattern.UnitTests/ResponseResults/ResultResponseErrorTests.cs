using ViniBas.ResultPattern.ResponseResults;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.UnitTests.ResponseResults;

public class ResultResponseErrorTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var errors = new List<string> { "Error 1", "Error 2" };
        var errorType = ErrorType.Validation;

        var resultResponseError = new ResultResponseError(errors, errorType);

        Assert.False(resultResponseError.IsSuccess);
        Assert.Equal(errors, resultResponseError.Errors);
        Assert.Equal(errorType, resultResponseError.Type);
    }
}
