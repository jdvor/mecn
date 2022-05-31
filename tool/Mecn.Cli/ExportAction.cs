namespace Mecn.Cli;

using global::Npgsql;
using System.Text;
using System.Text.Json;

internal static class ExportAction
{
    internal static async Task<int> ExecuteAsync(Focus focus, OutputFormat format)
    {
        var data = await GetDataAsync(focus).ConfigureAwait(false);
        if (data.Count == 0)
        {
            return 0;
        }

        switch (format)
        {
            case OutputFormat.Json:
                WriteJson(data);
                break;

            case OutputFormat.Default:
            default:
                WriteDefault(data);
                break;
        }

        return 0;
    }

    private static string CreateSqlQuery(Focus focus)
    {
        var sb = new StringBuilder();
        sb.Append("select key, value from ");
        sb.Append(focus.Schema);
        sb.Append('.');
        sb.Append(focus.Table);
        sb.Append(" where app = @app");
        if (focus.HasEnv)
        {
            sb.Append(" and (env = '*' or env = @env)");
        }
        else
        {
            sb.Append(" and env = '*'");
        }

        if (focus.HasHost)
        {
            sb.Append(" and (host = '*' or host = @host)");
        }
        else
        {
            sb.Append(" and host = '*'");
        }

        sb.Append(" order by env, host, key");

        return sb.ToString();
    }

    internal static async Task<Dictionary<string, string>> GetDataAsync(Focus focus)
    {
        var connectionString = Util.NormalizeConnectionString(focus.ConnectionString);
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync().ConfigureAwait(false);
        var sql = CreateSqlQuery(focus);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("app", focus.App);
        if (focus.HasEnv)
        {
            cmd.Parameters.AddWithValue("env", focus.Env);
        }

        if (focus.HasHost)
        {
            cmd.Parameters.AddWithValue("host", focus.Host);
        }

        var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            return new Dictionary<string, string>(0);
        }

        var data = new Dictionary<string, string>();
        while (await reader.ReadAsync())
        {
            var key = reader.GetString(0);
            var value = reader.GetString(1);
            data[key] = value;
        }

        return data;
    }

    private static void WriteDefault(Dictionary<string, string> data)
    {
        var maxKeyLength = data.Keys.Max(x => x.Length);
        foreach (var kvp in data)
        {
            Console.WriteLine("{0} = \"{1}\"", kvp.Key.PadRight(maxKeyLength), kvp.Value);
        }
    }

    private static void WriteJson(Dictionary<string, string> data)
    {
        var hier = Util.ToHierarchy(data);
        var jsonOpts = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(hier, jsonOpts);
        Console.WriteLine(json);
    }
}
