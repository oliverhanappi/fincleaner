using System.Globalization;
using System.Text;

namespace FinCleaner.Parsing;

public record CsvFileParserOptions(Encoding Encoding, string Delimiter, char Quote, CsvFileParserMode Mode, bool HasHeaderRow, CultureInfo Culture);