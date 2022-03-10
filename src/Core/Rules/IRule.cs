using FinCleaner.Model;

namespace FinCleaner.Rules;

public interface IRule
{
  IEnumerable<Transaction> Apply(Transaction transaction);
}