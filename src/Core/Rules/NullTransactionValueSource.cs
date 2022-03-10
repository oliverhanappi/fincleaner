using FinCleaner.Model;

namespace FinCleaner.Rules;

public class NullTransactionValueSource : ITransactionValueSource
{
  public string? GetValue(Transaction transaction)
  {
    return null;
  }
}