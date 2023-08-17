namespace CrossWikiEditor;

public class Result<T>
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

    public bool IsSuccessful { get; set; }
    public T? Value { get; set; }
    public string? Error { get; set; }
}