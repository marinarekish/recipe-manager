namespace RecipeManager.Application.Common.Results;

public class Result
{
    public ResultStatus Status { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<string> Errors { get; }
    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.NoContent;

    protected Result(ResultStatus status, string? error = null)
    {
        Status = status;
        ErrorMessage = error;
        Errors = error is not null ? [error] : [];
    }

    protected Result(ResultStatus status, IEnumerable<string> errors)
    {
        var list = errors.ToList();
        Status = status;
        Errors = list;
        ErrorMessage = list.FirstOrDefault();
    }

    public static Result Ok() => new(ResultStatus.Ok);
    public static Result NotFound(string? message = null) => new(ResultStatus.NotFound, message);
    public static Result NoContent() => new(ResultStatus.NoContent);
    public static Result ValidationError(string message) => new(ResultStatus.ValidationError, message);
    public static Result ValidationError(IEnumerable<string> errors) => new(ResultStatus.ValidationError, errors);
    public static Result Conflict(string? message = null) => new(ResultStatus.Conflict, message);
    public static Result Forbidden(string? message = null) => new(ResultStatus.Forbidden, message);
    public static Result Unauthorized(string? message = null) => new(ResultStatus.Unauthorized, message);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(ResultStatus status, T? value = default, string? error = null)
        : base(status, error)
    {
        Value = value;
    }

    protected Result(ResultStatus status, T? value, IEnumerable<string> errors)
        : base(status, errors)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(ResultStatus.Ok, value);
    public new static Result<T> NotFound(string? message = null) => new(ResultStatus.NotFound, default, message);
    public new static Result<T> ValidationError(string message) => new(ResultStatus.ValidationError, default, message);
    public new static Result<T> ValidationError(IEnumerable<string> errors) => new(ResultStatus.ValidationError, default, errors);
    public new static Result<T> Conflict(string? message = null) => new(ResultStatus.Conflict, default, message);
    public new static Result<T> Forbidden(string? message = null) => new(ResultStatus.Forbidden, default, message);
    public new static Result<T> Unauthorized(string? message = null) => new(ResultStatus.Unauthorized, default, message);
}
