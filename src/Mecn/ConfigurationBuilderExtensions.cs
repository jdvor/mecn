namespace Mecn;

using Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddNpgsql(
        this IConfigurationBuilder builder,
        string applicationName,
        string environmentName = "prod",
        string? connectionString = null)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            var csEnvVarName = $"{applicationName.ToUpperInvariant()}_CONNECTION_STRING";
            var cs = Environment.GetEnvironmentVariable(csEnvVarName);
            connectionString = string.IsNullOrEmpty(cs)
                ? throw new ArgumentException(NoConnectionMsg(csEnvVarName), nameof(connectionString))
                : cs;
        }

        var options = new Options
        {
            ApplicationName = applicationName,
            EnvironmentName = environmentName,
            ConnectionString = connectionString,
        };
        return AddNpgsql(builder, options);
    }

    private static string NoConnectionMsg(string csEnvVarName)
    {
        return "Connection string must be either explicitly set or environment variable "
               + $"'{csEnvVarName}' must exist with non-empty value.";
    }

    public static IConfigurationBuilder AddNpgsql(this IConfigurationBuilder builder, Action<Options> configure)
    {
        var options = new Options();
        configure(options);
        return AddNpgsql(builder, options);
    }

    public static IConfigurationBuilder AddNpgsql(this IConfigurationBuilder builder, Options options)
    {
        var source = new NpgsqlConfigurationSource(options);
        builder.Add(source);
        return builder;
    }
}
