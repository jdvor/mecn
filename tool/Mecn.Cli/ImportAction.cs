namespace Mecn.Cli;

using global::Npgsql;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

internal static class ImportAction
{
    internal static async Task<int> ExecuteAsync(Focus focus, string inputFile)
    {
        var data = LoadDataFromFile(inputFile);
        if (data.Count == 0)
        {
            return 1;
        }

        var connectionString = Util.NormalizeConnectionString(focus.ConnectionString);
        var sql = CreateSqlQuery(focus);

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync().ConfigureAwait(false);

        await using var txn = await conn.BeginTransactionAsync().ConfigureAwait(false);

        var affected = 0;
        foreach (var kvp in data)
        {
            await using var cmd = new NpgsqlCommand(sql, conn, txn);
            cmd.Parameters.AddWithValue("app", focus.App);
            cmd.Parameters.AddWithValue("env", focus.Env);
            cmd.Parameters.AddWithValue("host", focus.Host);
            cmd.Parameters.AddWithValue("key", kvp.Key);
            cmd.Parameters.AddWithValue("value", kvp.Value);
            var r = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            affected += r;
        }

        await txn.CommitAsync().ConfigureAwait(false);

        return affected == data.Count ? 0 : 2;
    }

    private static string CreateSqlQuery(Focus focus)
    {
        return $"INSERT INTO {focus.Schema}.{focus.Table} " +
               "(app, env, host, key, value) VALUES (@app, @env, @host, @key, @value)" +
               $"ON CONFLICT ON CONSTRAINT {focus.Table}_pk DO UPDATE SET value = @value";
    }

    private static Dictionary<string, string> LoadDataFromFile(string path)
    {
        var data = new Dictionary<string, string>();
        var json = File.ReadAllText(path);
        var je = JsonSerializer.Deserialize<JsonElement>(json);
        var enumerator = je.EnumerateObject();
        while (enumerator.MoveNext())
        {
            var curr = enumerator.Current;
            Flatten(curr.Value, data, curr.Name);
        }

        return data;
    }

    private static readonly Regex KeyRgx = new Regex(
        @"^\w+(\:\w+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static void Flatten(JsonElement je, Dictionary<string, string> data, string accKey)
    {
        switch (je.ValueKind)
        {
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
                if (!KeyRgx.IsMatch(accKey))
                {
                    throw new ArgumentException("key");
                }

                data[accKey] = je.ToString();
                break;

            case JsonValueKind.Object:
                var enumerator = je.EnumerateObject();
                while (enumerator.MoveNext())
                {
                    var curr = enumerator.Current;
                    Flatten(curr.Value, data, $"{accKey}:{curr.Name}");
                }

                break;

            case JsonValueKind.Array:
                throw new ArgumentException("array");

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                break;
        }
    }
}
