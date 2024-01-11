namespace NDSParse.Extensions;

public static class StringExtensions
{
    public static string Reversed(this string str)
    {
        var reversedCharacters = str.Reverse().ToArray();
        return new string(reversedCharacters);
    }
}