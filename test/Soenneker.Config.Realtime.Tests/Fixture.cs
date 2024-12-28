using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Soenneker.Fixtures.Unit;
using Soenneker.Utils.Test;
using Soenneker.Config.Realtime.Registrars;
using System.IO;

namespace Soenneker.Config.Realtime.Tests;

public class Fixture : UnitFixture
{
    public override System.Threading.Tasks.ValueTask InitializeAsync()
    {
        SetupIoC(Services);

        return base.InitializeAsync();
    }

    private static void SetupIoC(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });

        string directory = Directory.GetCurrentDirectory();

        const string baseAppSettings = "appsettings.json";

        string? environmentAppSettings = null;

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile(baseAppSettings);

        services.AddRealtimeConfiguration(builder);

        if (environmentAppSettings != null)
            builder.AddJsonFile(environmentAppSettings);

        IConfiguration configRoot = builder.Build();

        services.AddSingleton(configRoot);
    }
}
