using BlazorWASM.BFF.Auth0.OpenIDConnect.Shared.DTO;

namespace BlazorWASM.BFF.Auth0.OpenIDConnect.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        return WeatherForecast.GetRandomForecasts();
    }

    [HttpPost]
    public WeatherForecast PostWeatherForecast(WeatherForecast forecast)
    {
        forecast.Summary = "Server accepted. " + forecast.Summary;

        return forecast;
    }
}
