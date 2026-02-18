using System.Text.Json;
using System.Text.Json.Serialization;
using AnydeskTracker.Controllers;

namespace AnydeskTracker.DTOs;

public class YandexApiResponse
{
    [JsonPropertyName("data")] public YandexData Data { get; set; }
}

public class YandexData
{
    [JsonPropertyName("tree")] public List<YandexTreeNode> Tree { get; set; }
}

public class YandexTreeNode
{
    [JsonPropertyName("dimension_fields")] public List<DimensionField> DimensionFields { get; set; }
    [JsonPropertyName("fields")] public List<Field> Fields { get; set; }
    [JsonPropertyName("entity_fields")] public List<EntityField> EntityFields { get; set; }

    public Dictionary<string, string> Period = new Dictionary<string, string>()
    {
        { "today", "Сегодня" },
        { "yesterday", "Вчера" },
        { "thismonth", "Этот месяц" },
        { "lastmonth", "Прошлый месяц" },
        { "30days", "30 дней" },
        { "90days", "90 дней" },
        { "180days", "180 дней" },
        { "365days", "365 дней" },
        { "thisyear", "Текущий год" },
    };

    public Dictionary<string, string> Accounts => YandexMetrikaController.Accounts;
}

public class DimensionField
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public List<string[]> Values { get; set; }
}

public class Field
{
    public int Category { get; set; }
    [JsonPropertyName("category_name")] public string CategoryName { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public int Index { get; set; }
    public string Hint { get; set; }
    public string Unit { get; set; }
}

public class BuildRequestDto
{
    public Dictionary<string, string> Dimensions { get; set; } = new();
    public List<string> Fields { get; set; } = new();
    public string Period { get; set; }
    public List<string> EntityFields { get; set; } = new();
    public List<string> Accounts { get; set; } = new();
}

public class YandexReportResponse
{
    [JsonPropertyName("data")] public YandexReportData Data { get; set; }
    public string AccountName;
}

public class YandexReportData
{
    [JsonPropertyName("points")] public List<YandexPoint> Points { get; set; }

    [JsonPropertyName("measures")] public Dictionary<string, YandexMeasureMeta> MeasuresMeta { get; set; }
    [JsonPropertyName("dimensions")] public Dictionary<string, YandexDimensionMeta> DimensionsMeta { get; set; }

    [JsonPropertyName("totals")] public Dictionary<string, List<Dictionary<string, JsonElement>>> Totals { get; set; }
}

public class YandexDimensionMeta
{
    [JsonPropertyName("index")] public int Index { get; set; }
    [JsonPropertyName("title")] public string Title { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
}

public class YandexMeasureMeta
{
    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("unit")] public string Unit { get; set; }
}

public class YandexPoint
{
    [JsonPropertyName("dimensions")] public Dictionary<string, JsonElement> Dimensions { get; set; }

    [JsonPropertyName("measures")] public List<Dictionary<string, JsonElement>> Measures { get; set; }
}

public class EntityField
{
    [JsonPropertyName("category")] public int Category { get; set; }
    [JsonPropertyName("category_name")] public string CategoryName { get; set; }
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("label")] public string Label { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("index")] public int Index { get; set; }
}