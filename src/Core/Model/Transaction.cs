namespace FinCleaner.Model;

public record Transaction(DateOnly Date, decimal Amount, Account AssetAccount, Account OppositeAccount, string? Description, string? Reference = null)
{
  public static readonly Transaction Empty = new(DateOnly.MinValue, 0, Account.Unknown, Account.Unknown, null);

  public string? this[TransactionField field] => field switch
  {
    TransactionField.AssetAccountName => AssetAccount.Name,
    TransactionField.AssetAccountIBAN => AssetAccount.IBAN,
    TransactionField.AssetAccountNumber => AssetAccount.AccountNumber,
    TransactionField.OppositeAccountName => OppositeAccount.Name,
    TransactionField.OppositeAccountIBAN => OppositeAccount.IBAN,
    TransactionField.OppositeAccountNumber => OppositeAccount.AccountNumber,
    TransactionField.Description => Description,
    TransactionField.Reference => Reference,
    _ => throw new ArgumentOutOfRangeException(nameof(field), field, $"Unknown field: {field}")
  };

  public Transaction WithField(TransactionField field, string? value)
  {
    return field switch
    {
      TransactionField.AssetAccountName => this with { AssetAccount = AssetAccount with { Name = value } },
      TransactionField.AssetAccountIBAN => this with { AssetAccount = AssetAccount with { IBAN = value } },
      TransactionField.AssetAccountNumber => this with { AssetAccount = AssetAccount with { AccountNumber = value } },
      TransactionField.OppositeAccountName => this with { OppositeAccount = OppositeAccount with { Name = value } },
      TransactionField.OppositeAccountIBAN => this with { OppositeAccount = OppositeAccount with { IBAN = value } },
      TransactionField.OppositeAccountNumber => this with { OppositeAccount = OppositeAccount with { AccountNumber = value } },
      TransactionField.Description => this with { Description = value },
      TransactionField.Reference => this with { Reference = value },
      _ => throw new ArgumentOutOfRangeException(nameof(field), field, $"Unknown field: {field}")
    };
  }
}