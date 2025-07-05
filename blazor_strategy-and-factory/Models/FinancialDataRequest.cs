namespace blazor_strategy_and_factory.Models;

public class FinancialDataRequest
{
    public string? EmployeeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Region { get; set; }
    public string? Department { get; set; }
    public string? TransactionType { get; set; }
    public string? AnnouncementType { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
    public string? Prompt { get; set; }
    public List<string> DataSources { get; set; } = new();
    public List<string> OutputComponents { get; set; } = new();
}

public class FinancialDataResponse
{
    public string ReportId { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public List<string> Messages { get; set; } = new();
    public bool IsSuccess { get; set; }
}

public enum DataSourceType
{
    FTP,
    Database,
    WebAPI,
    TextFile,
    PDF
}

public enum OutputComponentType
{
    PDFViewer,
    Chart,
    Table,
    Summary,
    Dashboard
}
