using FinCleaner.Model;

namespace FinCleaner.Rules;

public interface ITransactionValueSource
{
  string? GetValue(Transaction transaction);
}