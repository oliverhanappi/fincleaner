using CsvHelper;
using CsvHelper.Configuration;
using FinCleaner.Model;

namespace FinCleaner.Parsing;

public class CsvFileParser : IFileParser
{
  public CsvFileParserOptions Options { get; }
  public CsvFileParserSources Sources { get; }

  public CsvFileParser(CsvFileParserOptions options, CsvFileParserSources sources)
  {
    Options = options;
    Sources = sources;
  }
  
  public IEnumerable<Transaction> EnumerateTransactions(string path)
  {
    var csvConfiguration = new CsvConfiguration(Options.Culture)
    {
      Encoding = Options.Encoding,
      Delimiter = Options.Delimiter,
      HasHeaderRecord = Options.HasHeaderRow,
      Quote = Options.Quote,
      Mode = Options.Mode switch
      {
        CsvFileParserMode.Escape => CsvMode.Escape,
        CsvFileParserMode.NoEscape => CsvMode.NoEscape,
        CsvFileParserMode.RFC4180 => CsvMode.RFC4180,
        _ => throw new ArgumentOutOfRangeException(nameof(Options.Mode), Options.Mode, $"Unknown CSV mode: {Options.Mode}")
      }
    };

    using var streamReader = new StreamReader(path, Options.Encoding, detectEncodingFromByteOrderMarks: true);
    using var csvReader = new CsvReader(streamReader, csvConfiguration);

    if (Options.HasHeaderRow)
      csvReader.Read();

    while (csvReader.Read())
    {
      var date = Sources.Date.GetValue<DateOnly>(path, csvReader);
      var amount = Sources.Amount.GetValue<decimal>(path, csvReader);

      var assetAccountName = Sources.AssetAccountName.GetNormalizedString(path, csvReader);
      var assetAccountIBAN = Sources.AssetAccountIBAN.GetNormalizedString(path, csvReader);
      var assetAccountNumber = Sources.AssetAccountNumber.GetNormalizedString(path, csvReader);
      var assetAccount = new Account(assetAccountName, assetAccountIBAN, assetAccountNumber);

      var oppositeAccountName = Sources.OppositeAccountName.GetNormalizedString(path, csvReader);
      var oppositeAccountIBAN = Sources.OppositeAccountIBAN.GetNormalizedString(path, csvReader);
      var oppositeAccountNumber = Sources.OppositeAccountNumber.GetNormalizedString(path, csvReader);
      var oppositeAccount = new Account(oppositeAccountName, oppositeAccountIBAN, oppositeAccountNumber);

      var description = Sources.Description?.GetNormalizedString(path, csvReader);
      var reference = Sources.Reference?.GetNormalizedString(path, csvReader);

      var transaction = new Transaction(date, amount, assetAccount, oppositeAccount, description, reference);
      yield return transaction;
    }
  }
}