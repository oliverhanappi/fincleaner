using FinCleaner.Model;

namespace FinCleaner.Duplicates;

public record DuplicateTransaction(IReadOnlyCollection<Transaction> Transactions);