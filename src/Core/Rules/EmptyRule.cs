using FinCleaner.Model;

namespace FinCleaner.Rules;

public class EmptyRule : IRule
{
  public IEnumerable<Transaction> Apply(Transaction transaction)
  {
    yield return transaction;
  }
}