[![](https://img.shields.io/nuget/v/soenneker.config.realtime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.config.realtime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.config.realtime/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.config.realtime/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.config.realtime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.config.realtime/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Config.Realtime

### Real-time, thread-safe configuration updates for .NET applications.

## Features

- **Real-Time Updates**: Dynamically add, update, or remove configuration values at runtime.
- **Thread-Safe**: Safe updates in multi-threaded environments.
- **Hierarchical Keys**: Supports nested keys using `:` as a separator (e.g., `AppSettings:FeatureFlag`).
- **Automatic Propagation**: Updates are reflected in `IConfiguration` consumers.

---

## Installation

```bash
dotnet add package Soenneker.Config.Realtime
```

You just need access to `IServiceCollection` and `IBuilderConfiguration`:

```csharp
serviceCollection.AddRealtimeConfiguration(builderConfiguration);
```

This is just one example:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add and register the RealtimeConfigurationProvider in one step
builder.Services.AddRealtimeConfiguration(builder.Configuration);

var app = builder.Build();
```

## Usage

```csharp
public class MyService
{
    private readonly IRealtimeConfigurationProvider _configProvider;
    private readonly IConfiguration _config;

    // Simply inject IRealtimeConfigurationProvider:
    public MyService(IRealtimeConfigurationProvider configProvider, IConfiguration config)
    {
        _configProvider = configProvider;
        _config = config;
    }

    public void SetKeyDynamically()
    {
        // Make your updates
        _configProvider.Set("SomeKey", "SomeValue");
    }

    public void ReadUpdatedValue()
    {
        // IConfiguration is now updated
        string newValue = _config["SomeKey"]; // Outputs: "SomeValue"
    }
}
```