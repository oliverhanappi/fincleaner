using FinCleaner.Model;

namespace FinCleaner.Rules;

public class CondititionalRule : IRule
{
  public ITransactionCondition Condition { get; }
  public IRule Rule { get; }

  public CondititionalRule(ITransactionCondition condition, IRule rule)
  {
    Condition = condition;
    Rule = rule;
  }
  
  public IEnumerable<Transaction> Apply(Transaction transaction)
  {
    if (Condition.IsSatisfied(transaction))
    {
      foreach (var processedTransaction in Rule.Apply(transaction))
        yield return processedTransaction;
    }
    else
    {
      yield return transaction;
    }
  }
}