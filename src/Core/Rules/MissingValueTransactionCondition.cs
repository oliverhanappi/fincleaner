using FinCleaner.Model;

namespace FinCleaner.Rules;

public class MissingValueTransactionCondition : ITransactionCondition
{
  public ITransactionValueSource ValueSource { get; }

  public MissingValueTransactionCondition(ITransactionValueSource valueSource)
  {
    ValueSource = valueSource;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    var value = ValueSource.GetValue(transaction);
    return value == null;
  }
}