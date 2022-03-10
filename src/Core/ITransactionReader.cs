using FinCleaner.Model;

namespace FinCleaner;

public interface ITransactionReader
{
  IEnumerable<Transaction> ReadTransactions(string path);
}