using FinCleaner.Model;

namespace FinCleaner.Rules;

public class FilterRule : IRule
{
  public ITransactionCondition Condition { get; }

  public FilterRule(ITransactionCondition condition)
  {
    Condition = condition;
  }

  public IEnumerable<Transaction> Apply(Transaction transaction)
  {
    if (!Condition.IsSatisfied(transaction))
      yield return transaction;
  }
}