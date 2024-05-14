using System.Diagnostics.CodeAnalysis;

namespace CrossWikiEditor.Core.Utils;

public readonly struct Result<TResult> : IEquatable<Result<TResult>>
{
    public Result(TResult result)
    {
        IsSuccessful = true;
        Value = result;
        Exception = default;
        ErrorMessage = string.Empty;
    }

    public Result(Exception exception)
    {
        IsSuccessful = false;
        Value = default;
        Exception = exception;
        ErrorMessage = Exception.Message;
    }

    [MemberNotNullWhen(returnValue: true, nameof(Value))]
    public bool IsSuccessful { get; }

    [MemberNotNullWhen(returnValue: true, nameof(ErrorMessage))]
    public bool IsError => !IsSuccessful;

    public TResult? Value { get; }

    public Exception? Exception { get; }
    public string ErrorMessage { get; }

    public bool Equals(Result<TResult> other) => IsSuccessful && other.IsSuccessful && Equals(Value, other.Value);
    public override bool Equals(object? obj) => obj is Result<TResult> result && Equals(result);
    public static bool operator ==(Result<TResult> left, Result<TResult> right) => left.Equals(right);
    public static bool operator !=(Result<TResult> left, Result<TResult> right) => !(left == right);
    public override int GetHashCode() => HashCode.Combine(IsSuccessful, Value, Exception, ErrorMessage);

    public static implicit operator Result<TResult>(TResult value) => new(value);
    public static implicit operator Result<TResult>(Exception value) => new(value);
}