using System;

namespace CrossWikiEditor.Core.Utils;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Default = new();

    public override int GetHashCode()
    {
        return 0;
    }

    public bool Equals(Unit other)
    {
        return true;
    }

    public override bool Equals(object? obj)
    {
        return true;
    }

    public static bool operator ==(Unit lhs, Unit rhs)
    {
        return true;
    }

    public static bool operator !=(Unit lhs, Unit rhs)
    {
        return false;
    }

    public static bool operator >(Unit lhs, Unit rhs)
    {
        return false;
    }

    public static bool operator >=(Unit lhs, Unit rhs)
    {
        return true;
    }

    public static bool operator <(Unit lhs, Unit rhs)
    {
        return false;
    }

    public static bool operator <=(Unit lhs, Unit rhs)
    {
        return true;
    }

    public int CompareTo(Unit other)
    {
        return 0;
    }
}