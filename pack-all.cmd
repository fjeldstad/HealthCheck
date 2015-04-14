nuget pack .\src\HealthCheck.Core\HealthCheck.Core.csproj -Build -Prop Configuration=Release -Symbols -OutputDirectory .\build
nuget pack .\src\HealthCheck.Redis\HealthCheck.Redis.csproj -Build -Prop Configuration=Release -Symbols -OutputDirectory .\build
nuget pack .\src\HealthCheck.Windows\HealthCheck.Windows.csproj -Build -Prop Configuration=Release -Symbols -OutputDirectory .\build
nuget pack .\src\HealthCheck.Nancy\HealthCheck.Nancy.csproj -Build -Prop Configuration=Release -Symbols -OutputDirectory .\build