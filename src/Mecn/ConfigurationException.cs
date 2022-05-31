namespace Mecn;

public class ConfigurationException : Exception
{
    public string? PropertyName { get; init; }

    public ConfigurationException(string message)
        : base(message)
    {
    }

    public ConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static ConfigurationException InvalidOptions(string property, string? message = null)
    {
        var msg = message ?? $"Missing or invalid value in Options.{property}.";
        return new ConfigurationException(msg)
        {
            PropertyName = property,
        };
    }

    public static ConfigurationException InvalidConnectionString()
    {
        const string msg = "Invalid PostgreSQL database connection string (Npgsql driver). " +
                           "Either the connection string is invalid or environment variable containing it " +
                           "is missing or empty. The default environment variable is MECN_CONNECTION_STRING.";
        return new ConfigurationException(msg)
        {
            PropertyName = "ConnectionString",
        };
    }
}
