using FinCleaner.Model;

namespace FinCleaner.Rules;

public class KeywordTransactionCondition : ITransactionCondition
{
  public ITransactionValueSource ValueSource { get; }
  public string Keyword { get; }

  public KeywordTransactionCondition(ITransactionValueSource valueSource, string keyword)
  {
    ValueSource = valueSource;
    Keyword = keyword;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    var value = ValueSource.GetValue(transaction);
    if (value == null)
      return false;

    var words = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    return words.Contains(Keyword, StringComparer.OrdinalIgnoreCase);
  }
}