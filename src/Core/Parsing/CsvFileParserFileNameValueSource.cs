using CsvHelper;

namespace FinCleaner.Parsing;

public class CsvFileParserFileNameValueSource : ICsvFileParserValueSource
{
  public T GetValue<T>(string path, IReaderRow row)
  {
    return (T) Convert.ChangeType(path, typeof(T));
  }
}