namespace Mecn;

using global::Npgsql;
using Microsoft.Extensions.Configuration;

public class NpgsqlConfigurationProvider : ConfigurationProvider
{
    private readonly Options options;

    public NpgsqlConfigurationProvider(Options options)
    {
        options.EnsureValidOrThrow();
        this.options = options;
    }

    public override void Load()
    {
        Data = LoadAsync(options).GetAwaiter().GetResult();
    }

    private static async Task<Dictionary<string, string>> LoadAsync(Options options)
    {
        try
        {
            var connStr = NormalizeConnectionString(options.ConnectionString);
            var sql = CreateSqlQuery(options);
            await using var conn = new NpgsqlConnection(connStr);
            await conn.OpenAsync().ConfigureAwait(false);
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("app", options.ApplicationName);
            cmd.Parameters.AddWithValue("env", options.EnvironmentName);
            if (options.HasHostName)
            {
                cmd.Parameters.AddWithValue("host", options.HostName);
            }

            var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                return new Dictionary<string, string>(0);
            }

            var result = new Dictionary<string, string>();
            while (await reader.ReadAsync())
            {
                var key = reader.GetString(0);
                var value = reader.GetString(1);
                result[key] = value;
            }

            return result;
        }
        catch (NpgsqlException) when (options.Optional)
        {
            // the provider is optional, so we can be silent here.
        }

        return new Dictionary<string, string>(0);
    }

    private static string CreateSqlQuery(Options options)
    {
        var where = options.HasHostName
            ? "app = @app AND (env = '*' OR env = @env) AND (host = '*' OR host = @host) ORDER BY env, host, key"
            : "app = @app AND (env = '*' OR env = @env) AND host = '*' ORDER BY env, host, key";
        return $"SELECT key, value FROM {options.DbSchema}.{options.DbTable} WHERE " + where;
    }

    private static string NormalizeConnectionString(string connStr)
    {
        if (!string.IsNullOrEmpty(connStr) && connStr.Contains("Database="))
        {
            return connStr;
        }

        var csEnv = Environment.GetEnvironmentVariable(connStr);
        if (!string.IsNullOrEmpty(csEnv) && csEnv.Contains("Database="))
        {
            return csEnv;
        }

        throw ConfigurationException.InvalidConnectionString();
    }
}
