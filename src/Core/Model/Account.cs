using FinCleaner.Utils;

namespace FinCleaner.Model;

public record Account
{
  public static readonly Account Unknown = new(null, null, null);

  private readonly string? _name;
  private readonly string? _iban;
  private readonly string? _accountNumber;

  public string? Name
  {
    get => _name;
    init => _name = value.TrimAndWhiteSpaceToNull();
  }

  public string? IBAN
  {
    get => _iban;
    init => _iban = value.TrimAndWhiteSpaceToNull();
  }

  public string? AccountNumber
  {
    get => _accountNumber;
    init => _accountNumber = value.TrimAndWhiteSpaceToNull();
  }

  public Account(string? name, string? iban, string? accountNumber)
  {
    Name = name;
    IBAN = iban;
    AccountNumber = accountNumber;
  }

  public bool Matches(Account other)
  {
    return string.Equals(IBAN, other.IBAN, StringComparison.OrdinalIgnoreCase)
           || string.Equals(AccountNumber, other.AccountNumber, StringComparison.OrdinalIgnoreCase);
  }

  public Account ApplyFallback(Account fallback)
  {
    var iban = IBAN == null && AccountNumber == null ? fallback.IBAN : IBAN;
    var accountNumber = IBAN == null && AccountNumber == null ? fallback.AccountNumber : AccountNumber;

    return new Account(Name ?? fallback.Name, iban, accountNumber);
  }
}