using FinCleaner.Model;

namespace FinCleaner.Rules;

public class ConstantTransactionCondition : ITransactionCondition
{
  public bool Satisfied { get; }

  public ConstantTransactionCondition(bool satisfied)
  {
    Satisfied = satisfied;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    return Satisfied;
  }
}