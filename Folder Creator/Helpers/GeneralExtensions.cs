namespace Folder_Creator.Helpers;

public static class StringExtensions{
    public static bool IsEmpty(this string text) => string.IsNullOrWhiteSpace(text);
}
