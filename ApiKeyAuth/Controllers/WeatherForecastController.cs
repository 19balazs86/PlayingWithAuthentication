using ApiKeyAuth.Solutions;
using Microsoft.AspNetCore.Mvc;

namespace ApiKeyAuth.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    [HttpGet]
    [ApiKeyAuth] // TODO: Solution 2/b - Apply authorization filter individually
    //[ServiceFilter(typeof(ApiKeyAuthFilter))] // Instead of this, use the custom ApiKeyAuth attribute
    public IEnumerable<WeatherForecast> Get()
    {
        return WeatherForecast.GetRandomForecasts();
    }

    [HttpGet("Anonymous")]
    public IEnumerable<WeatherForecast> GetAnonymous()
    {
        return WeatherForecast.GetRandomForecasts();
    }
}
