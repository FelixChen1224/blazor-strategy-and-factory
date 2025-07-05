using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using blazor_strategy_and_factory.Models;
using System.Text.Json;

namespace blazor_strategy_and_factory.Services.OutputComponents;

public interface IOutputComponentStrategy
{
    OutputComponentType ComponentType { get; }
    string ComponentName { get; }
    RenderFragment<FinancialDataResponse> RenderComponent { get; }
    bool CanHandle(string componentName);
}

public class SQLQueryComponent : IOutputComponentStrategy
{
    public OutputComponentType ComponentType => OutputComponentType.Summary;
    public string ComponentName => "SQL查詢語句";

    public RenderFragment<FinancialDataResponse> RenderComponent => (data) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "sql-query-container");
        
        builder.OpenElement(2, "h4");
        builder.AddContent(3, "� SQL查詢語句");
        builder.CloseElement();
        
        if (data.Data.ContainsKey("SqlQuery"))
        {
            builder.OpenElement(4, "div");
            builder.AddAttribute(5, "class", "card");
            builder.OpenElement(6, "div");
            builder.AddAttribute(7, "class", "card-body");
            
            builder.OpenElement(8, "h6");
            builder.AddContent(9, "Oracle SQL查詢：");
            builder.CloseElement();
            
            builder.OpenElement(10, "pre");
            builder.AddAttribute(11, "class", "bg-light p-3 rounded");
            builder.AddAttribute(12, "style", "white-space: pre-wrap; font-family: 'Courier New', monospace;");
            builder.AddContent(13, data.Data["SqlQuery"].ToString());
            builder.CloseElement();
            
            if (data.Data.ContainsKey("TableName"))
            {
                builder.OpenElement(14, "div");
                builder.AddAttribute(15, "class", "mt-2");
                builder.OpenElement(16, "small");
                builder.AddAttribute(17, "class", "text-muted");
                builder.AddContent(18, $"資料表: {data.Data["TableName"]}");
                builder.CloseElement();
                builder.CloseElement();
            }
            
            builder.CloseElement();
            builder.CloseElement();
        }
        
        builder.CloseElement();
    };

    public bool CanHandle(string componentName)
    {
        return componentName.Contains("SQL", StringComparison.OrdinalIgnoreCase) ||
               componentName.Contains("查詢", StringComparison.OrdinalIgnoreCase);
    }
}

public class DataTableComponent : IOutputComponentStrategy
{
    public OutputComponentType ComponentType => OutputComponentType.Table;
    public string ComponentName => "資料表格";

