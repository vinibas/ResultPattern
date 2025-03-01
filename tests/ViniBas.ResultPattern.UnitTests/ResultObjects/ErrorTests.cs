using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.UnitTests.ResultObjects;

public class ErrorTests
{
    private Error _error => _errors.First();
    private readonly List<Error> _errors = [
        Error.Failure("Code1", "Description1"),
        Error.Failure("Code2", "Description2"),
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
        Assert.Equal(ErrorTypes.Failure, _error.Type);
    }

    [Fact]
    public void CreateError_WithMultipleDetails_ShouldInitializeCorrectly()
    {        
        var error = new Error(_errorDetails, ErrorTypes.Validation);
        var errorDetails = error.Details.ToList();

        Assert.Equal(2, errorDetails.Count);

        Assert.Equal("Code1", errorDetails[0].Code);
        Assert.Equal("Description1", errorDetails[0].Description);

        Assert.Equal("Code2", errorDetails[1].Code);
        Assert.Equal("Description2", errorDetails[1].Description);

        Assert.Equal(ErrorTypes.Validation, error.Type);
    }

    [Fact]
    public void CreateError_WithNewType_ShouldInitializeCorrectly()
    {
        Error.ErrorTypes.AddTypes("NewType");
        var error = new Error("Code1", "Description1", "NewType");
        
        Assert.Single(error.Details);
        var errorDetail = error.Details.Single();
        Assert.Equal("Code1", errorDetail.Code);
        Assert.Equal("Description1", errorDetail.Description);
        Assert.Equal("NewType", error.Type);
    }

    [Fact]
    public void CreateError_WithSingleDetailAndUnexpectedType_ShouldThrowArgumentException()
    {
        var errorDetail = _errorDetails.First();
        var ex = Assert.Throws<ArgumentException>(() => new Error(errorDetail.Code, errorDetail.Description, "UnexpectedType"));
        Assert.Equal("Type not defined in ErrorTypes. (Parameter 'type')", ex.Message);
    }

    [Fact]
    public void CreateMultipleError_WithMultipleDetailsAndUnexpectedType_ShouldThrowArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Error(_errorDetails, "UnexpectedType"));
        Assert.Equal("Type not defined in ErrorTypes. (Parameter 'type')", ex.Message);
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
    public void ImplicitConversion_FromErrorNoneToResult_ShouldReturnSuccessResult()
    {
        Result result = Error.None;

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void ImplicitConversion_FromListOfErrorsToError_ShouldCombineDetails()
    {
        Error combinedError = _errors;
        var errorDetails = combinedError.Details.ToList();

        Assert.Equal(2, combinedError.Details.Count);
        Assert.Equal(ErrorTypes.Failure, combinedError.Type);

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
            new("Code1", "Description1", ErrorTypes.Failure),
            new("Code2", "Description2", ErrorTypes.Validation),
        };

        var ex = Assert.Throws<ArgumentException>(() => { Error combinedError = errors; });
        Assert.Equal("All errors must be of the same type (Parameter 'errors')", ex.Message);
    }

    [Theory]
    [InlineData(ErrorTypes.Failure)]
    [InlineData(ErrorTypes.Validation)]
    [InlineData(ErrorTypes.NotFound)]
    [InlineData(ErrorTypes.Conflict)]
    public void StaticConstructor_ErrorConstructor_ShouldCreateFailureError(string errorType)
    {
        var error = (Error)typeof(Error).GetMethod(errorType)!.Invoke(null, ["Code1", "Description1"])!;
        var errorDetail = error.Details.Single();

        Assert.Equal("Code1", errorDetail.Code);
        Assert.Equal("Description1", errorDetail.Description);
        Assert.Equal(errorType, error.Type);
    }

    [Fact]
    public void ListDescriptions_ShouldReturnAllDescriptions()
    {
        var error = new Error(_errorDetails, ErrorTypes.Validation);

        var descriptions = error.ListDescriptions();

        Assert.Equal(descriptions, ["Description1", "Description2"]);
    }

    [Fact]
    public void PredefinedError_None_ShouldBeInitializedCorrectly()
    {
        Assert.Equal(string.Empty, Error.None.Details.First().Code);
        Assert.Equal(string.Empty, Error.None.Details.First().Description);
        Assert.Equal(ErrorTypes.Failure, Error.None.Type);
    }
}