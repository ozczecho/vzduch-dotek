public static class StringExtensions
{
    ///
    /// Simulate Java Style SubString
    ///
    public static string JavaStyleSubstring(this string s, int beginIndex, int endIndex)
    {
        var len = endIndex - beginIndex;
        return s.Substring(beginIndex, len);
    }
}