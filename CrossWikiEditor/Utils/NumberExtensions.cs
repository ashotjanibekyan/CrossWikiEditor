namespace CrossWikiEditor.Utils;

public static class NumberExtensions
{
    public static bool IsEven(this int num)
    {
        return num % 2 == 0;
    }

    public static bool IsOdd(this int num)
    {
        return !num.IsEven();
    }
}