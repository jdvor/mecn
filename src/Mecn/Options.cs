namespace Mecn;

using System.Text.RegularExpressions;

/// <summary>
/// Configuration options.
/// </summary>
public class Options
{
    private static readonly Regex EnvVarNameRgx = new Regex(@"^[A-Z][A-Z0-9_]*$");
    private static readonly Regex AppRgx = new Regex(@"^[a-z]\w*$");
    private static readonly Regex EnvRgx = new Regex(@"^[a-z]+$");
    private static readonly Regex HostRgx = new Regex(@"^[a-z]\w*(\.[a-z]\w*)*$", RegexOptions.IgnoreCase);
    private static readonly Regex SafeRgx = new Regex(@"^[a-z][a-z_]*$");

    /// <summary>
    /// Application name (required).
    /// </summary>
    public string ApplicationName { get; set; } = string.Empty;

    /// <summary>
    /// Environment name; default is 'prod'. Suggested environment names: prod, production, stage, staging, test, dev.
    /// </summary>
    public string EnvironmentName { get; set; } = "prod";

    /// <summary>
    /// Hosting system name. Typically result of commands `hostname --fqdn` on Linux or `hostname` on Windows.
    /// </summary>
    public string HostName { get; set; } = "*";

    /// <summary>
    /// Either explicitly PostgreSQL connection string (Npgsql driver) or environment variable name
    /// that point to the same.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// PostgreSQL database table name; default is 'appsettings';
    /// </summary>
    public string DbTable { get; set; } = "appsettings";

    /// <summary>
    /// PostgreSQL database schema name; default is 'public';
    /// </summary>
    public string DbSchema { get; set; } = "public";

    /// <summary>
    /// Npgsql exceptions, thrown mostly when you cannot connect to database server
    /// or required schema or table are not present, are ignored when this is set to <code>true</code>;
    /// default is <code>false</code>.
    /// </summary>
    public bool Optional { get; set; }

    public bool HasHostName => !string.IsNullOrEmpty(HostName) && HostName != "*";

    /// <summary>
    /// Validates this instance of Options and throws an exception when it is deemed invalid.
    /// </summary>
    /// <exception cref="ConfigurationException"></exception>
    public void EnsureValidOrThrow()
    {
        // ReSharper disable NotResolvedInText
        if (string.IsNullOrWhiteSpace(ApplicationName) || !AppRgx.IsMatch(ApplicationName))
        {
            throw ConfigurationException.InvalidOptions(nameof(ApplicationName));
        }

        if (string.IsNullOrWhiteSpace(EnvironmentName) || !EnvRgx.IsMatch(EnvironmentName))
        {
            throw ConfigurationException.InvalidOptions(nameof(EnvironmentName));
        }

        if (string.IsNullOrWhiteSpace(HostName) || (HostName != "*" && !HostRgx.IsMatch(HostName)))
        {
            throw ConfigurationException.InvalidOptions(nameof(HostName));
        }

        if (string.IsNullOrWhiteSpace(ConnectionString)
            || (!EnvVarNameRgx.IsMatch(ConnectionString)
            && !ConnectionString.Contains("Database=", StringComparison.OrdinalIgnoreCase)))
        {
            throw ConfigurationException.InvalidConnectionString();
        }

        if (string.IsNullOrWhiteSpace(DbSchema) || !SafeRgx.IsMatch(DbSchema))
        {
            throw ConfigurationException.InvalidOptions(nameof(DbSchema));
        }

        if (string.IsNullOrWhiteSpace(DbTable) || !SafeRgx.IsMatch(DbTable))
        {
            throw ConfigurationException.InvalidOptions(nameof(DbTable));
        }
    }
}
