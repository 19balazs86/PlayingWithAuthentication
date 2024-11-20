using SharedLib;

namespace BlazorOidc.Endpoints;

public static class WeatherForecastEndpoints
{
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/weather-forecast", getWeatherForecast).RequireAuthorization();

        return app;
    }

    private static WeatherForecast[] getWeatherForecast(int? numberOfDays = null)
    {
        return WeatherForecast.CreateMore(numberOfDays ?? 7);
    }
}
