using ApiKeyAuth.Solutions;
using Microsoft.AspNetCore.Mvc;

namespace ApiKeyAuth.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    [HttpGet]
    [ApiKeyAuth] // TODO: Solution 2/b - Apply AuthorizationFilter individually with the attribute
    //[ServiceFilter(typeof(ApiKeyAuthFilter))] // This is ugly, instead use the custom ApiKeyAuth attribute
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
