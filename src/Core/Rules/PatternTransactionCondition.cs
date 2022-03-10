using System.Text.RegularExpressions;
using FinCleaner.Model;

namespace FinCleaner.Rules;

public class PatternTransactionCondition : ITransactionCondition
{
  public ITransactionValueSource ValueSource { get; }
  public Regex Pattern { get; }

  public PatternTransactionCondition(ITransactionValueSource valueSource, Regex pattern)
  {
    ValueSource = valueSource;
    Pattern = pattern;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    var value = ValueSource.GetValue(transaction);
    return value != null && Pattern.IsMatch(value);
  }
}