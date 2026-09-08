namespace PersonalWealth.Application.Results;

public sealed record ApplicationError
{
    public ApplicationError(string code, string message)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Error code must not be empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Error message must not be empty.", nameof(message));
        }

        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    public static ApplicationError Validation(string message) =>
        new("validation", message);

    public static ApplicationError Conflict(string message) =>
        new("conflict", message);

    public static ApplicationError NotFound(string message) =>
        new("not_found", message);
}

public sealed class Result<T>
{
    private readonly T? value;
    private readonly ApplicationError? error;

    private Result(T value)
    {
        this.value = value;
        IsSuccess = true;
    }

    private Result(ApplicationError error)
    {
        this.error = error;
        IsSuccess = false;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("A failed result does not contain a value.");

    public ApplicationError Error => !IsSuccess
        ? error!
        : throw new InvalidOperationException("A successful result does not contain an error.");

    public static Result<T> Success(T value) =>
        new(value);

    public static Result<T> Failure(ApplicationError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }
}
