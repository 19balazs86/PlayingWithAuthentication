using SharedLib;

namespace BlazorOidc.Services;

public sealed class ServerWeatherForecaster(ILogger<ServerWeatherForecaster> _logger) : IWeatherForecaster
{
    public async Task<WeatherForecast[]> GetWeatherForecasts(int numberOfForecasts)
    {
        _logger.LogInformation("Fetching weathers using {ImplementationType}", nameof(ServerWeatherForecaster));

        await Task.Delay(500);

        return WeatherForecast.CreateMore(numberOfForecasts);
    }
}
