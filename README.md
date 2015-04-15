# HealthCheck - flexible, asynchronous health checks for your apps

[![Build status](https://img.shields.io/appveyor/ci/hihaj/healthcheck/master.svg)](https://ci.appveyor.com/project/Hihaj/healthcheck/branch/master)
[![Lastest NuGet package version](https://img.shields.io/nuget/v/Hihaj.HealthCheck.svg)](https://www.nuget.org/packages?q=hihaj.healthcheck)

Heavily inspired by [Runscope/healthcheck](https://github.com/Runscope/healthcheck), HealthCheck provides a simple but flexible way to monitor application health, including OS resources, third-party service status etc.

## High-level overview

A *health check* runs one or more independent *checks* that either pass or not depending on some specific *metric(s)*. The health check as a whole is successful if (and only if) all checks pass.

A conceptual example could be to require that CPU usage is below 80% and Redis is available.

## Getting started

HealthCheck can be used in any type of application - just create an instance of `HealthCheck`, add some `IChecker` implementations and run it. A simple example that uses a built-in checker from the `Hihaj.HealthCheck.Windows` package:

```c#
var healthCheck = new HealthCheck(new IChecker[]
{
    new SystemDriveHasFreeSpace(new AvailableSystemDriveSpace())
});
var result = await healthCheck.Run();

Console.WriteLine(result.Status); // Success or Failure
foreach (var checkResult in result.Results)
{
    Console.WriteLine(checkResult.Checker); // E.g. "System drive has free space"
    Console.WriteLine(checkResult.Passed);  // true/false
    Console.WriteLine(checkResult.Output);  // E.g. "123.4 GB available"
}
```

### Bonus: Nancy health check module

If you're building a [Nancy](https://github.com/NancyFx) web app, things are even simpler. Just install the `Hihaj.HealthCheck.Nancy` package and add the `HealthCheckModule` to the `ConfigureApplicationContainer` method of your bootstrapper:

```c#
container.RegisterMultiple<IChecker>(new[]
{
    typeof(SystemDriveHasFreeSpace)
});
container.Register<HealthCheckModule>();
```

This will expose the health check on the default url (`/healthcheck`), require https and use default options for all `IChecker` implementations that you specified.

If you need more control, you can register non-default options:

```c#
container.Register(new SystemDriveHasFreeSpace.Options
{
  SystemDriveAvailableFreeSpaceWarningThresholdInBytes =
    5L * 1024L * 1024L * 1024L; // 5 GB
});
container.RegisterMultiple<IChecker>(new[]
{
    typeof(SystemDriveHasFreeSpace)
});
container.Register(new HealthCheckOptions
{
    Route = "/healthcheck",
    RequireHttps = true,
    AuthorizationCallback = context => Task.FromResult(true)
});
container.Register<HealthCheckModule>();
```

When issuing a GET request for the health check url (using the configured route or the default `/healthcheck` if none provided), you get back the complete result of the health check in JSON format. For example:

```json
{
  "results": [
    {
      "checker": "CPU usage is OK",
      "passed": true,
      "output": "Currently using 4,2 % of total processor time."
    },
    {
      "checker": "Redis has free memory",
      "passed": true,
      "output": "1 % of the reserved memory is used (507,8 MB free)."
    },
    {
      "checker": "Redis is available",
      "passed": true,
      "output": "Version 2.8.19, uptime 1 days 12 hours 51 minutes."
    },
    {
      "checker": "System drive has free space",
      "passed": true,
      "output": "84,9 GB available."
    }
  ],
  "passed": true,
  "status": "success"
}
```

This endpoint can easily be monitored continously by a tool or service such as [Runscope](https://www.runscope.com/) (in fact, the JSON output is compatible with the one produced by [Runscope/healthcheck](https://github.com/Runscope/healthcheck)).
