using FinCleaner.Model;

namespace FinCleaner.Rules;

public class FieldTransactionValueSource : ITransactionValueSource
{
  public TransactionField Field { get; }

  public FieldTransactionValueSource(TransactionField field)
  {
    Field = field;
  }
  
  public string? GetValue(Transaction transaction)
  {
    return transaction[Field];
  }
}