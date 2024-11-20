using SharedLib;
using System.Net.Http.Json;

namespace BlazorOidc.Client.Services;

public sealed class ClientWeatherForecaster(ILogger<ClientWeatherForecaster> _logger, HttpClient _httpClient) : IWeatherForecaster
{
    public async Task<WeatherForecast[]> GetWeatherForecasts(int numberOfForecasts)
    {
        // This log message will appear in the browser console
        _logger.LogInformation("Fetching weathers using {ImplementationType}", nameof(ClientWeatherForecaster));

        await Task.Delay(500);

        return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("/api/weather-forecast")
               ?? throw new IOException("No weather forecast!");
    }
}
