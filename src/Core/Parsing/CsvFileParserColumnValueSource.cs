using CsvHelper;

namespace FinCleaner.Parsing;

public class CsvFileParserColumnValueSource : ICsvFileParserValueSource
{
  public int ColumnIndex { get; }

  public CsvFileParserColumnValueSource(int columnIndex)
  {
    ColumnIndex = columnIndex;
  }
  
  public T GetValue<T>(string path, IReaderRow row)
  {
    return row.GetField<T>(ColumnIndex);
  }
}