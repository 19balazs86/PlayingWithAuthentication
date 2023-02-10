namespace ApiKeyAuth;

public sealed class WeatherForecast
{
    private static readonly string[] _summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; }

    public static WeatherForecast[] GetRandomForecasts(int n = 5)
    {
        return Enumerable.Range(1, n).Select(index => new WeatherForecast
        {
            Date         = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary      = _summaries[Random.Shared.Next(_summaries.Length)]
        })
       .ToArray();
    }
}
