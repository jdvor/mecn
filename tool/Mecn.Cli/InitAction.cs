namespace Mecn.Cli;

using global::Npgsql;
using System.Text;

internal static class InitAction
{
    internal static async Task<int> ExecuteAsync(string connStr, string schema, string table)
    {
        var connectionString = Util.NormalizeConnectionString(connStr);
        var sql1 = GetSql("init.sql", schema, table);
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync().ConfigureAwait(false);

        await using var cmd1 = new NpgsqlCommand(sql1, conn);
        await cmd1.ExecuteNonQueryAsync().ConfigureAwait(false);

        return 0;
    }

    private static string GetSql(string baseName, string schema, string table)
    {
        var type = typeof(InitAction);
        var resName = $"{type.Namespace}.{baseName}";
        using var stream = type.Assembly.GetManifestResourceStream(resName);
        using var reader = new StreamReader(stream!, Encoding.UTF8, false);
        var sql = reader.ReadToEnd();
        sql = sql.Replace(@"$schema", schema);
        sql = sql.Replace(@"$table", table);
        return sql;
    }
}
