namespace ServiceA;

public interface IServiceBProxy
{
    Task<WeatherForecast[]> GetWeatherForecast();
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);

public class ServiceBProxy : IServiceBProxy
{
    private readonly HttpClient _httpClient;

    public ServiceBProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    async Task<WeatherForecast[]> IServiceBProxy.GetWeatherForecast()
    {
        var forecast = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast");
        if (forecast != null) return forecast;
        throw new Exception("Failed to retrieve weather forecast");
    }
}