[![](https://img.shields.io/nuget/v/soenneker.config.realtime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.config.realtime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.config.realtime/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.config.realtime/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.config.realtime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.config.realtime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.config.realtime/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.config.realtime/actions/workflows/codeql.yml)

# Soenneker.Config.Realtime

Adds a process-local, mutable configuration provider whose changes publish standard .NET configuration reload notifications.

## Install

```bash
dotnet add package Soenneker.Config.Realtime
```

## Register with an application

Add the provider after the configuration sources it should override:

```csharp
using Soenneker.Config.Realtime.Registrars;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRealtimeConfiguration(builder.Configuration);

var app = builder.Build();
```

This adds one provider to `builder.Configuration` and registers that same instance as `IRealtimeConfigurationProvider`.

Outside dependency injection, add the source directly and retain the returned provider:

```csharp
IRealtimeConfigurationProvider realtime = configurationBuilder.AddRealtimeConfiguration();
IConfigurationRoot configuration = configurationBuilder.Build();
```

## Update values

```csharp
using Soenneker.Config.Realtime.Abstract;

public sealed class FeatureControl(IRealtimeConfigurationProvider realtime)
{
    public void EnableCheckout() => realtime.Set("Features:Checkout", "true");

    public void RemoveOverride() => realtime.Remove("Features:Checkout");
}
```

Keys are case-insensitive and use the normal `:` hierarchy delimiter. `Set` publishes a reload only when the effective value in this provider changes. Passing `null` removes the key. `Remove` does nothing when the key is absent.

Because this provider is added last in the examples, its values override earlier JSON, environment-variable, and command-line providers. Removing an override may reveal a value from an earlier provider rather than making the composed configuration value null.

## Observe changes

Read through `IConfiguration` for the latest value. For bound options that should react to reload notifications, use `IOptionsMonitor<T>`:

```csharp
public sealed class CheckoutWorker(IOptionsMonitor<FeatureOptions> options)
{
    public bool IsEnabled => options.CurrentValue.Checkout;
}
```

## Operational notes

- Changes exist only in the current process and are lost on restart. The package does not synchronize instances or persist values.
- Reload callbacks run as part of the configuration notification flow. Keep callbacks short and avoid calling `Set` recursively from them.
- Values are stored as strings, matching the .NET configuration model. Binding and validation happen in the consuming configuration or options layer.
- Runtime configuration can contain secrets. Restrict access to the provider and avoid logging values.
