using System.Text.Json;
using System.Web;
using AnydeskTracker.Controllers;
using AnydeskTracker.Data;
using AnydeskTracker.DTOs;

namespace AnydeskTracker.Services.MetrikaServices;

public class YandexMetrikaService(IHttpClientFactory httpClientFactory, ApplicationDbContext context)
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

    public async Task GetCurrentBrowserRevenue(string browser)
    {
        var browserModel = context.BrowserRevenues.FirstOrDefault(x => x.Browser == browser);
        
        if(browserModel == null)
            return;
            
        var requestDto = new BuildRequestDto();
        
        //TODO:Filters
        requestDto.Accounts.AddRange(YandexMetrikaController.Accounts.Select(x => x.Key));
        requestDto.Period = "today";
        requestDto.EntityFields.Add("browser");
        requestDto.Fields.Add("partner_wo_nds");
        
        var result = await BuildReportAsync(requestDto);
        if (result == null)
            return;

        var reward = result.Sum(x => x.Data.Points
            .FirstOrDefault(x => x.Dimensions["browser"].ToString() == browserModel.Browser)?.Measures
            .FirstOrDefault(d => d.ContainsKey("partner_wo_nds"))?
            .GetValueOrDefault("partner_wo_nds").GetDecimal() ?? 0);
        
        var previousReward = browserModel.LastRevenue;
        
        browserModel.DeltaRevenue = reward - previousReward;
        browserModel.LastRevenue = reward;

        await context.SaveChangesAsync();
    }
}

public class TableResult
{
    public List<string> ColumnKeys { get; set; } = new();
    public List<string> ColumnTitles { get; set; } = new();
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}