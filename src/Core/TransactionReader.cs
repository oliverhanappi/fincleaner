using FinCleaner.Model;
using FinCleaner.Parsing;
using FinCleaner.Rules;
using FinCleaner.Utils;

namespace FinCleaner;

public class TransactionReader : ITransactionReader
{
  private readonly IServiceMapping<string, IFileParser> _fileParsers;
  private readonly IServiceMapping<string, IRule> _rules;

  public TransactionReader(IServiceMapping<string, IFileParser> fileParsers, IServiceMapping<string, IRule> rules)
  {
    _fileParsers = fileParsers;
    _rules = rules;
  }
  
  public IEnumerable<Transaction> ReadTransactions(string path)
  {
    var fileParser = _fileParsers.Get(path);
    var rule = _rules.Get(path);

    return fileParser.EnumerateTransactions(path).SelectMany(rule.Apply);
  }
}