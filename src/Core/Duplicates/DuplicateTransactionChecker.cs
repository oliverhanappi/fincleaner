using FinCleaner.Model;

namespace FinCleaner.Duplicates;

public static class DuplicateTransactionChecker
{
  public static IReadOnlyCollection<DuplicateTransaction> DetectDuplicateTransactions(IEnumerable<Transaction> transactions)
  {
    return transactions
      .GroupBy(t => t, new DuplicateTransactionComparer())
      .Where(g => g.Count() >= 2)
      .Select(g => new DuplicateTransaction(g.ToList()))
      .ToList();
  }
}