using System.Globalization;
using System.Text.RegularExpressions;
using FinCleaner.Model;
using FinCleaner.Parsing;
using FinCleaner.Rules;
using FinCleaner.Serialization;
using FinCleaner.Utils;

namespace FinCleaner.Configuration.Schema;

public partial class Configuration
{
  public ITransactionReader CreateReader()
  {
    var fileParsers = FileParsers.ToDictionary(p => p.Name, p => p.CreateParser());
    var rules = RuleSetsSpecified && RuleSets != null ? RuleSets.ToDictionary(p => p.Name, p => p.CreateRule()) : new();

    var fileParserMapping = new ServiceMapping<string, IFileParser>(fileParsers[FilePatterns.DefaultParser]);
    var ruleMapping = new ServiceMapping<string, IRule>(FilePatterns.DefaultRuleSet != null ? rules[FilePatterns.DefaultRuleSet] : new EmptyRule());
    
    foreach (var filePattern in FilePatterns.Pattern)
    {
      var fileNamePattern = new Regex(filePattern.Pattern);
      fileParserMapping.AddMappedService(fileNamePattern.IsMatch, fileParsers[filePattern.Parser]);
      
      if (filePattern.RuleSet != null)
        ruleMapping.AddMappedService(fileNamePattern.IsMatch, rules[filePattern.RuleSet]);
    }

    return new TransactionReader(fileParserMapping, ruleMapping);
  }
}

public partial class ConfigurationSettings
{
  public SerializationSettings CreateSerializationSettings()
  {
    var cultureInfo = CultureInfo.GetCultureInfo(ExportCulture);
    var fallbackAccount = new Account(FallbackAccountName, FallbackAccountIBAN, FallbackAccountNumber);

    return new SerializationSettings(cultureInfo, fallbackAccount, FallbackDescription);
  }
}

public partial class Parser
{
  public abstract IFileParser CreateParser();
}

public partial class CsvParser
{
  public override IFileParser CreateParser()
  {
    var encoding = GetEncoding();
    var cultureInfo = CultureInfo.GetCultureInfo(Culture);
    var mode = Mode switch
    {
      CsvParserMode.Escape => CsvFileParserMode.Escape,
      CsvParserMode.NoEscape => CsvFileParserMode.NoEscape,
      CsvParserMode.RFC4180 => CsvFileParserMode.RFC4180,
      _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, $"Unknown CSV mode: {Mode}")
    };

    var options = new CsvFileParserOptions(encoding, Delimiter, Quote[0], mode, Header, cultureInfo);

    var sources = new CsvFileParserSources(
      Date.CreateValueSource(),
      Amount.CreateValueSource(),
      AssetAccountName.CreateValueSource(),
      AssetAccountIBAN.CreateValueSource(),
      AssetAccountNumber.CreateValueSource(),
      OppositeAccountName.CreateValueSource(),
      OppositeAccountIBAN.CreateValueSource(),
      OppositeAccountNumber.CreateValueSource(),
      Description?.CreateValueSource(),
      Reference?.CreateValueSource()
    );

    return new CsvFileParser(options, sources);
  }

  private System.Text.Encoding GetEncoding()
  {
    switch (Encoding)
    {
      case FileEncoding.Utf_8:
        return System.Text.Encoding.UTF8;
      case FileEncoding.Utf_16:
        return System.Text.Encoding.Unicode;
      case FileEncoding.Utf_16BE:
        return System.Text.Encoding.BigEndianUnicode;
      case FileEncoding.Ascii:
        return System.Text.Encoding.ASCII;
      case FileEncoding.Iso_8859_1:
        return System.Text.Encoding.Latin1;
      default:
        throw new ArgumentOutOfRangeException(nameof(Encoding), Encoding, $"Unknown encoding: {Encoding}");
    }
  }
}

public partial class ValueSource
{
  public abstract ICsvFileParserValueSource CreateValueSource();
}

public partial class ColumnValueSource
{
  public override ICsvFileParserValueSource CreateValueSource()
  {
    return new CsvFileParserColumnValueSource(ColumnIndex);
  }
}

public partial class FileNameValueSource
{
  public override ICsvFileParserValueSource CreateValueSource()
  {
    return new CsvFileParserFileNameValueSource();
  }
}

public partial class PatternValueSource
{
  public override ICsvFileParserValueSource CreateValueSource()
  {
    var source = Source.CreateValueSource();
    var patternValues = Value.Select(v => v.CreatePatternValue()).ToList();

    return new CsvFileParserPatternValueSource(source, patternValues);
  }
}

public partial class PatternValue
{
  public abstract CsvFileParserPatternValueSource.IPatternValue CreatePatternValue();
}

public partial class FixedPatternValue
{
  public override CsvFileParserPatternValueSource.IPatternValue CreatePatternValue()
  {
    var normalized = !string.IsNullOrWhiteSpace(Value) ? Value : null;
    return new CsvFileParserPatternValueSource.FixedPatternValue(new Regex(Pattern), normalized);
  }
}

public partial class MatchedPatternValue
{
  public override CsvFileParserPatternValueSource.IPatternValue CreatePatternValue()
  {
    var replaceValues = Replace
      .Select(r => new CsvFileParserPatternValueSource.MatchedPatternReplaceValue(new Regex(r.Pattern), r.Value))
      .ToList();

    return new CsvFileParserPatternValueSource.MatchedPatternValue(new Regex(Pattern), GroupName, replaceValues);
  }
}

