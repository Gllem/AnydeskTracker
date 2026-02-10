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

    public TableResult BuildTable(List<YandexReportResponse> reports)
    {
        var result = new TableResult();
        const string accountKey = "account";

        var firstReport = reports.First(r => r.Data.Points.Count > 0);
        var measuresMeta = firstReport.Data.MeasuresMeta;
        var dimensionsMeta = firstReport.Data.DimensionsMeta;

        foreach (var report in reports)
        {
            if (report.Data.Points.Count != 0 == false) continue;


            if (result.ColumnKeys.Contains(accountKey) == false)
            {
                result.ColumnKeys.Add(accountKey);
                result.ColumnTitles.Add("Аккаунт");
            }


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
        }

        var dimensionKeys = result.ColumnKeys.Where(k => !measuresMeta.ContainsKey(k));
        var measureKeys = result.ColumnKeys
            .Where(k => measuresMeta.ContainsKey(k))
            .OrderBy(k => measuresMeta[k].Index);

        result.ColumnKeys = dimensionKeys.Concat(measureKeys).ToList();

        foreach (var key in result.ColumnKeys)
        {
            if (key.Contains(accountKey)) continue;
            if (measuresMeta.TryGetValue(key, out var m)) result.ColumnTitles.Add(m.Title);
            else if (dimensionsMeta.TryGetValue(key, out var d)) result.ColumnTitles.Add(d.Title);
            else result.ColumnTitles.Add(key);
        }

        var totalRow = new Dictionary<string, string>();
        foreach (var key in result.ColumnKeys) totalRow[key] = "";
        totalRow[accountKey] = "ИТОГО";

        foreach (var report in reports)
        {
            if (report.Data.Totals.Count != 0)
            {
                var totalsEntry = report.Data.Totals.First().Value.FirstOrDefault();
                if (totalsEntry != null)
                {
                    foreach (var m in totalsEntry)
                    {
                        if (string.IsNullOrEmpty(totalRow[m.Key]))
                            totalRow[m.Key] = m.Value.ToString();
                        else if (float.TryParse(totalRow[m.Key], out var value) &&
                                 m.Value.TryGetSingle(out float additionalValue))
                            totalRow[m.Key] = (value + additionalValue).ToString();
                    }
                }
            }
        }
        result.Rows.Add(totalRow);
        
        foreach (var report in reports)
        {
            foreach (var point in report.Data.Points)
            {
                var row = new Dictionary<string, string>();
                foreach (var key in result.ColumnKeys) row[key] = "";
                row[accountKey] = report.AccountName;
                foreach (var dim in point.Dimensions) row[dim.Key] = dim.Value.ToString();
                var measures = point.Measures.FirstOrDefault();
                if (measures != null)
                {
                    foreach (var m in measures) row[m.Key] = m.Value.ToString();
                }

                result.Rows.Add(row);
            }
        }

        return result;
    }
}