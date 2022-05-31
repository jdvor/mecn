namespace Mecn.Cli;

using global::Npgsql;

internal static class SetAction
{
    internal static async Task<int> ExecuteAsync(Focus focus, string key, string value)
    {
        var connectionString = Util.NormalizeConnectionString(focus.ConnectionString);
        var sql1 = CreateSqlQuery(focus);
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync().ConfigureAwait(false);

        await using var cmd1 = new NpgsqlCommand(sql1, conn);
        cmd1.Parameters.AddWithValue("app", focus.App);
        cmd1.Parameters.AddWithValue("env", focus.Env);
        cmd1.Parameters.AddWithValue("host", focus.Host);
        cmd1.Parameters.AddWithValue("key", key);
        cmd1.Parameters.AddWithValue("value", value);
        var affected = await cmd1.ExecuteNonQueryAsync().ConfigureAwait(false);

        return affected == 1 ? 0 : 1;
    }

    private static string CreateSqlQuery(Focus focus)
    {
        return $"INSERT INTO {focus.Schema}.{focus.Table} " +
               "(app, env, host, key, value) VALUES (@app, @env, @host, @key, @value)" +
               $"ON CONFLICT ON CONSTRAINT {focus.Table}_pk DO UPDATE SET value = @value";
    }
}
