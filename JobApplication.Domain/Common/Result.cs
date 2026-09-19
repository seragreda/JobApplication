namespace JobApplication.Domain.Common;

public enum ErrorType { None, NotFound, Forbidden, BadRequest, Conflict, Unauthorized }

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    public ErrorType ErrorType { get; init; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error, ErrorType type) => new() { IsSuccess = false, Error = error, ErrorType = type };
}

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public ErrorType ErrorType { get; init; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Fail(string error, ErrorType type) => new() { IsSuccess = false, Error = error, ErrorType = type };
}