namespace Mecn.Tests;

using Xunit;

public class OptionsTests
{
    [InlineData("MECN_CONNECTION_STING")]
    [InlineData("Host=localhost;Port=5432;Username=dev;Password=dev;Database=mecn;ApplicationName=mecn")]
    [Theory]
    public void ValidConnectionString(string cs)
    {
        var options = new Options
        {
            ApplicationName = "example",
            ConnectionString = cs,
        };
        options.EnsureValidOrThrow();
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("un!safe")]
    [Theory]
    public void InvalidApplicationName(string appName)
    {
        var options = new Options
        {
            ApplicationName = appName,
            ConnectionString = "MECN_CONNECTION_STING",
        };
        Assert.Throws<ConfigurationException>(() => options.EnsureValidOrThrow());
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("notconnection")]
    [InlineData("lorem ipsum")]
    [Theory]
    public void InvalidConnectionString(string cs)
    {
        var options = new Options
        {
            ApplicationName = "example",
            ConnectionString = cs,
        };
        Assert.Throws<ConfigurationException>(() => options.EnsureValidOrThrow());
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("lorem$ipsum")]
    [Theory]
    public void InvalidDbSchema(string dbSchema)
    {
        var options = new Options
        {
            ApplicationName = "example",
            ConnectionString = "MECN_CONNECTION_STING",
            DbSchema = dbSchema,
        };
        Assert.Throws<ConfigurationException>(() => options.EnsureValidOrThrow());
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("lorem$ipsum")]
    [Theory]
    public void InvalidDbTable(string dbTable)
    {
        var options = new Options
        {
            ApplicationName = "example",
            ConnectionString = "MECN_CONNECTION_STING",
            DbTable = dbTable,
        };
        Assert.Throws<ConfigurationException>(() => options.EnsureValidOrThrow());
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("un?safe")]
    [Theory]
    public void InvalidEnvironmentName(string envName)
    {
        var options = new Options
        {
            ApplicationName = "example",
            ConnectionString = "MECN_CONNECTION_STING",
            EnvironmentName = envName,
        };
        Assert.Throws<ConfigurationException>(() => options.EnsureValidOrThrow());
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("un?safe")]
    [Theory]
    public void InvalidHostName(string hostName)
    {
        var options = new Options
        {
            ApplicationName = "example",
            ConnectionString = "MECN_CONNECTION_STING",
            HostName = hostName,
        };
        Assert.Throws<ConfigurationException>(() => options.EnsureValidOrThrow());
    }
}
