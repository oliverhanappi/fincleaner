using FinCleaner.Model;

namespace FinCleaner.Rules;

public class AggregateRule : IRule
{
  public IReadOnlyList<IRule> Rules { get; }

  public AggregateRule(IReadOnlyList<IRule> rules)
  {
    Rules = rules;
  }
  
  public IEnumerable<Transaction> Apply(Transaction transaction)
  {
    var current = new[] { transaction };

    foreach (var rule in Rules)
      current = current.SelectMany(rule.Apply).ToArray();

    return current;
  }
}