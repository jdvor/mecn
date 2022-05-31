namespace Mecn.Cli;

using System.Text.RegularExpressions;

public class Focus
{
    private static readonly Regex EnvVarNameRgx = new Regex(@"^[A-Z][A-Z0-9_]*$");
    private static readonly Regex AppRgx = new Regex(@"^[a-z]\w*$");
    private static readonly Regex EnvRgx = new Regex(@"^[a-z]+$");
    private static readonly Regex HostRgx = new Regex(@"^[a-z]\w*(\.[a-z]\w*)*$", RegexOptions.IgnoreCase);
    private static readonly Regex SafeRgx = new Regex(@"^[a-z][a-z_]*$");

    public string App { get; }

    public string Env { get; }

    public string Host { get; }

    public string Schema { get; }

    public string Table { get; }

    public string ConnectionString { get; }

    public bool HasEnv => !string.IsNullOrEmpty(Env) && Env != "*";

    public bool HasHost => !string.IsNullOrEmpty(Host) && Host != "*";

    public Focus(string app, string env, string host, string connStr, string schema, string table)
    {
        App = app;
        Env = env;
        Host = host;
        ConnectionString = connStr;
        Schema = schema;
        Table = table;
    }

    public void ValidOrThrow(bool requiresApp = true)
    {
        // ReSharper disable NotResolvedInText
        if (requiresApp)
        {
            if (string.IsNullOrWhiteSpace(App) || !AppRgx.IsMatch(App))
            {
                throw new ArgumentException("Invalid application name", "app");
            }
        }

        if (string.IsNullOrWhiteSpace(Env) || (Env != "*" && !EnvRgx.IsMatch(Env)))
        {
            throw new ArgumentException("Invalid environment name", "env");
        }

        if (string.IsNullOrWhiteSpace(Host) || (Host != "*" && !HostRgx.IsMatch(Host)))
        {
            throw new ArgumentException("Invalid host name", "host");
        }

        if (string.IsNullOrWhiteSpace(ConnectionString)
            || (!EnvVarNameRgx.IsMatch(ConnectionString)
                && !ConnectionString.Contains("Database=", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Invalid connection string", "connStr");
        }

        if (string.IsNullOrWhiteSpace(Schema) || !SafeRgx.IsMatch(Schema))
        {
            throw new ArgumentException("Invalid database schema name", "schema");
        }

        if (string.IsNullOrWhiteSpace(Table) || !SafeRgx.IsMatch(Table))
        {
            throw new ArgumentException("Invalid database table name", "table");
        }
    }
}
