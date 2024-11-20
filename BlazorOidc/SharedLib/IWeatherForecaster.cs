namespace SharedLib;

public interface IWeatherForecaster
{
    // This interface has 2 implementations because of the nature of Blazor pre-render mode
    // Even when the render mode is WebAssembly, the server still renders the page during pre-rendering
    // It looks for the interface on the server side NOT on the client side
    Task<WeatherForecast[]> GetWeatherForecasts(int numberOfForecasts);
}
