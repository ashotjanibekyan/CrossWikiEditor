namespace CrossWikiEditor.Core.Utils;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Default = new Unit();
    public override int GetHashCode() => 0;
    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => true;
    public static bool operator ==(Unit lhs, Unit rhs) => true;
    public static bool operator !=(Unit lhs, Unit rhs) => false;
    public static bool operator >(Unit lhs, Unit rhs) => false;
    public static bool operator >=(Unit lhs, Unit rhs) => true;
    public static bool operator <(Unit lhs, Unit rhs) => false;
    public static bool operator <=(Unit lhs, Unit rhs) => true;
    public int CompareTo(Unit other) => 0;
}