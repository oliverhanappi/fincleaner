namespace FinCleaner.Utils;

public interface IServiceMapping<in TKey, out TService>
{
  TService Get(TKey key);
}