    public RenderFragment<FinancialDataResponse> RenderComponent => (data) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "data-table-container");
        
        builder.OpenElement(2, "h4");
        builder.AddContent(3, "📊 查詢結果");
        builder.CloseElement();
        
        // 根據不同的資料類型渲染不同的表格
        if (data.Data.ContainsKey("EmployeeData"))
        {
            RenderEmployeeTable(builder, data);
        }
        else if (data.Data.ContainsKey("FinancialRecords"))
        {
            RenderFinancialTable(builder, data);
        }
        else if (data.Data.ContainsKey("Announcements"))
        {
            RenderAnnouncementTable(builder, data);
        }
        else if (data.Data.ContainsKey("MarketData"))
        {
            RenderMarketTable(builder, data);
        }
        else if (data.Data.ContainsKey("BudgetRecords"))
        {
            RenderBudgetTable(builder, data);
        }
        else
        {
            builder.OpenElement(4, "div");
            builder.AddAttribute(5, "class", "alert alert-info");
            builder.AddContent(6, "暫無資料可顯示");
            builder.CloseElement();
        }
        
        builder.CloseElement();
    };

    private void RenderEmployeeTable(RenderTreeBuilder builder, FinancialDataResponse data)
    {
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "table-responsive");
        builder.OpenElement(6, "table");
        builder.AddAttribute(7, "class", "table table-striped table-hover");
        
        builder.OpenElement(8, "thead");
        builder.AddAttribute(9, "class", "table-dark");
        builder.OpenElement(10, "tr");
        builder.OpenElement(11, "th");
        builder.AddContent(12, "員工編號");
        builder.CloseElement();
        builder.OpenElement(13, "th");
        builder.AddContent(14, "姓名");
        builder.CloseElement();
        builder.OpenElement(15, "th");
        builder.AddContent(16, "部門");
        builder.CloseElement();
        builder.OpenElement(17, "th");
        builder.AddContent(18, "區域");
        builder.CloseElement();
        builder.OpenElement(19, "th");
        builder.AddContent(20, "職位");
        builder.CloseElement();
        builder.OpenElement(21, "th");
        builder.AddContent(22, "入職日期");
        builder.CloseElement();
        builder.OpenElement(23, "th");
        builder.AddContent(24, "薪資");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(25, "tbody");
        
        var employees = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data.Data["EmployeeData"]));
        if (employees.ValueKind == JsonValueKind.Array)
        {
            var index = 26;
            foreach (var emp in employees.EnumerateArray())
            {
                builder.OpenElement(index++, "tr");
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("EmployeeId").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("EmployeeName").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("Department").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("Region").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("Position").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("HireDate").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, emp.GetProperty("Salary").GetString());
                builder.CloseElement();
                builder.CloseElement();
            }
        }
        
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    private void RenderFinancialTable(RenderTreeBuilder builder, FinancialDataResponse data)
    {
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "table-responsive");
        builder.OpenElement(6, "table");
        builder.AddAttribute(7, "class", "table table-striped table-hover");
        
        builder.OpenElement(8, "thead");
        builder.AddAttribute(9, "class", "table-dark");
        builder.OpenElement(10, "tr");
        builder.OpenElement(11, "th");
        builder.AddContent(12, "記錄編號");
        builder.CloseElement();
        builder.OpenElement(13, "th");
        builder.AddContent(14, "員工編號");
        builder.CloseElement();
        builder.OpenElement(15, "th");
        builder.AddContent(16, "日期");
        builder.CloseElement();
        builder.OpenElement(17, "th");
        builder.AddContent(18, "金額");
        builder.CloseElement();
        builder.OpenElement(19, "th");
        builder.AddContent(20, "交易類型");
        builder.CloseElement();
        builder.OpenElement(21, "th");
        builder.AddContent(22, "描述");
        builder.CloseElement();
        builder.OpenElement(23, "th");
        builder.AddContent(24, "狀態");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(25, "tbody");
        
        var records = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data.Data["FinancialRecords"]));
        if (records.ValueKind == JsonValueKind.Array)
        {
            var index = 26;
            foreach (var record in records.EnumerateArray())
            {
                builder.OpenElement(index++, "tr");
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("RecordId").GetInt32().ToString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("EmployeeId").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("RecordDate").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("Amount").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("TransactionType").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("Description").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, record.GetProperty("Status").GetString());
                builder.CloseElement();
                builder.CloseElement();
            }
        }
        
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    private void RenderAnnouncementTable(RenderTreeBuilder builder, FinancialDataResponse data)
    {
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "table-responsive");
        builder.OpenElement(6, "table");
        builder.AddAttribute(7, "class", "table table-striped table-hover");
        
        builder.OpenElement(8, "thead");
        builder.AddAttribute(9, "class", "table-dark");
        builder.OpenElement(10, "tr");
        builder.OpenElement(11, "th");
        builder.AddContent(12, "公告編號");
        builder.CloseElement();
        builder.OpenElement(13, "th");
        builder.AddContent(14, "標題");
        builder.CloseElement();
        builder.OpenElement(15, "th");
        builder.AddContent(16, "日期");
        builder.CloseElement();
        builder.OpenElement(17, "th");
        builder.AddContent(18, "類型");
        builder.CloseElement();
        builder.OpenElement(19, "th");
        builder.AddContent(20, "區域");
        builder.CloseElement();
        builder.OpenElement(21, "th");
        builder.AddContent(22, "重要性");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(23, "tbody");
        
        var announcements = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data.Data["Announcements"]));
        if (announcements.ValueKind == JsonValueKind.Array)
        {
            var index = 24;
            foreach (var announcement in announcements.EnumerateArray())
            {
                builder.OpenElement(index++, "tr");
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, announcement.GetProperty("AnnouncementId").GetInt32().ToString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, announcement.GetProperty("Title").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                // 處理不同的日期屬性名稱
                var dateValue = announcement.TryGetProperty("PublishedDate", out var publishedDate) ? 
                    publishedDate.GetString() : 
                    announcement.TryGetProperty("AnnouncementDate", out var announcementDate) ? 
                    announcementDate.GetString() : "未知";
                builder.AddContent(index++, dateValue);
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, announcement.GetProperty("AnnouncementType").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                // 處理不同的區域屬性名稱（模擬模式可能沒有Region）
                var regionValue = announcement.TryGetProperty("Region", out var region) ? 
                    region.GetString() : "未指定";
                builder.AddContent(index++, regionValue);
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                // 處理不同的重要性屬性名稱
                var importanceValue = announcement.TryGetProperty("Priority", out var priority) ? 
                    priority.GetString() : 
                    announcement.TryGetProperty("ImportanceLevel", out var importanceLevel) ? 
                    importanceLevel.GetString() : "未知";
                builder.AddContent(index++, importanceValue);
                builder.CloseElement();
                builder.CloseElement();
            }
        }
        
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    private void RenderMarketTable(RenderTreeBuilder builder, FinancialDataResponse data)
    {
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "table-responsive");
        builder.OpenElement(6, "table");
        builder.AddAttribute(7, "class", "table table-striped table-hover");
        
        builder.OpenElement(8, "thead");
        builder.AddAttribute(9, "class", "table-dark");
        builder.OpenElement(10, "tr");
        builder.OpenElement(11, "th");
        builder.AddContent(12, "股票代碼");
        builder.CloseElement();
        builder.OpenElement(13, "th");
        builder.AddContent(14, "日期");
        builder.CloseElement();
        builder.OpenElement(15, "th");
        builder.AddContent(16, "開盤價");
        builder.CloseElement();
        builder.OpenElement(17, "th");
        builder.AddContent(18, "收盤價");
        builder.CloseElement();
        builder.OpenElement(19, "th");
        builder.AddContent(20, "漲跌幅");
        builder.CloseElement();
        builder.OpenElement(21, "th");
        builder.AddContent(22, "成交量");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(23, "tbody");
        
        var marketData = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data.Data["MarketData"]));
        if (marketData.ValueKind == JsonValueKind.Array)
        {
            var index = 24;
            foreach (var market in marketData.EnumerateArray())
            {
                builder.OpenElement(index++, "tr");
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, market.GetProperty("Symbol").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, market.GetProperty("MarketDate").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, market.GetProperty("OpenPrice").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, market.GetProperty("ClosePrice").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, market.GetProperty("ChangePercent").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, market.GetProperty("Volume").GetString());
                builder.CloseElement();
                builder.CloseElement();
            }
        }
        
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    private void RenderBudgetTable(RenderTreeBuilder builder, FinancialDataResponse data)
    {
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "table-responsive");
        builder.OpenElement(6, "table");
        builder.AddAttribute(7, "class", "table table-striped table-hover");
        
        builder.OpenElement(8, "thead");
        builder.AddAttribute(9, "class", "table-dark");
        builder.OpenElement(10, "tr");
        builder.OpenElement(11, "th");
        builder.AddContent(12, "部門");
        builder.CloseElement();
        builder.OpenElement(13, "th");
        builder.AddContent(14, "期間");
        builder.CloseElement();
        builder.OpenElement(15, "th");
        builder.AddContent(16, "預算金額");
        builder.CloseElement();
        builder.OpenElement(17, "th");
        builder.AddContent(18, "實際金額");
        builder.CloseElement();
        builder.OpenElement(19, "th");
        builder.AddContent(20, "差異");
        builder.CloseElement();
        builder.OpenElement(21, "th");
        builder.AddContent(22, "差異率");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(23, "tbody");
        
        var budgetData = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(data.Data["BudgetRecords"]));
        if (budgetData.ValueKind == JsonValueKind.Array)
        {
            var index = 24;
            foreach (var budget in budgetData.EnumerateArray())
            {
                builder.OpenElement(index++, "tr");
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, budget.GetProperty("Department").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, budget.GetProperty("BudgetPeriod").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, budget.GetProperty("PlannedAmount").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, budget.GetProperty("ActualAmount").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, budget.GetProperty("Variance").GetString());
                builder.CloseElement();
                builder.OpenElement(index++, "td");
                builder.AddContent(index++, budget.GetProperty("VariancePercent").GetString());
                builder.CloseElement();
                builder.CloseElement();
            }
        }
        
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    public bool CanHandle(string componentName)
    {
        return componentName.Contains("Table", StringComparison.OrdinalIgnoreCase) ||
               componentName.Contains("表格", StringComparison.OrdinalIgnoreCase) ||
               componentName.Contains("資料", StringComparison.OrdinalIgnoreCase);
    }
}

