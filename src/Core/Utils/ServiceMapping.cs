namespace FinCleaner.Utils;

public class ServiceMapping<TKey, TService> : IServiceMapping<TKey, TService>
{
  private readonly TService _defaultService;
  private readonly List<(Func<TKey, bool>, TService)> _services;

  public ServiceMapping(TService defaultService)
  {
    _defaultService = defaultService;
    _services = new();
  }

  public void AddMappedService(Func<TKey, bool> predicate, TService service)
  {
    _services.Add((predicate, service));
  }

  public TService Get(TKey key)
  {
    foreach (var (predicate, service) in _services)
    {
      if (predicate.Invoke(key))
        return service;
    }

    return _defaultService;
  }
}