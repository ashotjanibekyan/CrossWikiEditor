namespace CrossWikiEditor;

public sealed class Result
{
    private Result() { }

    public static Result Failure(string failureMessage) => new() { IsSuccessful = false, Error = failureMessage };

    public static Result Success() => new() { IsSuccessful = true };

    public bool IsSuccessful { get; private init; } = true;

    public string Error { get; private init; } = string.Empty;
}

public sealed class Result<T>
{
    private Result(bool isSuccessful, T? value, string? error)
    {
        IsSuccessful = isSuccessful;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T result)
    {
        return new Result<T>(isSuccessful: true, result, null);
    }
    
    public static Result<string> Failure(string errorMessage)
    {
        return new Result<string>(isSuccessful: false, default, errorMessage);
    }

    public bool IsSuccessful { get; private init; }
    public T? Value { get; private init; }
    public string? Error { get; set; }
}