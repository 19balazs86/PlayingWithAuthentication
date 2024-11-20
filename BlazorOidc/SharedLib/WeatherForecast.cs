namespace SharedLib;

public sealed class WeatherForecast
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private static readonly string[] _cities =
    [
        "London", "Budapest", "Paris", "Dublin", "Zurich", "Los Angeles", "New York"
    ];

    public required string   City        { get; init; }
    public required DateTime Date        { get; init; }
    public required int      Temperature { get; init; }
    public required string   Summary     { get; init; }

    public static WeatherForecast Create(int addDays = 1)
    {
        return new WeatherForecast
        {
            City        = _cities[Random.Shared.Next(_cities.Length)],
            Date        = DateTime.UtcNow.AddDays(addDays),
            Temperature = Random.Shared.Next(-20, 55),
            Summary     = _summaries[Random.Shared.Next(_summaries.Length)]
        };
    }

    public static WeatherForecast[] CreateMore(int count = 5)
    {
        return Enumerable.Range(1, count)
                         .Select(Create)
                         .ToArray();
    }
}
