using CsvHelper;
using FinCleaner.Utils;

namespace FinCleaner.Parsing;

public interface ICsvFileParserValueSource
{
  T? GetValue<T>(string path, IReaderRow row);

  string? GetNormalizedString(string path, IReaderRow row)
  {
    return GetValue<string?>(path, row).TrimAndWhiteSpaceToNull();
  }
}