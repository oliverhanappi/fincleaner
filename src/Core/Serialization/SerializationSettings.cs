using System.Globalization;
using FinCleaner.Model;

namespace FinCleaner.Serialization;

public record SerializationSettings(CultureInfo Culture, Account FallbackAccount, string FallbackDescription);