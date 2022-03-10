using System.Text.RegularExpressions;
using FinCleaner.Model;

namespace FinCleaner.Parsing;

public class MappingFileParser : IFileParser
{
  public IFileParser DefaultParser { get; }
  public IReadOnlyList<ParserMapping> Rules { get; }

  public MappingFileParser(IFileParser defaultParser, IReadOnlyList<ParserMapping> rules)
  {
    DefaultParser = defaultParser;
    Rules = rules;
  }
  
  public IEnumerable<Transaction> EnumerateTransactions(string path)
  {
    var fileName = Path.GetFileName(path);
    
    foreach (var rule in Rules)
    {
      if (rule.FileNamePattern.IsMatch(fileName))
        return rule.FileParser.EnumerateTransactions(path);
    }

    return DefaultParser.EnumerateTransactions(path);
  }

  public record ParserMapping(Regex FileNamePattern, IFileParser FileParser);
}