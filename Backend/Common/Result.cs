using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PrjA.Common;


public enum ErrorType
{
    Unauthorized,
    Forbidden,
    NotFound,
    BadRequest,
}

public record Error
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public ErrorType Type{ get; init; }
    public string? StackTrace { get; init; }
}

public class Result<T> 
{
    public bool isSuccess { get; }
    
    
    public T? Value { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        this.isSuccess = isSuccess;
        this.Value = value;
        this.Error = error;
    }

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(true, value, null);
        
    }

    public static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<T>(false, default, error);
    }
}