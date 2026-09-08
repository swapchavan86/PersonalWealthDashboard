using PersonalWealth.Application.Results;
using Xunit;

namespace PersonalWealth.UnitTests.Application.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_contains_the_value_and_no_failure_state()
    {
        var result = Result<string>.Success("complete");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("complete", result.Value);
    }

    [Fact]
    public void Failure_contains_structured_error_information()
    {
        var error = ApplicationError.Validation("The request is invalid.");
        var result = Result<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Error_contains_a_stable_code_and_human_readable_message()
    {
        var error = ApplicationError.Conflict("The resource already exists.");

        Assert.Equal("conflict", error.Code);
        Assert.Equal("The resource already exists.", error.Message);
    }

    [Fact]
    public void Common_error_semantics_have_framework_independent_codes()
    {
        Assert.Equal("validation", ApplicationError.Validation("invalid").Code);
        Assert.Equal("conflict", ApplicationError.Conflict("duplicate").Code);
        Assert.Equal("not_found", ApplicationError.NotFound("missing").Code);
    }

    [Fact]
    public void Result_and_error_state_are_immutable()
    {
        var result = Result<string>.Success("complete");
        var error = ApplicationError.NotFound("missing");

        Assert.All(typeof(Result<string>).GetProperties(), property => Assert.Null(property.SetMethod));
        Assert.All(typeof(ApplicationError).GetProperties(), property => Assert.Null(property.SetMethod));
        Assert.Equal("complete", result.Value);
        Assert.Equal("missing", error.Message);
    }

    [Fact]
    public void Invalid_error_data_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new ApplicationError("", "message"));
        Assert.Throws<ArgumentException>(() => new ApplicationError("code", ""));
    }

    [Fact]
    public void Inconsistent_value_or_error_access_is_rejected()
    {
        var success = Result<string>.Success("complete");
        var failure = Result<string>.Failure(ApplicationError.NotFound("missing"));

        Assert.Throws<InvalidOperationException>(() => success.Error);
        Assert.Throws<InvalidOperationException>(() => failure.Value);
    }

    [Fact]
    public void Null_failure_error_is_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
    }
}
