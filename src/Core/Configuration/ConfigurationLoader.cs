using System.Xml.Serialization;

namespace FinCleaner.Configuration.Schema;

public static class ConfigurationLoader
{
  public static Configuration Load(string path)
  {
    using var stream = File.OpenRead(path);
    return Load(stream);
  }
  
  public static Configuration Load(Stream source)
  {
    var xmlSerializer = new XmlSerializer(typeof(Configuration));
    var configuration = (Configuration?)xmlSerializer.Deserialize(source);

    if (configuration == null)
      throw new Exception("Failed to load configuration.");

    return configuration;
  }
}