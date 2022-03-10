using FinCleaner.Model;

namespace FinCleaner.Parsing;

public interface IFileParser
{
  IEnumerable<Transaction> EnumerateTransactions(string path);
}