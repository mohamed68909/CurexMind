
public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new ArgumentException("Invalid arguments for Result constructor");
        IsSuccess = isSuccess;
        Error = error;
    }
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; set; } = default!;
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);

}
public class Result<T>(T? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
    private readonly T? _value = value;
    public T Value => IsSuccess ?
        _value! :
        throw new InvalidOperationException("Cannot access Value when Result is not successful.");
}
