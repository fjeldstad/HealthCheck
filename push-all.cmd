@IF [%1] == [] GOTO PrintUsage
nuget push .\build\Hihaj.HealthCheck.%1.0.nupkg
nuget push .\build\Hihaj.HealthCheck.Redis.%1.0.nupkg
nuget push .\build\Hihaj.HealthCheck.Windows.%1.0.nupkg
nuget push .\build\Hihaj.HealthCheck.Nancy.%1.0.nupkg
@GOTO End

:PrintUsage
@ECHO Usage: push-all [version]

:End