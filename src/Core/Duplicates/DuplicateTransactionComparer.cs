using FinCleaner.Model;

namespace FinCleaner.Duplicates;

public class DuplicateTransactionComparer : IEqualityComparer<Transaction>
{
  public bool Equals(Transaction? x, Transaction? y)
  {
    if (ReferenceEquals(x, y)) return true;
    if (ReferenceEquals(x, null)) return false;
    if (ReferenceEquals(y, null)) return false;

    return x.Date.Equals(y.Date)
           && x.Amount == y.Amount
           && x.AssetAccount.Matches(y.AssetAccount)
           && x.OppositeAccount.Matches(y.OppositeAccount);
  }

  public int GetHashCode(Transaction obj)
  {
    return HashCode.Combine(obj.Date, obj.Amount, obj.AssetAccount, obj.OppositeAccount);
  }
}