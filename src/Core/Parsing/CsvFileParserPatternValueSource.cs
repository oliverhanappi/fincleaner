using System.Text.RegularExpressions;
using CsvHelper;

namespace FinCleaner.Parsing;

public class CsvFileParserPatternValueSource : ICsvFileParserValueSource
{
  public ICsvFileParserValueSource Source { get; }
  public IReadOnlyList<IPatternValue> PatternValues { get; }

  public CsvFileParserPatternValueSource(ICsvFileParserValueSource source, IReadOnlyList<IPatternValue> patternValues)
  {
    Source = source;
    PatternValues = patternValues;
  }
  
  public T? GetValue<T>(string path, IReaderRow row)
  {
    var sourceValue = Source.GetValue<string>(path, row);
    if (!string.IsNullOrWhiteSpace(sourceValue))
    {
      foreach (var patternValue in PatternValues)
      {
        if (patternValue.TryGetValue(sourceValue, out var value))
          return value != null ? (T)Convert.ChangeType(value, typeof(T)) : default;
      }
    }

    return default;
  }
  
  public interface IPatternValue
  {
    bool TryGetValue(string input, out string? value);
  }

  public class FixedPatternValue : IPatternValue
  {
    public Regex Pattern { get; }
    public string? Value { get; }

    public FixedPatternValue(Regex pattern, string? value)
    {
      Pattern = pattern;
      Value = value;
    }
    
    public bool TryGetValue(string input, out string? value)
    {
      if (Pattern.IsMatch(input))
      {
        value = Value;
        return true;
      }

      value = null;
      return false;
    }
  }

  public class MatchedPatternValue : IPatternValue
  {
    public Regex Pattern { get; }
    public string GroupName { get; }
    public IReadOnlyList<MatchedPatternReplaceValue> ReplaceValues { get; }

    public MatchedPatternValue(Regex pattern, string groupName, IReadOnlyList<MatchedPatternReplaceValue> replaceValues)
    {
      Pattern = pattern;
      GroupName = groupName;
      ReplaceValues = replaceValues;
    }
    
    public bool TryGetValue(string input, out string value)
    {
      var match = Pattern.Match(input);
      if (match.Success)
      {
        var matchedValue = match.Groups[GroupName].Value.Trim();
        foreach (var replaceValue in ReplaceValues)
          matchedValue = replaceValue.Pattern.Replace(matchedValue, replaceValue.Value);

        value = matchedValue;
        return true;
      }

      value = string.Empty;
      return false;
    }
  }

  public record MatchedPatternReplaceValue(Regex Pattern, string Value);
}