using FinCleaner.Model;

namespace FinCleaner.Serialization;

public class TransactionCsvRecord
{
  public DateOnly Date { get; }
  public decimal Amount { get; }
  
  public string? AssetAccountName { get; }
  public string? AssetAccountIBAN { get; }
  public string? AssetAccountNumber { get; }
  
  public string? OppositeAccountName { get; }
  public string? OppositeAccountIBAN { get; }
  public string? OppositeAccountNumber { get; }

  public string? Description { get; }
  public string? Reference { get; }

  public TransactionCsvRecord(Transaction transaction)
  {
    Date = transaction.Date;
    Amount = transaction.Amount;

    AssetAccountName = transaction.AssetAccount.Name;
    AssetAccountIBAN = transaction.AssetAccount.IBAN;
    AssetAccountNumber = transaction.AssetAccount.AccountNumber;

    OppositeAccountName = transaction.OppositeAccount.Name;
    OppositeAccountIBAN = transaction.OppositeAccount.IBAN;
    OppositeAccountNumber = transaction.OppositeAccount.AccountNumber;
    
    Description = transaction.Description;
    Reference = transaction.Reference;
  }
}