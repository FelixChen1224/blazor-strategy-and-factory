using blazor_strategy_and_factory.Models;
using blazor_strategy_and_factory.Data;
using blazor_strategy_and_factory.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace blazor_strategy_and_factory.Services.DataSources;

public interface IDataSourceStrategy
{
    DataSourceType SourceType { get; }
    string TableName { get; }
    Task<FinancialDataResponse> FetchDataAsync(FinancialDataRequest request);
    bool CanHandle(string dataSourceName);
    Task<List<string>> GetAvailableValuesAsync(string fieldName);
}

public class EmployeeDataSource : IDataSourceStrategy
{
    private readonly FinancialDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly OracleSimulationService _oracleSimulationService;
    
    public EmployeeDataSource(FinancialDbContext context, IConfiguration configuration, OracleSimulationService oracleSimulationService)
    {
        _context = context;
        _configuration = configuration;
        _oracleSimulationService = oracleSimulationService;
    }

    public DataSourceType SourceType => DataSourceType.Database;
    public string TableName => "EMPLOYEES";

    public async Task<FinancialDataResponse> FetchDataAsync(FinancialDataRequest request)
    {
        try
        {
            // 在模擬模式下，使用模擬資料
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                Console.WriteLine("✅ EmployeeDataSource: 使用模擬模式");
                return await GetSimulatedEmployeeDataAsync(request);
            }

            Console.WriteLine("📋 EmployeeDataSource: 使用實際資料庫模式");
            // 使用Oracle SQL語法查詢員工資料
            var query = _context.Employees.AsQueryable();

            // 根據請求條件過濾
            if (!string.IsNullOrEmpty(request.EmployeeId))
            {
                query = query.Where(e => e.EmployeeId == request.EmployeeId);
            }

            if (!string.IsNullOrEmpty(request.Region))
            {
                query = query.Where(e => e.Region == request.Region);
            }

            if (!string.IsNullOrEmpty(request.Department))
            {
                query = query.Where(e => e.Department == request.Department);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(e => e.HireDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(e => e.HireDate <= request.EndDate.Value);
            }

            var employees = await query.Take(100).ToListAsync();

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["EmployeeData"] = employees.Select(e => new
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.EmployeeName,
                        Department = e.Department,
                        Region = e.Region,
                        Position = e.Position,
                        HireDate = e.HireDate.ToString("yyyy-MM-dd"),
                        Salary = e.Salary.ToString("C")
                    }),
                    ["RecordCount"] = employees.Count,
                    ["SqlQuery"] = $"SELECT * FROM {TableName} WHERE 1=1" +
                                  (!string.IsNullOrEmpty(request.EmployeeId) ? $" AND EMPLOYEE_ID = '{request.EmployeeId}'" : "") +
                                  (!string.IsNullOrEmpty(request.Region) ? $" AND REGION = '{request.Region}'" : "") +
                                  (request.StartDate.HasValue ? $" AND HIRE_DATE >= TO_DATE('{request.StartDate:yyyy-MM-dd}', 'YYYY-MM-DD')" : "") +
                                  (request.EndDate.HasValue ? $" AND HIRE_DATE <= TO_DATE('{request.EndDate:yyyy-MM-dd}', 'YYYY-MM-DD')" : "")
                },
                Messages = new List<string> { $"已成功從{TableName}資料表查詢到 {employees.Count} 筆員工資料" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"查詢{TableName}資料表時發生錯誤: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    private async Task<FinancialDataResponse> GetSimulatedEmployeeDataAsync(FinancialDataRequest request)
    {
        try
        {
            Console.WriteLine("🔧 開始執行模擬員工資料查詢");
            // 從 OracleSimulationService 取得模擬員工資料
            var simulatedEmployees = await _oracleSimulationService.GetSimulatedEmployeesAsync();
            Console.WriteLine($"🎯 從模擬服務取得 {simulatedEmployees.Count} 筆員工資料");
            
            // 根據請求條件過濾模擬資料
            var filteredEmployees = simulatedEmployees.AsQueryable();

            if (!string.IsNullOrEmpty(request.EmployeeId))
            {
                filteredEmployees = filteredEmployees.Where(e => e.EmployeeId == request.EmployeeId);
            }

            if (!string.IsNullOrEmpty(request.Region))
            {
                filteredEmployees = filteredEmployees.Where(e => e.Region == request.Region);
            }

            if (!string.IsNullOrEmpty(request.Department))
            {
                filteredEmployees = filteredEmployees.Where(e => e.Department == request.Department);
            }

            if (request.StartDate.HasValue)
            {
                filteredEmployees = filteredEmployees.Where(e => e.HireDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                filteredEmployees = filteredEmployees.Where(e => e.HireDate <= request.EndDate.Value);
            }

            var employees = filteredEmployees.ToList();
            Console.WriteLine($"✨ 過濾後剩餘 {employees.Count} 筆員工資料");

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["EmployeeData"] = employees.Select(e => new
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.EmployeeName,
                        Department = e.Department,
                        Region = e.Region,
                        Position = e.Position,
                        HireDate = e.HireDate.ToString("yyyy-MM-dd"),
                        Salary = e.Salary.ToString("C")
                    }),
                    ["RecordCount"] = employees.Count,
                    ["SqlQuery"] = $"模擬查詢 - 從 {TableName} 取得 {employees.Count} 筆資料"
                },
                Messages = new List<string> { $"✅ 模擬模式: 已從模擬資料取得 {employees.Count} 筆員工資料" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"❌ 模擬模式查詢失敗: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    public bool CanHandle(string dataSourceName)
    {
        return dataSourceName.Contains("員工", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("Employee", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetAvailableValuesAsync(string fieldName)
    {
        try
        {
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                // 模擬模式下使用模擬資料
                var simulatedEmployees = await _oracleSimulationService.GetSimulatedEmployeesAsync();
                
                return fieldName.ToLower() switch
                {
                    "department" => simulatedEmployees.Where(e => !string.IsNullOrEmpty(e.Department)).Select(e => e.Department!).Distinct().ToList(),
                    "region" => simulatedEmployees.Where(e => !string.IsNullOrEmpty(e.Region)).Select(e => e.Region!).Distinct().ToList(),
                    "position" => simulatedEmployees.Where(e => !string.IsNullOrEmpty(e.Position)).Select(e => e.Position!).Distinct().ToList(),
                    _ => new List<string>()
                };
            }
            else
            {
                // 實際資料庫模式
                return fieldName.ToLower() switch
                {
                    "department" => await _context.Employees.Where(e => !string.IsNullOrEmpty(e.Department)).Select(e => e.Department).Distinct().ToListAsync(),
                    "region" => await _context.Employees.Where(e => !string.IsNullOrEmpty(e.Region)).Select(e => e.Region).Distinct().ToListAsync(),
                    "position" => await _context.Employees.Where(e => !string.IsNullOrEmpty(e.Position)).Select(e => e.Position).Distinct().ToListAsync(),
                    _ => new List<string>()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GetAvailableValuesAsync 錯誤: {ex.Message}");
            return new List<string>();
        }
    }
}

public class FinancialRecordDataSource : IDataSourceStrategy
{
    private readonly FinancialDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly OracleSimulationService _oracleSimulationService;
    
    public FinancialRecordDataSource(FinancialDbContext context, IConfiguration configuration, OracleSimulationService oracleSimulationService)
    {
        _context = context;
        _configuration = configuration;
        _oracleSimulationService = oracleSimulationService;
    }

    public DataSourceType SourceType => DataSourceType.Database;
    public string TableName => "FINANCIAL_RECORDS";

    public async Task<FinancialDataResponse> FetchDataAsync(FinancialDataRequest request)
    {
        try
        {
            // 在模擬模式下，使用模擬資料
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                Console.WriteLine("✅ FinancialRecordDataSource: 使用模擬模式");
                return await GetSimulatedFinancialRecordDataAsync(request);
            }

            Console.WriteLine("📋 FinancialRecordDataSource: 使用實際資料庫模式");
            var query = _context.FinancialRecords.AsQueryable();

            if (!string.IsNullOrEmpty(request.EmployeeId))
            {
                query = query.Where(f => f.EmployeeId == request.EmployeeId);
            }

            var records = await query.OrderByDescending(f => f.RecordDate).Take(100).ToListAsync();

            var totalAmount = records.Sum(r => r.Amount);
            var avgAmount = records.Any() ? records.Average(r => r.Amount) : 0;

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["FinancialRecords"] = records.Select(r => new
                    {
                        RecordId = r.RecordId,
                        EmployeeId = r.EmployeeId,
                        RecordDate = r.RecordDate.ToString("yyyy-MM-dd"),
                        Amount = r.Amount.ToString("C"),
                        TransactionType = r.TransactionType,
                        Description = r.Description,
                        Region = r.Region,
                        Category = r.Category,
                        Status = r.Status
                    }),
                    ["Summary"] = new
                    {
                        TotalAmount = totalAmount.ToString("C"),
                        AverageAmount = avgAmount.ToString("C"),
                        RecordCount = records.Count,
                        DateRange = $"{request.StartDate?.ToString("yyyy-MM-dd")} ~ {request.EndDate?.ToString("yyyy-MM-dd")}"
                    },
                    ["SqlQuery"] = $"SELECT * FROM {TableName} WHERE 1=1" +
                                  (!string.IsNullOrEmpty(request.EmployeeId) ? $" AND EMPLOYEE_ID = '{request.EmployeeId}'" : "") +
                                  " ORDER BY RECORD_DATE DESC"
                },
                Messages = new List<string> { $"已成功從{TableName}資料表查詢到 {records.Count} 筆財務記錄" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"查詢{TableName}資料表時發生錯誤: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    private async Task<FinancialDataResponse> GetSimulatedFinancialRecordDataAsync(FinancialDataRequest request)
    {
        try
        {
            Console.WriteLine("🔧 開始執行模擬財務記錄查詢");
            // 從 OracleSimulationService 取得模擬財務記錄
            var simulatedRecords = await _oracleSimulationService.GetSimulatedFinancialRecordsAsync();
            Console.WriteLine($"🎯 從模擬服務取得 {simulatedRecords.Count} 筆財務記錄");
            
            // 根據請求條件過濾模擬資料
            var filteredRecords = simulatedRecords.AsQueryable();

            if (!string.IsNullOrEmpty(request.EmployeeId))
            {
                filteredRecords = filteredRecords.Where(f => f.EmployeeId == request.EmployeeId);
            }

            var records = filteredRecords.OrderByDescending(f => f.RecordDate).ToList();
            Console.WriteLine($"✨ 過濾後剩餘 {records.Count} 筆財務記錄");

            var totalAmount = records.Sum(r => r.Amount);
            var avgAmount = records.Any() ? records.Average(r => r.Amount) : 0;

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["FinancialRecords"] = records.Select(r => new
                    {
                        RecordId = r.RecordId,
                        EmployeeId = r.EmployeeId,
                        RecordDate = r.RecordDate.ToString("yyyy-MM-dd"),
                        Amount = r.Amount.ToString("C"),
                        TransactionType = r.TransactionType,
                        Description = r.Description,
                        Region = r.Region,
                        Category = r.Category,
                        Status = r.Status
                    }),
                    ["Summary"] = new
                    {
                        TotalAmount = totalAmount.ToString("C"),
                        AverageAmount = avgAmount.ToString("C"),
                        RecordCount = records.Count,
                        DateRange = $"{request.StartDate?.ToString("yyyy-MM-dd")} ~ {request.EndDate?.ToString("yyyy-MM-dd")}"
                    },
                    ["RecordCount"] = records.Count,
                    ["SqlQuery"] = $"Oracle SQL查詢：\n" +
                                  $"SELECT * FROM {TableName} WHERE 1=1" +
                                  (!string.IsNullOrEmpty(request.EmployeeId) ? $" AND EMPLOYEE_ID = '{request.EmployeeId}'" : "") +
                                  " ORDER BY RECORD_DATE DESC\n" +
                                  $"模擬查詢 - 從 {TableName} 取得 {records.Count} 筆資料"
                },
                Messages = new List<string> { $"✅ 模擬模式: 已從模擬資料取得 {records.Count} 筆財務記錄" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"❌ 模擬模式查詢失敗: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    public bool CanHandle(string dataSourceName)
    {
        return dataSourceName.Contains("財務記錄", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("Financial", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetAvailableValuesAsync(string fieldName)
    {
        try
        {
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                // 模擬模式下使用模擬資料
                var simulatedRecords = await _oracleSimulationService.GetSimulatedFinancialRecordsAsync();
                
                return fieldName.ToLower() switch
                {
                    "transactiontype" => simulatedRecords.Where(r => !string.IsNullOrEmpty(r.TransactionType)).Select(r => r.TransactionType!).Distinct().ToList(),
                    "region" => simulatedRecords.Where(r => !string.IsNullOrEmpty(r.Region)).Select(r => r.Region!).Distinct().ToList(),
                    "category" => simulatedRecords.Where(r => !string.IsNullOrEmpty(r.Category)).Select(r => r.Category!).Distinct().ToList(),
                    "status" => simulatedRecords.Where(r => !string.IsNullOrEmpty(r.Status)).Select(r => r.Status!).Distinct().ToList(),
                    _ => new List<string>()
                };
            }
            else
            {
                // 實際資料庫模式
                return fieldName.ToLower() switch
                {
                    "transactiontype" => await _context.FinancialRecords.Where(r => !string.IsNullOrEmpty(r.TransactionType)).Select(r => r.TransactionType).Distinct().ToListAsync(),
                    "region" => await _context.FinancialRecords.Where(r => !string.IsNullOrEmpty(r.Region)).Select(r => r.Region).Distinct().ToListAsync(),
                    "category" => await _context.FinancialRecords.Where(r => !string.IsNullOrEmpty(r.Category)).Select(r => r.Category).Distinct().ToListAsync(),
                    "status" => await _context.FinancialRecords.Where(r => !string.IsNullOrEmpty(r.Status)).Select(r => r.Status).Distinct().ToListAsync(),
                    _ => new List<string>()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GetAvailableValuesAsync 錯誤: {ex.Message}");
            return new List<string>();
        }
    }
}

public class CompanyAnnouncementDataSource : IDataSourceStrategy
{
    private readonly FinancialDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly OracleSimulationService _oracleSimulationService;
    
    public CompanyAnnouncementDataSource(FinancialDbContext context, IConfiguration configuration, OracleSimulationService oracleSimulationService)
    {
        _context = context;
        _configuration = configuration;
        _oracleSimulationService = oracleSimulationService;
    }

    public DataSourceType SourceType => DataSourceType.Database;
    public string TableName => "COMPANY_ANNOUNCEMENTS";

    public async Task<FinancialDataResponse> FetchDataAsync(FinancialDataRequest request)
    {
        try
        {
            // 在模擬模式下，使用 OracleSimulationService 的模擬資料
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                var simulatedAnnouncements = await _oracleSimulationService.GetSimulatedCompanyAnnouncementsAsync();
                
                // 根據請求條件篩選資料
                var filteredAnnouncements = simulatedAnnouncements.AsQueryable();
                
                // 根據員工ID篩選相關的公司重訊
                if (!string.IsNullOrEmpty(request.EmployeeId))
                {
                    filteredAnnouncements = filteredAnnouncements.Where(a => a.EmployeeId == request.EmployeeId);
                }

                var announcements = filteredAnnouncements.OrderByDescending(a => a.PublishedDate).Take(50).ToList();

                return new FinancialDataResponse
                {
                    ReportId = Guid.NewGuid().ToString(),
                    GeneratedAt = DateTime.Now,
                    Data = new Dictionary<string, object>
                    {
                        ["TableName"] = TableName,
                        ["Announcements"] = announcements.Select(a => new
                        {
                            AnnouncementId = a.AnnouncementId,
                            Title = a.Title,
                            Content = a.Content?.Length > 100 ? a.Content.Substring(0, 100) + "..." : a.Content,
                            PublishedDate = a.PublishedDate.ToString("yyyy-MM-dd"),
                            AnnouncementType = a.AnnouncementType,
                            Publisher = a.Publisher,
                            Priority = a.Priority,
                            Status = a.Status
                        }),
                        ["RecordCount"] = announcements.Count,
                        ["SqlQuery"] = $"SELECT * FROM {TableName} WHERE 1=1" +
                                      (!string.IsNullOrEmpty(request.EmployeeId) ? $" AND EMPLOYEE_ID = '{request.EmployeeId}'" : "") +
                                      " ORDER BY PUBLISHED_DATE DESC"
                    },
                    Messages = new List<string> { $"模擬模式: 已成功從{TableName}資料表查詢到 {announcements.Count} 筆公司重訊" },
                    IsSuccess = true
                };
            }
            
            // 實際模式下的資料庫查詢
            var query = _context.CompanyAnnouncements.AsQueryable();

            if (request.StartDate.HasValue)
            {
                query = query.Where(c => c.AnnouncementDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(c => c.AnnouncementDate <= request.EndDate.Value);
            }

            var dbAnnouncements = await query.OrderByDescending(c => c.AnnouncementDate).Take(50).ToListAsync();

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["Announcements"] = dbAnnouncements.Select(a => new
                    {
                        AnnouncementId = a.AnnouncementId,
                        Title = a.Title,
                        Content = a.Content.Length > 100 ? a.Content.Substring(0, 100) + "..." : a.Content,
                        AnnouncementDate = a.AnnouncementDate.ToString("yyyy-MM-dd"),
                        AnnouncementType = a.AnnouncementType,
                        Region = a.Region,
                        ImportanceLevel = a.ImportanceLevel,
                        Status = a.Status
                    }),
                    ["RecordCount"] = dbAnnouncements.Count,
                    ["SqlQuery"] = $"SELECT * FROM {TableName} WHERE 1=1" +
                                  (request.StartDate.HasValue ? $" AND ANNOUNCEMENT_DATE >= TO_DATE('{request.StartDate:yyyy-MM-dd}', 'YYYY-MM-DD')" : "") +
                                  (request.EndDate.HasValue ? $" AND ANNOUNCEMENT_DATE <= TO_DATE('{request.EndDate:yyyy-MM-dd}', 'YYYY-MM-DD')" : "") +
                                  " ORDER BY ANNOUNCEMENT_DATE DESC"
                },
                Messages = new List<string> { $"已成功從{TableName}資料表查詢到 {dbAnnouncements.Count} 筆公司重訊" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"查詢{TableName}資料表時發生錯誤: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    public bool CanHandle(string dataSourceName)
    {
        return dataSourceName.Contains("公司重訊", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("重訊", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("公司公告", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("公告", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("Announcement", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetAvailableValuesAsync(string fieldName)
    {
        try
        {
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                // 模擬模式下使用模擬資料
                var simulatedAnnouncements = await _oracleSimulationService.GetSimulatedCompanyAnnouncementsAsync();
                
                return fieldName.ToLower() switch
                {
                    "announcementtype" => simulatedAnnouncements.Where(a => !string.IsNullOrEmpty(a.AnnouncementType)).Select(a => a.AnnouncementType!).Distinct().ToList(),
                    "priority" => simulatedAnnouncements.Where(a => !string.IsNullOrEmpty(a.Priority)).Select(a => a.Priority!).Distinct().ToList(),
                    "status" => simulatedAnnouncements.Where(a => !string.IsNullOrEmpty(a.Status)).Select(a => a.Status!).Distinct().ToList(),
                    _ => new List<string>()
                };
            }
            else
            {
                // 實際資料庫模式
                return fieldName.ToLower() switch
                {
                    "announcementtype" => await _context.CompanyAnnouncements.Where(a => !string.IsNullOrEmpty(a.AnnouncementType)).Select(a => a.AnnouncementType).Distinct().ToListAsync(),
                    "region" => await _context.CompanyAnnouncements.Where(a => !string.IsNullOrEmpty(a.Region)).Select(a => a.Region).Distinct().ToListAsync(),
                    "priority" => await _context.CompanyAnnouncements.Where(a => !string.IsNullOrEmpty(a.ImportanceLevel)).Select(a => a.ImportanceLevel).Distinct().ToListAsync(),
                    "status" => await _context.CompanyAnnouncements.Where(a => !string.IsNullOrEmpty(a.Status)).Select(a => a.Status).Distinct().ToListAsync(),
                    _ => new List<string>()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GetAvailableValuesAsync 錯誤: {ex.Message}");
            return new List<string>();
        }
    }
}

public class MarketDataSource : IDataSourceStrategy
{
    private readonly FinancialDbContext _context;
    
    public MarketDataSource(FinancialDbContext context)
    {
        _context = context;
    }

    public DataSourceType SourceType => DataSourceType.Database;
    public string TableName => "MARKET_DATA";

    public async Task<FinancialDataResponse> FetchDataAsync(FinancialDataRequest request)
    {
        try
        {
            var query = _context.MarketData.AsQueryable();

            if (!string.IsNullOrEmpty(request.Region))
            {
                query = query.Where(m => m.Region == request.Region);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(m => m.MarketDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(m => m.MarketDate <= request.EndDate.Value);
            }

            var marketData = await query.OrderByDescending(m => m.MarketDate).Take(100).ToListAsync();

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["MarketData"] = marketData.Select(m => new
                    {
                        Symbol = m.Symbol,
                        MarketDate = m.MarketDate.ToString("yyyy-MM-dd"),
                        OpenPrice = m.OpenPrice.ToString("C"),
                        ClosePrice = m.ClosePrice.ToString("C"),
                        HighPrice = m.HighPrice.ToString("C"),
                        LowPrice = m.LowPrice.ToString("C"),
                        Volume = m.Volume.ToString("N0"),
                        ChangePercent = m.ChangePercent.ToString("P2"),
                        Region = m.Region,
                        MarketType = m.MarketType
                    }),
                    ["SqlQuery"] = $"SELECT * FROM {TableName} WHERE 1=1" +
                                  (!string.IsNullOrEmpty(request.Region) ? $" AND REGION = '{request.Region}'" : "") +
                                  (request.StartDate.HasValue ? $" AND MARKET_DATE >= TO_DATE('{request.StartDate:yyyy-MM-dd}', 'YYYY-MM-DD')" : "") +
                                  (request.EndDate.HasValue ? $" AND MARKET_DATE <= TO_DATE('{request.EndDate:yyyy-MM-dd}', 'YYYY-MM-DD')" : "") +
                                  " ORDER BY MARKET_DATE DESC"
                },
                Messages = new List<string> { $"已成功從{TableName}資料表查詢到 {marketData.Count} 筆市場資料" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"查詢{TableName}資料表時發生錯誤: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    public bool CanHandle(string dataSourceName)
    {
        return dataSourceName.Contains("市場資料", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("Market", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetAvailableValuesAsync(string fieldName)
    {
        await Task.CompletedTask;
        return new List<string>();
    }
}

public class BudgetDataSource : IDataSourceStrategy
{
    private readonly FinancialDbContext _context;
    
    public BudgetDataSource(FinancialDbContext context)
    {
        _context = context;
    }

    public DataSourceType SourceType => DataSourceType.Database;
    public string TableName => "BUDGET_RECORDS";

    public async Task<FinancialDataResponse> FetchDataAsync(FinancialDataRequest request)
    {
        try
        {
            var query = _context.BudgetRecords.AsQueryable();

            if (!string.IsNullOrEmpty(request.Region))
            {
                query = query.Where(b => b.Region == request.Region);
            }

            if (request.StartDate.HasValue)
            {
                var startYear = request.StartDate.Value.Year;
                var startMonth = request.StartDate.Value.Month;
                query = query.Where(b => b.BudgetYear > startYear || (b.BudgetYear == startYear && b.BudgetMonth >= startMonth));
            }

            if (request.EndDate.HasValue)
            {
                var endYear = request.EndDate.Value.Year;
                var endMonth = request.EndDate.Value.Month;
                query = query.Where(b => b.BudgetYear < endYear || (b.BudgetYear == endYear && b.BudgetMonth <= endMonth));
            }

            var budgetData = await query.OrderByDescending(b => b.BudgetYear).ThenByDescending(b => b.BudgetMonth).Take(100).ToListAsync();

            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>
                {
                    ["TableName"] = TableName,
                    ["BudgetRecords"] = budgetData.Select(b => new
                    {
                        Department = b.Department,
                        BudgetPeriod = $"{b.BudgetYear}-{b.BudgetMonth:D2}",
                        PlannedAmount = b.PlannedAmount.ToString("C"),
                        ActualAmount = b.ActualAmount.ToString("C"),
                        Variance = b.Variance.ToString("C"),
                        VariancePercent = b.PlannedAmount != 0 ? (b.Variance / b.PlannedAmount * 100).ToString("F2") + "%" : "N/A",
                        Region = b.Region,
                        Category = b.Category,
                        Status = b.Status
                    }),
                    ["SqlQuery"] = $"SELECT * FROM {TableName} WHERE 1=1" +
                                  (!string.IsNullOrEmpty(request.Region) ? $" AND REGION = '{request.Region}'" : "") +
                                  (request.StartDate.HasValue ? $" AND (BUDGET_YEAR > {request.StartDate.Value.Year} OR (BUDGET_YEAR = {request.StartDate.Value.Year} AND BUDGET_MONTH >= {request.StartDate.Value.Month}))" : "") +
                                  (request.EndDate.HasValue ? $" AND (BUDGET_YEAR < {request.EndDate.Value.Year} OR (BUDGET_YEAR = {request.EndDate.Value.Year} AND BUDGET_MONTH <= {request.EndDate.Value.Month}))" : "") +
                                  " ORDER BY BUDGET_YEAR DESC, BUDGET_MONTH DESC"
                },
                Messages = new List<string> { $"已成功從{TableName}資料表查詢到 {budgetData.Count} 筆預算資料" },
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new FinancialDataResponse
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Messages = new List<string> { $"查詢{TableName}資料表時發生錯誤: {ex.Message}" },
                IsSuccess = false
            };
        }
    }

    public bool CanHandle(string dataSourceName)
    {
        return dataSourceName.Contains("預算", StringComparison.OrdinalIgnoreCase) ||
               dataSourceName.Contains("Budget", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<string>> GetAvailableValuesAsync(string fieldName)
    {
        await Task.CompletedTask;
        return new List<string>();
    }
}
