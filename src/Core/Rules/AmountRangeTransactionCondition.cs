using FinCleaner.Model;

namespace FinCleaner.Rules;

public class AmountRangeTransactionCondition : ITransactionCondition
{
  public decimal? Min { get; }
  public bool MinInclusive { get; }
  public decimal? Max { get; }
  public bool MaxInclusive { get; }

  public AmountRangeTransactionCondition(decimal? min, bool minInclusive, decimal? max, bool maxInclusive)
  {
    Min = min;
    MinInclusive = minInclusive;
    Max = max;
    MaxInclusive = maxInclusive;
  }
  
  public bool IsSatisfied(Transaction transaction)
  {
    var minSatisfied = Min == null || transaction.Amount > Min.Value || (MinInclusive && transaction.Amount == Min.Value);
    var maxSatisfied = Max == null || transaction.Amount < Max.Value || (MaxInclusive && transaction.Amount == Max.Value);
    return minSatisfied && maxSatisfied;
  }
}