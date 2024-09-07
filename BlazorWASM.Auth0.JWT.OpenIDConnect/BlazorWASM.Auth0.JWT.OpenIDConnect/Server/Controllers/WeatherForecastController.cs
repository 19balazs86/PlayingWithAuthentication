namespace BlazorWASM.Auth0.JWT.OpenIDConnect.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return WeatherForecast.GetRandomForecasts();
    }
}
