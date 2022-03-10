using FinCleaner.Model;

namespace FinCleaner.Rules;

public interface ITransactionCondition
{
  bool IsSatisfied(Transaction transaction);
}