using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.UnitTests.ResultObjects;

public class ErrorTests
{
    private Error _error => _errors.First();
    private readonly List<Error> _errors = [
        new("Code1", "Description1", ErrorType.Failure),
        new("Code2", "Description2", ErrorType.Failure),
    ];
    private readonly List<Error.ErrorDetails> _errorDetails = [
        new("Code1", "Description1"),
        new("Code2", "Description2"),
    ];

    [Fact]
    public void CreateError_WithSingleDetail_ShouldInitializeCorrectly()
    {
        Assert.Single(_error.Details);
        var errorDetail = _error.Details.Single();
        Assert.Equal("Code1", errorDetail.Code);
        Assert.Equal("Description1", errorDetail.Description);
        Assert.Equal(ErrorType.Failure, _error.Type);
    }

    [Fact]
    public void CreateError_WithMultipleDetails_ShouldInitializeCorrectly()
    {        
        var error = new Error(_errorDetails, ErrorType.Validation);
        var errorDetails = error.Details.ToList();

        Assert.Equal(2, errorDetails.Count);

        Assert.Equal("Code1", errorDetails[0].Code);
        Assert.Equal("Description1", errorDetails[0].Description);

        Assert.Equal("Code2", errorDetails[1].Code);
        Assert.Equal("Description2", errorDetails[1].Description);

        Assert.Equal(ErrorType.Validation, error.Type);
    }

    [Fact]
    public void ImplicitConversion_FromErrorToResult_ShouldReturnFailureResult()
    {
        Result result = _error;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(_error, result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromListOfErrorsToError_ShouldCombineDetails()
    {
        Error combinedError = _errors;
        var errorDetails = combinedError.Details.ToList();

        Assert.Equal(2, combinedError.Details.Count);
        Assert.Equal(ErrorType.Failure, combinedError.Type);

        Assert.Equal("Code1", errorDetails[0].Code);
        Assert.Equal("Description1", errorDetails[0].Description);

        Assert.Equal("Code2", errorDetails[1].Code);
        Assert.Equal("Description2", errorDetails[1].Description);
    }


    [Fact]
    public void ImplicitConversion_FromListOfDistinctErrorTypesToError_ShouldThrowArgumentException()
    {
        var errors = new List<Error>
        {
            new("Code1", "Description1", ErrorType.Failure),
            new("Code2", "Description2", ErrorType.Validation),
        };

        var ex = Assert.Throws<ArgumentException>(() => { Error combinedError = errors; });
        Assert.Equal("All errors must be of the same type (Parameter 'errors')", ex.Message);
    }

    [Theory]
    [InlineData("Failure", ErrorType.Failure)]
    [InlineData("Validation", ErrorType.Validation)]
    [InlineData("NotFound", ErrorType.NotFound)]
    [InlineData("Conflict", ErrorType.Conflict)]
    public void StaticConstructor_ErrorConstructor_ShouldCreateFailureError(string constructorName, ErrorType expectedType)
    {
        var error = (Error)typeof(Error).GetMethod(constructorName)!.Invoke(null, ["Code1", "Description1"])!;
        var errorDetail = error.Details.Single();

        Assert.Equal("Code1", errorDetail.Code);
        Assert.Equal("Description1", errorDetail.Description);
        Assert.Equal(expectedType, error.Type);
    }

    [Fact]
    public void ListDescriptions_ShouldReturnAllDescriptions()
    {
        var error = new Error(_errorDetails, ErrorType.Validation);

        var descriptions = error.ListDescriptions();

        Assert.Equal(descriptions, ["Description1", "Description2"]);
    }

    [Fact]
    public void PredefinedError_None_ShouldBeInitializedCorrectly()
    {
        Assert.Equal(string.Empty, Error.None.Details.First().Code);
        Assert.Equal(string.Empty, Error.None.Details.First().Description);
        Assert.Equal(ErrorType.Failure, Error.None.Type);
    }

    // [Fact]
    // public void PredefinedError_NullValue_ShouldBeInitializedCorrectly()
    // {
    //     Assert.Equal("Error.NullValue", Error.NullValue.Details.First().Code);
    //     Assert.Equal("Null value was provided", Error.NullValue.Details.First().Description);
    //     Assert.Equal(ErrorType.Failure, Error.NullValue.Type);
    // }
}