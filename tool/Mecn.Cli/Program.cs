using Mecn.Cli;
using System.CommandLine;

int returnCode = 100;

var appArg = new Argument<string>
        (name: "application",
        description: "Application name");

var keyArg = new Argument<string>
    (name: "key",
    description: "Configuration key");

var valueArg = new Argument<string>
    (name: "value",
    description: "Configuration value");

var inputFileArg = new Argument<string>
    (name: "input-file",
    description: "Path to a JSON file to import");

var schemaOption = new Option<string>
    (aliases: new[] {"--schema", "-s"},
    description: "PostgreSQL database schema name",
    getDefaultValue: () => "public");

var tableOption = new Option<string>
    (aliases: new[] {"--table", "-t"},
    description: "PostgreSQL database table name",
    getDefaultValue: () => "appsettings");

var envOption = new Option<string>
    (aliases: new[] {"--environment", "-e"},
    description: "Environment name",
    getDefaultValue: () => "*");

var hostOption = new Option<string>
    (aliases: new[] {"--host", "-o"},
    description: "Host name",
    getDefaultValue: () => "*");

var connStrOption = new Option<string>
    (aliases: new[] {"--connection-string", "-c"},
    description: "PostgreSQL connection string (Npgsql driver) or environment variable name which contains it",
    getDefaultValue: () => "MECN_CONNECTION_STRING");

var formatOption = new Option<OutputFormat>
    (name: "--format",
    description: "Output format",
    getDefaultValue: () => OutputFormat.Default);

var initCmd = new Command("init", "Initialize underlying database storage");
initCmd.AddOption(connStrOption);
initCmd.AddOption(schemaOption);
initCmd.AddOption(tableOption);
initCmd.SetHandler(
    async (string connStr, string schema, string table) =>
    {
        returnCode = await InitAction.ExecuteAsync(connStr, schema, table);
    },
    connStrOption, schemaOption, tableOption);

var setCmd = new Command("set", "Set key-value");
setCmd.AddArgument(appArg);
setCmd.AddArgument(keyArg);
setCmd.AddArgument(valueArg);
setCmd.AddOption(envOption);
setCmd.AddOption(hostOption);
setCmd.AddOption(connStrOption);
setCmd.AddOption(schemaOption);
setCmd.AddOption(tableOption);
setCmd.SetHandler(
    async (string app, string env, string host, string key, string value, string connStr, string schema, string table) =>
    {
        var focus = new Focus(app, env, host, connStr, schema, table);
        focus.ValidOrThrow();
        returnCode = await SetAction.ExecuteAsync(focus, key, value);
    },
    appArg, envOption, hostOption, keyArg, valueArg, connStrOption, schemaOption, tableOption);

var importCmd = new Command("import", "Import key-value pairs from JSON file");
importCmd.AddArgument(appArg);
importCmd.AddArgument(inputFileArg);
importCmd.AddOption(envOption);
importCmd.AddOption(hostOption);
importCmd.AddOption(connStrOption);
importCmd.AddOption(schemaOption);
importCmd.AddOption(tableOption);
importCmd.SetHandler(
    async (string app, string env, string host, string inputFile, string connStr, string schema, string table) =>
    {
        var focus = new Focus(app, env, host, connStr, schema, table);
        focus.ValidOrThrow();
        returnCode = await ImportAction.ExecuteAsync(focus, inputFile);
    },
    appArg, envOption, hostOption, inputFileArg, connStrOption, schemaOption, tableOption);

var exportCmd = new Command("export", "Export key-value pairs to JSON file");
exportCmd.AddArgument(appArg);
exportCmd.AddOption(formatOption);
exportCmd.AddOption(envOption);
exportCmd.AddOption(hostOption);
exportCmd.AddOption(connStrOption);
exportCmd.AddOption(schemaOption);
exportCmd.AddOption(tableOption);
exportCmd.SetHandler(
    async (string app, string env, string host, OutputFormat format, string connStr, string schema, string table) =>
    {
        var focus = new Focus(app, env, host, connStr, schema, table);
        focus.ValidOrThrow();
        returnCode = await ExportAction.ExecuteAsync(focus, format);
    },
    appArg, envOption, hostOption, formatOption, connStrOption, schemaOption, tableOption);


var root = new RootCommand
{
    initCmd,
    setCmd,
    importCmd,
    exportCmd,
};

await root.InvokeAsync(args);

return returnCode;
