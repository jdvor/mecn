# MECN (Microsoft.Extensions.Configuration.Npgsql)

PostgreSQL configuration provider for Microsoft.Extensions.Configuration system.

Features:
* Configure connection string explicitly or point it to environment variable.
* Choose database schema and table name through options.
* Provide different cofiguration data sets based on application name, environment name and a host.
* Have some configuration data shared across environments and hosts.

Part of the solution is a command line tool **mecn**, which can:
* initialize required database structure
* export configuration to JSON file
* import configuration from JSON file
* view particular key-value configuration data 
* set key-value configuration data


## Badges

[![Tests](https://github.com/jdvor/mecn/actions/workflows/test.yml/badge.svg?branch=main)](https://github.com/jdvor/mecn/actions/workflows/test.yml)


## Quickstart

nuget package [Mecn](https://www.nuget.org/packages/Mecn)


```shell
dotnet add package Mecn [ -v 1.0.0 ]
```

Setup Microsoft.Extensions.Configuration.ConfigurationBuilder:
```csharp
using Mecn;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();

builder.AddNpgsql(opts =>
{
    opts.ApplicationName = "myapp";
    opts.EnvironmentName = "stage";
    opts.ConnectionString = "MYAPP_CONNECTION_STRING"; // either connection string or environment variable name
});
```


## Useful commands (for contributors)

### Run tests
```shell
dotnet test -v minimal --nologo
```

### Build strict
(code analysis ON, warnings as errors)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release -v minimal -p:TreatWarningsAsErrors=True --nologo -clp:NoSummary
```

### Build permissive
(code analysis OFF)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release -v minimal -p:RunAnalyzers=False --nologo
```

### Tag version
```shell
git tag -a "2.0.0" -m "version 2.0.0" --force [ commit ]
git push --tags
```

### Create NuGet package (CI variant)
```shell
./pack.sh -c
```

### Create NuGet package (local development)
```shell
./pack.sh [ -v {version_prefix} ] [ -s {version_suffix} ] [ -p {nuget_package_cache} ]
```

### Create tools (mecn)
```shell
./tool.sh
```

Creates **mecn** CLI in `publish/windows` and `publish/linux` directories.


### Test coverage & report
```shell
# https://github.com/coverlet-coverage/coverlet
dotnet tool install -g coverlet.console

# https://github.com/danielpalme/ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test --collect:"XPlat Code Coverage" --results-directory publish/coverage
reportgenerator -reports:publish/coverage/**/coverage.cobertura.xml -targetdir:publish/report -reporttypes:HtmlInline
```

Then you can find results in `./publish/report` directory.
