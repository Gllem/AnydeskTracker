using System.Text.Json;
using System.Web;
using AnydeskTracker.DTOs;

namespace AnydeskTracker.Services.MetrikaServices;

public class YandexMetrikaService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("YandexClient");

    public async Task<YandexApiResponse?> GetTreeAsync()
    {
        var builder = new UriBuilder("https://partner.yandex.ru/api/statistics2/tree");
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["lang"] = "ru";
        query["pretty"] = "1";
        query["stat_type"] = "main";
        builder.Query = query.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        request.Headers.Add("Authorization", Environment.GetEnvironmentVariable("YANDEX_METRIKA"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<YandexApiResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<YandexReportResponse?> BuildReportAsync(Dictionary<string, string> requestDimensions,
        List<string> requestFields, string requestPeriod)
    {
        var builder = new UriBuilder("https://partner.yandex.ru/api/statistics2/get");
        var query = HttpUtility.ParseQueryString(builder.Query);

        query["lang"] = "ru";
        query["period"] = requestPeriod;
        Console.WriteLine($"Period {requestPeriod}");
        query["entity_field"] = "page_caption";

        foreach (var dimension in requestDimensions)
        {
            if (!string.IsNullOrEmpty(dimension.Value))
                query.Add("dimension_field", $"{dimension.Key}|{dimension.Value}");
        }

        foreach (var fieldValue in requestFields)
        {
            query.Add("field", fieldValue);
        }

        builder.Query = query.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        request.Headers.Add("Authorization",
            Environment.GetEnvironmentVariable("YANDEX_METRIKA"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<YandexReportResponse>(json);
    }
    
   
}
public class TableResult
{
    public List<string> ColumnKeys { get; set; } = new();
    public List<string> ColumnTitles { get; set; } = new();
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}
