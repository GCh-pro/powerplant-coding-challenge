using System.Collections.Generic;

namespace Mapper;

public class ErrorOr<T>
{
    public T? Result { get; init; }
    public List<string> Errors { get; init; } = new();
    public bool IsError => Errors != null && Errors.Count > 0;

    public static ErrorOr<T> Ok(T result) => new ErrorOr<T> { Result = result };
    public static ErrorOr<T> Error(params string[] errors) => new ErrorOr<T> { Errors = new List<string>(errors) };
}