public partial class RuleSet
{
  public IRule CreateRule()
  {
    if (Rule.Count == 0)
      return new EmptyRule();

    var rules = Rule.Select(r => r.CreateRule()).ToList();
    return new AggregateRule(rules);
  }
}

public partial class Rule
{
  public abstract IRule CreateRule();

  protected ITransactionCondition CreateCondition()
  {
    if (Condition.Count == 0)
      return new ConstantTransactionCondition(satisfied: true);

    var conditions = Condition.Select(c => c.CreateCondition()).ToList();
    return ConditionMatchingMode switch
    {
      ConditionMatchingMode.All => new AllTransactionCondition(conditions),
      ConditionMatchingMode.Any => new AnyTransactionCondition(conditions),
      _ => throw new ArgumentOutOfRangeException(nameof(ConditionMatchingMode), ConditionMatchingMode, $"Unknown condition matching mode: {ConditionMatchingMode}")
    };
  }
}

public partial class ModifyFieldsRule
{
  public override IRule CreateRule()
  {
    var rules = new List<IRule>();
    var condition = CreateCondition();

    AddRule(TransactionField.AssetAccountName, AssetAccountName);
    AddRule(TransactionField.AssetAccountIBAN, AssetAccountIBAN);
    AddRule(TransactionField.AssetAccountNumber, AssetAccountNumber);

    AddRule(TransactionField.OppositeAccountName, OppositeAccountName);
    AddRule(TransactionField.OppositeAccountIBAN, OppositeAccountIBAN);
    AddRule(TransactionField.OppositeAccountNumber, OppositeAccountNumber);
    
    AddRule(TransactionField.Description, Description);
    AddRule(TransactionField.Reference, Reference);

    if (rules.Count == 0)
      return new EmptyRule();

    return new CondititionalRule(condition, new AggregateRule(rules));

    void AddRule(TransactionField transactionField, TransactionValueSource? valueSource)
    {
      if (valueSource == null)
        return;

      var mappedField = transactionField.Map();
      var mappedSource = valueSource.CreateSource();

      var modifyFieldRule = new ModifyFieldRule(mappedField, mappedSource);
      rules.Add(modifyFieldRule);
    }
  }
}

public partial class FilterRule
{
  public override IRule CreateRule()
  {
    return new Rules.FilterRule(CreateCondition());
  }
}

public partial class TransactionCondition
{
  public abstract ITransactionCondition CreateCondition();
}

public partial class AmountRangeCondition
{
  public override ITransactionCondition CreateCondition()
  {
    decimal? min = MinSpecified ? Min : null;
    decimal? max = MaxSpecified ? Max : null;
    return new AmountRangeTransactionCondition(min, MinInclusive, max, MaxInclusive);
  }
}

public partial class FieldTransactionCondition
{
  public sealed override ITransactionCondition CreateCondition()
  {
    var source = new Rules.FieldTransactionValueSource(Field.Map());
    return CreateCondition(source);
  }
  
  protected abstract ITransactionCondition CreateCondition(ITransactionValueSource valueSource);
}

public partial class PatternCondition
{
  protected override ITransactionCondition CreateCondition(ITransactionValueSource valueSource)
  {
    var pattern = new Regex(Pattern, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
    return new PatternTransactionCondition(valueSource, pattern);
  }
}

public partial class KeywordCondition
{
  protected override ITransactionCondition CreateCondition(ITransactionValueSource valueSource)
  {
    return new KeywordTransactionCondition(valueSource, Keyword.Trim());
  }
}

public partial class MissingValueCondition
{
  protected override ITransactionCondition CreateCondition(ITransactionValueSource valueSource)
  {
    return new MissingValueTransactionCondition(valueSource);
  }
}

public partial class TransactionValueSource
{
  public abstract ITransactionValueSource CreateSource();
}

public partial class NullTransactionValueSource
{
  public override ITransactionValueSource CreateSource()
  {
    return new Rules.NullTransactionValueSource();
  }
}

public partial class FixedTransactionValueSource
{
  public override ITransactionValueSource CreateSource()
  {
    return new Rules.FixedTransactionValueSource(Value);
  }
}

public partial class FieldTransactionValueSource
{
  public override ITransactionValueSource CreateSource()
  {
    return new Rules.FieldTransactionValueSource(Field.Map());
  }
}

public partial class ReplaceTransactionValueSource
{
  public override ITransactionValueSource CreateSource()
  {
    var source = Source.CreateSource();
    var pattern = new Regex(Pattern, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
    return new Rules.ReplaceTransactionValueSource(source, pattern, Replacement);
  }
}

public static class TransactionFieldMappingExtensions
{
  public static Model.TransactionField Map(this TransactionField field)
  {
    return field switch
    {
      TransactionField.AssetAccountName => Model.TransactionField.AssetAccountName,
      TransactionField.AssetAccountIBAN => Model.TransactionField.AssetAccountIBAN,
      TransactionField.AssetAccountNumber => Model.TransactionField.AssetAccountNumber,
      TransactionField.OppositeAccountName => Model.TransactionField.OppositeAccountName,
      TransactionField.OppositeAccountIBAN => Model.TransactionField.OppositeAccountIBAN,
      TransactionField.OppositeAccountNumber => Model.TransactionField.OppositeAccountNumber,
      TransactionField.Description => Model.TransactionField.Description,
      TransactionField.Reference => Model.TransactionField.Reference,
      _ => throw new ArgumentOutOfRangeException(nameof(field), field, $"Unknown field: {field}")
    };
  }
}