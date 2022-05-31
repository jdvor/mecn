using Microsoft.Extensions.Configuration;
using Mecn;

var builder = new ConfigurationBuilder();

builder.AddNpgsql(opts =>
{
    opts.ApplicationName = "example";
    opts.EnvironmentName = "stage";
    opts.ConnectionString = "MECN_CONNECTION_STRING"; // either connection string or environment variable name
    opts.DbSchema = "example";
});

IConfiguration cfg = builder.Build();

var expectedKeys = new[] { "key1", "root:key2", "root:key3" };
var maxKeyLength = expectedKeys.Max(x => x.Length);
foreach (var key in expectedKeys)
{
    var value = cfg.GetValue<string>(key);
    Console.WriteLine("{0} = {1}", key.PadRight(maxKeyLength), value is null ? "NULL" : $"\"{value}\"" );
}
