using System;
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

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccessful { get; }

    [MemberNotNullWhen(true, nameof(ErrorMessage))]
    public bool IsError => !IsSuccessful;

    public TResult? Value { get; }

    public Exception? Exception { get; }
    public string ErrorMessage { get; }

    public bool Equals(Result<TResult> other)
    {
        return IsSuccessful && other.IsSuccessful && Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<TResult> result && Equals(result);
    }

    public static bool operator ==(Result<TResult> left, Result<TResult> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<TResult> left, Result<TResult> right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsSuccessful, Value, Exception, ErrorMessage);
    }

    public static implicit operator Result<TResult>(TResult value)
    {
        return new Result<TResult>(value);
    }

    public static implicit operator Result<TResult>(Exception value)
    {
        return new Result<TResult>(value);
    }
}