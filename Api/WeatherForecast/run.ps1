using namespace System.Net
using namespace BlazorApp.Shared
# Input bindings are passed in via param block.
param($Request, $TriggerMetadata)

Add-Type -Path C:\Users\jgrote\Projects\AzurePrice\Shared\bin\Debug\netstandard2.0\Shared.dll

$Body = @(
    [WeatherForecast]@{
        Date = Get-Date
        TemperatureC = 53
        Summary = 'Sunny'
    }
    [WeatherForecast]@{
        Date = (Get-Date).AddDays(1)
        TemperatureC = 40
        Summary = 'Cloudy'
    }
)

# Associate values to output bindings by calling 'Push-OutputBinding'.
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $body
})
