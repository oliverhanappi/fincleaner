namespace FinCleaner.Utils;

public static class StringExtensions
{
  public static string? TrimAndWhiteSpaceToNull(this string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
  }
}