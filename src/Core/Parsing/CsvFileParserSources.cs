namespace FinCleaner.Parsing;

public record CsvFileParserSources(
  ICsvFileParserValueSource Date,
  ICsvFileParserValueSource Amount,
  ICsvFileParserValueSource AssetAccountName,
  ICsvFileParserValueSource AssetAccountIBAN,
  ICsvFileParserValueSource AssetAccountNumber,
  ICsvFileParserValueSource OppositeAccountName,
  ICsvFileParserValueSource OppositeAccountIBAN,
  ICsvFileParserValueSource OppositeAccountNumber,
  ICsvFileParserValueSource? Description,
  ICsvFileParserValueSource? Reference);