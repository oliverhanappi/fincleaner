using FinCleaner.Model;

namespace FinCleaner.Rules;

public class ModifyFieldRule : IRule
{
  public TransactionField Field { get; }
  public ITransactionValueSource ValueSource { get; }

  public ModifyFieldRule(TransactionField field, ITransactionValueSource valueSource)
  {
    Field = field;
    ValueSource = valueSource;
  }
  
  public IEnumerable<Transaction> Apply(Transaction transaction)
  {
    var replacement = ValueSource.GetValue(transaction);
    yield return transaction.WithField(Field, replacement);
  }
}