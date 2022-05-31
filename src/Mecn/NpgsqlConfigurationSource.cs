namespace Mecn;

using Microsoft.Extensions.Configuration;

public class NpgsqlConfigurationSource : IConfigurationSource
{
    private readonly Options options;

    public NpgsqlConfigurationSource(Options options)
    {
        this.options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new NpgsqlConfigurationProvider(options);
    }
}
