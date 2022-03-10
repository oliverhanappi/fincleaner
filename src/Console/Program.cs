using System.Reflection;
using FinCleaner.Configuration.Schema;
using FinCleaner.Duplicates;
using FinCleaner.Model;
using FinCleaner.Serialization;

namespace FinCleaner.Console;

public static class Program
{
  public static void Main(string[] args)
  {
    var stopOnExit = true;
    
    try
    {
      var arguments = ParseArguments(args);
      if (arguments != null)
      {
        stopOnExit = arguments.StopOnExit;
        Run(arguments);
      }
    }
    catch (Exception ex)
    {
      System.Console.WriteLine($"An error occurred: {ex.Message}");
      System.Console.WriteLine("Details:");
      System.Console.WriteLine(ex);

      Environment.ExitCode = 1;
    }

    if (stopOnExit)
    {
      System.Console.WriteLine("Press return to exit.");
      System.Console.ReadLine();
    }
  }
  
  public static void Run(FinCleanerArguments arguments)
  {
    var transactionReader = arguments.Configuration.CreateReader();
    var serializationSettings = arguments.Configuration.Settings.CreateSerializationSettings();

    var transactions = new List<Transaction>();
    foreach (var transactionFilePath in arguments.TransactionFilePaths)
    {
      System.Console.WriteLine($"Reading {transactionFilePath}...");

      var fileTransactions = transactionReader.ReadTransactions(transactionFilePath);
      transactions.AddRange(fileTransactions);
    }

    transactions = transactions.OrderBy(t => t.Date).ThenBy(t => t.Amount).ToList();

    var duplicateTransactions = DuplicateTransactionChecker.DetectDuplicateTransactions(transactions);
    foreach (var duplicateTransaction in duplicateTransactions)
    {
      System.Console.WriteLine("Duplicate transaction detected:");
      foreach (var t in duplicateTransaction.Transactions)
        System.Console.WriteLine($"  - {t}");
    }
    
    if (duplicateTransactions.Count == 0)
      System.Console.WriteLine("No duplicate transactions detected.");

    var transactionWriter = new TransactionWriter(serializationSettings);

    System.Console.WriteLine($"Generating output {arguments.TargetFilePath}...");
    transactionWriter.Write(transactions, arguments.TargetFilePath);

    System.Console.WriteLine("Finished successfully.");
  }

  public static FinCleanerArguments? ParseArguments(string[] args)
  {
    var stopOnExit = !ExtractSwitch("noprompt");
    
    if (args.Length < 2)
    {
      PrintUsage();
      return null;
    }

    var configurationFilePaths = args.Where(p => string.Equals(Path.GetExtension(p), ".xml", StringComparison.OrdinalIgnoreCase)).ToList();
    var transactionFilePaths = args.Except(configurationFilePaths).ToList();

    if (configurationFilePaths.Count == 0)
    {
      PrintUsage();
      System.Console.WriteLine();
      System.Console.WriteLine("You must provide a configuration file.");

      return null;
    }

    if (configurationFilePaths.Count >= 2)
    {
      PrintUsage();
      System.Console.WriteLine();
      System.Console.WriteLine("You must provide exactly one configuration file.");
      
      foreach (var configurationFilePath in configurationFilePaths)
        System.Console.WriteLine($"  - {configurationFilePath}");

      return null;
    }

    if (transactionFilePaths.Count == 0)
    {
      PrintUsage();
      System.Console.WriteLine();
      System.Console.WriteLine("You must provide at least one transaction file.");

      return null;
    }

    var targetDirectory = Path.GetDirectoryName(transactionFilePaths[0]);
    var targetFilePath = Path.Combine(targetDirectory ?? Environment.CurrentDirectory, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-FinCleaner-Result.csv");
    
    var configuration = ConfigurationLoader.Load(configurationFilePaths[0]);
    return new(configuration, transactionFilePaths, targetFilePath, stopOnExit);

    bool ExtractSwitch(string name)
    {
      var arg = $"--{name}";
      if (args.Contains(arg, StringComparer.OrdinalIgnoreCase))
      {
        args = args.Except(new[] { arg }, StringComparer.OrdinalIgnoreCase).ToArray();
        return true;
      }

      return false;
    }
  }

  private static void PrintUsage()
  {
    System.Console.WriteLine($"FinCleaner {Assembly.GetExecutingAssembly().GetName().Version}");
    System.Console.WriteLine();
    System.Console.WriteLine("Usage:");
    System.Console.WriteLine("FinCleaner.exe <Configuration.xml> <Transactions.csv> [<Transactions2.csv> ...]");
  }

  public record FinCleanerArguments(Configuration.Schema.Configuration Configuration, IReadOnlyCollection<string> TransactionFilePaths, string TargetFilePath, bool StopOnExit);
}