public class ChartComponent : IOutputComponentStrategy
{
    public OutputComponentType ComponentType => OutputComponentType.Chart;
    public string ComponentName => "圖表";

    public RenderFragment<FinancialDataResponse> RenderComponent => (data) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "chart-container");
        
        builder.OpenElement(2, "h4");
        builder.AddContent(3, "📊 資料圖表");
        builder.CloseElement();
        
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "chart-placeholder");
        builder.AddAttribute(6, "style", "height: 300px; border: 2px dashed #ccc; display: flex; align-items: center; justify-content: center; background: #f8f9fa;");
        builder.AddContent(7, "📈 圖表將在此處顯示 (需要集成Chart.js或其他圖表庫)");
        builder.CloseElement();
        
        if (data.Data.ContainsKey("RecordCount"))
        {
            builder.OpenElement(8, "div");
            builder.AddAttribute(9, "class", "mt-3");
            builder.AddContent(10, $"記錄數量: {data.Data["RecordCount"]}");
            builder.CloseElement();
        }
        
        builder.CloseElement();
    };

    public bool CanHandle(string componentName)
    {
        return componentName.Contains("Chart", StringComparison.OrdinalIgnoreCase) ||
               componentName.Contains("圖表", StringComparison.OrdinalIgnoreCase);
    }
}

