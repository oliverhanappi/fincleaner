using FinCleaner.Model;

namespace FinCleaner.Rules;

public class FixedTransactionValueSource : ITransactionValueSource
{
  public string Value { get; }

  public FixedTransactionValueSource(string value)
  {
    Value = value;
  }
  
  public string? GetValue(Transaction transaction)
  {
    return Value;
  }
}