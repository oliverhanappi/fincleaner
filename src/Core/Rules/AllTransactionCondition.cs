using FinCleaner.Model;

namespace FinCleaner.Rules;

public class AllTransactionCondition : ITransactionCondition
{
  public IReadOnlyCollection<ITransactionCondition> Conditions { get; }

  public AllTransactionCondition(IReadOnlyCollection<ITransactionCondition> conditions)
  {
    Conditions = conditions;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    return Conditions.All(c => c.IsSatisfied(transaction));
  }
}