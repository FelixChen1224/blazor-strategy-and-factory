using blazor_strategy_and_factory.Models;

namespace blazor_strategy_and_factory.Services;

public class FinancialAnalysisService
{
    private readonly OracleSimulationService _oracleService;
    private readonly GeminiAIService _geminiService;
    private readonly ILogger<FinancialAnalysisService> _logger;

    public FinancialAnalysisService(
        OracleSimulationService oracleService,
        GeminiAIService geminiService,
        ILogger<FinancialAnalysisService> logger)
    {
        _oracleService = oracleService;
        _geminiService = geminiService;
        _logger = logger;
    }

    // 生成員工金融報告
    public async Task<EmployeeFinancialAnalysisResult> GenerateEmployeeFinancialAnalysisAsync(
        FinancialDataRequest request)
    {
        try
        {
            _logger.LogInformation("開始生成員工 {EmployeeId} 的金融分析報告", request.EmployeeId);

            // 1. 取得員工完整資料
            var reportData = await _oracleService.GetEmployeeFinancialReportAsync(request.EmployeeId);
            
            if (reportData.Employee == null)
            {
                throw new InvalidOperationException($"找不到員工 {request.EmployeeId} 的資料");
            }

            // 2. 直接使用所有資料，不進行日期範圍過濾
            var allRecords = reportData.FinancialRecords;
            var allAnnouncements = reportData.CompanyAnnouncements;

            // 3. 計算投資統計
            var investmentStats = CalculateInvestmentStatistics(allRecords);

            // 4. 使用AI分析資料
            var aiAnalysis = await _geminiService.AnalyzeEmployeeFinancialReportAsync(
                request.EmployeeId,
                reportData.Employee,
                allRecords.Cast<object>().ToList(),
                allAnnouncements.Cast<object>().ToList()
            );

            // 5. 生成投資摘要
            var investmentSummary = await _geminiService.GenerateEmployeeInvestmentSummaryAsync(
                request.EmployeeId,
                allRecords.Count,
                investmentStats.TotalInvestment,
                investmentStats.StockCodes
            );

            return new EmployeeFinancialAnalysisResult
            {
                Employee = reportData.Employee,
                FinancialRecords = allRecords,
                CompanyAnnouncements = allAnnouncements,
                InvestmentStatistics = investmentStats,
                AIAnalysis = aiAnalysis,
                InvestmentSummary = investmentSummary,
                GeneratedAt = DateTime.UtcNow,
                RequestParameters = request
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成員工金融分析報告時發生錯誤");
            throw;
        }
    }

    // 計算投資統計
    private InvestmentStatistics CalculateInvestmentStatistics(
        List<OracleSimulationService.FinancialRecordDto> records)
    {
        var totalInvestment = records.Where(r => r.TransactionType == "買入").Sum(r => r.Amount);
        var totalDivestment = records.Where(r => r.TransactionType == "賣出").Sum(r => r.Amount);
        var stockCodes = records.Select(r => r.StockCode).Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList();

        return new InvestmentStatistics
        {
            TotalInvestment = totalInvestment,
            TotalDivestment = totalDivestment,
            NetInvestment = totalInvestment - totalDivestment,
            TransactionCount = records.Count,
            StockCodes = stockCodes!,
            AverageTransactionAmount = records.Count > 0 ? records.Average(r => r.Amount) : 0
        };
    }
}

// 投資統計資料
public class InvestmentStatistics
{
    public decimal TotalInvestment { get; set; }
    public decimal TotalDivestment { get; set; }
    public decimal NetInvestment { get; set; }
    public int TransactionCount { get; set; }
    public List<string> StockCodes { get; set; } = new();
    public decimal AverageTransactionAmount { get; set; }
}

// 員工金融分析結果
public class EmployeeFinancialAnalysisResult
{
    public OracleSimulationService.EmployeeDto? Employee { get; set; }
    public List<OracleSimulationService.FinancialRecordDto> FinancialRecords { get; set; } = new();
    public List<OracleSimulationService.CompanyAnnouncementDto> CompanyAnnouncements { get; set; } = new();
    public InvestmentStatistics? InvestmentStatistics { get; set; }
    public string AIAnalysis { get; set; } = string.Empty;
    public string InvestmentSummary { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public FinancialDataRequest? RequestParameters { get; set; }
}