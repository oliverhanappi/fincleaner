using System.Text.RegularExpressions;
using FinCleaner.Model;

namespace FinCleaner.Rules;

class ReplaceTransactionValueSource : ITransactionValueSource
{
  public ITransactionValueSource Source { get; }
  public Regex Pattern { get; }
  public string? Replacement { get; }

  public ReplaceTransactionValueSource(ITransactionValueSource source, Regex pattern, string? replacement)
  {
    Source = source;
    Pattern = pattern;
    Replacement = replacement;
  }
  
  public string? GetValue(Transaction transaction)
  {
    var value = Source.GetValue(transaction);

    if (value != null)
      value = Pattern.Replace(value, Replacement ?? string.Empty);

    return value;
  }
}