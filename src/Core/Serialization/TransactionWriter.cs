using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FinCleaner.Model;

namespace FinCleaner.Serialization;

public class TransactionWriter
{
  public SerializationSettings SerializationSettings { get; }

  public TransactionWriter(SerializationSettings serializationSettings)
  {
    SerializationSettings = serializationSettings;
  }
  
  public void Write(IEnumerable<Transaction> transactions, string outputPath)
  {
    var csvConfiguration = new CsvConfiguration(SerializationSettings.Culture)
    {
      HasHeaderRecord = true,
      Delimiter = ";"
    };

    using var streamWriter = new StreamWriter(outputPath, append: false, encoding: Encoding.UTF8);
    using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);

    csvWriter.WriteHeader<TransactionCsvRecord>();
    csvWriter.NextRecord();
    
    foreach (var transaction in transactions)
    {
      var transactionToWrite = transaction with
      {
        AssetAccount = transaction.AssetAccount.ApplyFallback(SerializationSettings.FallbackAccount),
        OppositeAccount = transaction.OppositeAccount.ApplyFallback(SerializationSettings.FallbackAccount),
        Description = transaction.Description ?? SerializationSettings.FallbackDescription,
      };

      var csvRecord = new TransactionCsvRecord(transactionToWrite);
      csvWriter.WriteRecord(csvRecord);
      csvWriter.NextRecord();
    }
  }
}