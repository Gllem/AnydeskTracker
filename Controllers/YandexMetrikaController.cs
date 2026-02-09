using AnydeskTracker.DTOs;
using AnydeskTracker.Services.MetrikaServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnydeskTracker.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Metrika")]
public class YandexMetrikaController(YandexMetrikaService metrikaService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tree = await metrikaService.GetTreeAsync();
        return View("MetrikaIndex", tree);
    }

    [HttpPost("Build")]
    public async Task<IActionResult> Build([FromBody] BuildRequestDto request)
    {
        var result = await metrikaService.BuildReportAsync(
            request);

        var table = BuildTable(result);

        return Ok(table);
    }

    public TableResult BuildTable(YandexReportResponse report)
    {
        var result = new TableResult();

        if (report?.Data?.Points == null || !report.Data.Points.Any())
            return result;

        var measuresMeta = report.Data.MeasuresMeta ?? new Dictionary<string, YandexMeasureMeta>();
        var dimensionsMeta = report.Data.DimensionsMeta ?? new Dictionary<string, YandexDimensionMeta>();

        // 1. Собираем ключи колонок (порядок dimensions сохраняется как в API)
        foreach (var point in report.Data.Points)
        {
            foreach (var dim in point.Dimensions.Keys)
                if (!result.ColumnKeys.Contains(dim))
                    result.ColumnKeys.Add(dim);

            var measures = point.Measures.FirstOrDefault();
            if (measures != null)
            {
                foreach (var m in measures.Keys)
                    if (!result.ColumnKeys.Contains(m))
                        result.ColumnKeys.Add(m);
            }
        }

     
        var dimensionKeys = result.ColumnKeys
            .Where(k => !measuresMeta.ContainsKey(k));

        var measureKeys = result.ColumnKeys
            .Where(k => measuresMeta.ContainsKey(k))
            .OrderBy(k => measuresMeta[k].Index);

        result.ColumnKeys = dimensionKeys
            .Concat(measureKeys)
            .ToList();

        // 3. Заголовки
        foreach (var key in result.ColumnKeys)
        {
            if (measuresMeta.TryGetValue(key, out var m))
                result.ColumnTitles.Add(m.Title);
            else if (dimensionsMeta.TryGetValue(key, out var d))
                result.ColumnTitles.Add(d.Title);
            else
                result.ColumnTitles.Add(key);
        }

        // 4. Totals первой строкой
        if (report.Data.Totals?.Any() == true)
        {
            var totalsEntry = report.Data.Totals.First().Value.FirstOrDefault();
            if (totalsEntry != null)
            {
                var totalRow = new Dictionary<string, string>();

                foreach (var key in result.ColumnKeys)
                    totalRow[key] = "";

                foreach (var m in totalsEntry)
                    totalRow[m.Key] = m.Value.ToString();

                if (result.ColumnKeys.Any())
                    totalRow[result.ColumnKeys.First()] = "ИТОГО";

                result.Rows.Add(totalRow);
            }
        }

        // 5. Обычные строки
        foreach (var point in report.Data.Points)
        {
            var row = new Dictionary<string, string>();

            foreach (var key in result.ColumnKeys)
                row[key] = "";

            foreach (var dim in point.Dimensions)
                row[dim.Key] = dim.Value.ToString();

            var measures = point.Measures.FirstOrDefault();
            if (measures != null)
            {
                foreach (var m in measures)
                    row[m.Key] = m.Value.ToString();
            }

            result.Rows.Add(row);
        }

        return result;
    }
}