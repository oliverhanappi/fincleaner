using FinCleaner.Model;

namespace FinCleaner.Rules;

public class AnyTransactionCondition : ITransactionCondition
{
  public IReadOnlyCollection<ITransactionCondition> Conditions { get; }

  public AnyTransactionCondition(IReadOnlyCollection<ITransactionCondition> conditions)
  {
    Conditions = conditions;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    return Conditions.Any(c => c.IsSatisfied(transaction));
  }
}