public class SummaryComponent : IOutputComponentStrategy
{
    public OutputComponentType ComponentType => OutputComponentType.Summary;
    public string ComponentName => "查詢狀態";

    public RenderFragment<FinancialDataResponse> RenderComponent => (data) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "summary-container mb-3");
        
        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "row");
        
        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "col-md-4");
        builder.OpenElement(6, "div");
        builder.AddAttribute(7, "class", "card border-primary");
        builder.OpenElement(8, "div");
        builder.AddAttribute(9, "class", "card-body text-center");
        builder.OpenElement(10, "h6");
        builder.AddAttribute(11, "class", "card-title");
        builder.AddContent(12, "查詢狀態");
        builder.CloseElement();
        builder.OpenElement(13, "p");
        builder.AddAttribute(14, "class", data.IsSuccess ? "text-success fs-4" : "text-danger fs-4");
        builder.AddContent(15, data.IsSuccess ? "✅ 查詢成功" : "❌ 查詢失敗");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(16, "div");
        builder.AddAttribute(17, "class", "col-md-4");
        builder.OpenElement(18, "div");
        builder.AddAttribute(19, "class", "card border-info");
        builder.OpenElement(20, "div");
        builder.AddAttribute(21, "class", "card-body text-center");
        builder.OpenElement(22, "h6");
        builder.AddAttribute(23, "class", "card-title");
        builder.AddContent(24, "記錄數量");
        builder.CloseElement();
        builder.OpenElement(25, "p");
        builder.AddAttribute(26, "class", "text-info fs-4");
        builder.AddContent(27, data.Data.ContainsKey("RecordCount") ? data.Data["RecordCount"].ToString() : "0");
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.OpenElement(28, "div");
        builder.AddAttribute(29, "class", "col-md-4");
        builder.OpenElement(30, "div");
        builder.AddAttribute(31, "class", "card border-secondary");
        builder.OpenElement(32, "div");
        builder.AddAttribute(33, "class", "card-body text-center");
        builder.OpenElement(34, "h6");
        builder.AddAttribute(35, "class", "card-title");
        builder.AddContent(36, "查詢時間");
        builder.CloseElement();
        builder.OpenElement(37, "p");
        builder.AddAttribute(38, "class", "text-secondary");
        builder.AddContent(39, data.GeneratedAt.ToString("HH:mm:ss"));
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
        
        builder.CloseElement();
        builder.CloseElement();
    };

    public bool CanHandle(string componentName)
    {
        return componentName.Contains("Summary", StringComparison.OrdinalIgnoreCase) ||
               componentName.Contains("摘要", StringComparison.OrdinalIgnoreCase) ||
               componentName.Contains("狀態", StringComparison.OrdinalIgnoreCase);
    }
}
