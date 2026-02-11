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
        request.Headers.Add("Authorization", Environment.GetEnvironmentVariable("YANDEX_API_BIRD_MILK"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<YandexApiResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<List<YandexReportResponse>?> BuildReportAsync(BuildRequestDto requestDto)
    {
        var responses = new List<YandexReportResponse>();
        foreach (var keyName in requestDto.Accounts)
        {
            var accountName = keyName.Replace("YANDEX_API_", "");
            var builder = new UriBuilder("https://partner.yandex.ru/api/statistics2/get");
            var query = HttpUtility.ParseQueryString(builder.Query);

            query["lang"] = "ru";
            query["period"] = requestDto.Period;

            foreach (var dimension in requestDto.Dimensions)
                if (!string.IsNullOrEmpty(dimension.Value))
                    query.Add("dimension_field", $"{dimension.Key}|{dimension.Value}");

            foreach (var fieldValue in requestDto.Fields)
                query.Add("field", fieldValue);

            foreach (var entityField in requestDto.EntityFields)
                query.Add("entity_field", entityField);

            builder.Query = query.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            request.Headers.Add("Authorization",
                Environment.GetEnvironmentVariable(keyName));
            Console.WriteLine($"Account {keyName}");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var reportResponse = JsonSerializer.Deserialize<YandexReportResponse>(json);
            reportResponse.AccountName = accountName;
            responses.Add(reportResponse);
        }

        return responses;
    }
}

public class TableResult
{
    public List<string> ColumnKeys { get; set; } = new();
    public List<string> ColumnTitles { get; set; } = new();
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}