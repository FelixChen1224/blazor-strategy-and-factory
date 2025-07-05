using blazor_strategy_and_factory.Data;
using Microsoft.EntityFrameworkCore;

namespace blazor_strategy_and_factory.Services;

public class OracleSimulationService
{
    private readonly FinancialDbContext _context;
    private readonly IConfiguration _configuration;

    public OracleSimulationService(FinancialDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<bool> TestOracleConnectionAsync()
    {
        try
        {
            Console.WriteLine("開始 Oracle 連線測試...");
            
            // 在模擬模式下，總是返回成功
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                Console.WriteLine("模擬模式: 模擬Oracle連線測試");
                await Task.Delay(500); // 模擬連線延遲
                Console.WriteLine("模擬模式: 連線測試完成");
                return true;
            }

            // 實際測試Oracle連線
            Console.WriteLine("實際模式: 嘗試連線到Oracle資料庫");
            
            // 使用 Task.Run 來避免潛在的線程問題
            var connectionResult = await Task.Run(async () =>
            {
                try
                {
                    return await _context.Database.CanConnectAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"連線測試內部錯誤: {ex.Message}");
                    return false;
                }
            });

            if (connectionResult)
            {
                Console.WriteLine("Oracle連線測試成功");
                return true;
            }
            else
            {
                Console.WriteLine("Oracle連線測試失敗");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Oracle連線測試失敗: {ex.Message}");
            Console.WriteLine($"錯誤類型: {ex.GetType().Name}");
            Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            
            // 如果不是模擬模式但連線失敗，嘗試自動切換到模擬模式
            if (!_configuration.GetValue<bool>("SimulationMode", false))
            {
                Console.WriteLine("連線失敗，建議啟用模擬模式進行測試");
            }
            
            return false;
        }
    }

    public async Task<(bool, string)> CreateMigrationAsync(string migrationName)
    {
        if (_configuration.GetValue<bool>("SimulationMode", false))
        {
            await Task.Delay(150); // 模擬延遲
            var output = $"✅ 模擬成功: Migration '{migrationName}' 已建立。\n" +
                         "- Timestamp: " + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            return (true, output);
        }

        // 實際模式下，我們無法直接執行 dotnet ef，所以回傳提示
        return (false, "❌ 實際模式下無法從應用程式內部建立Migration。\n" +
                       "請在專案目錄下使用終端機執行以下指令：\n" +
                      $"dotnet ef migrations add {migrationName}");
    }

    public async Task<string> GenerateCreateScriptAsync()
    {
        try
        {
            // 在模擬模式下，返回模擬的SQL腳本
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                return GenerateSimulatedCreateTableScript();
            }

            // 實際產生Oracle建表腳本
            var script = _context.Database.GenerateCreateScript();
            return await Task.FromResult(script);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"產生建表腳本失敗: {ex.Message}");
            return GenerateSimulatedCreateTableScript();
        }
    }

    public async Task<List<string>> GetPendingMigrationsAsync()
    {
        try
        {
            // 在模擬模式下，返回模擬的migration資訊
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                return new List<string> { "20250705060021_InitialCreate" };
            }

            // 實際取得pending migrations
            var migrations = await _context.Database.GetPendingMigrationsAsync();
            return migrations.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"取得Migration資訊失敗: {ex.Message}");
            return new List<string> { "模擬Migration: InitialCreate" };
        }
    }

    public async Task<(bool, string)> ApplyMigrationsAsync()
    {
        try
        {
            // 在模擬模式下，模擬套用migration
            if (_configuration.GetValue<bool>("SimulationMode", false))
            {
                await Task.Delay(200); // 模擬處理時間
                var output = "✅ 模擬成功: 資料庫已更新到最新的 Migration。";
                Console.WriteLine(output);
                return (true, output);
            }

            // 實際套用migrations
            await _context.Database.MigrateAsync();
            var successMessage = "✅ 資料庫已成功更新！";
            return (true, successMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"❌ 套用Migration失敗: {ex.Message}";
            Console.WriteLine(errorMessage);
            return (false, errorMessage);
        }
    }

    private string GenerateSimulatedCreateTableScript()
    {
        return @"
-- 模擬的Oracle建表腳本 (不會實際執行)
CREATE TABLE EMPLOYEES (
    EMPLOYEE_ID NVARCHAR2(50) NOT NULL,
    EMPLOYEE_NAME NVARCHAR2(200) NOT NULL,
    DEPARTMENT NVARCHAR2(100) NOT NULL,
    REGION NVARCHAR2(100) NOT NULL,
    POSITION NVARCHAR2(100) NOT NULL,
    HIRE_DATE TIMESTAMP(7) NOT NULL,
    SALARY DECIMAL(18,2) NOT NULL,
    CREATED_DATE TIMESTAMP(7) NOT NULL,
    UPDATED_DATE TIMESTAMP(7) NOT NULL,
    CONSTRAINT PK_EMPLOYEES PRIMARY KEY (EMPLOYEE_ID)
);

CREATE TABLE FINANCIAL_RECORDS (
    RECORD_ID NUMBER(10) NOT NULL,
    EMPLOYEE_ID NVARCHAR2(50) NOT NULL,
    RECORD_DATE TIMESTAMP(7) NOT NULL,
    AMOUNT DECIMAL(18,2) NOT NULL,
    TRANSACTION_TYPE NVARCHAR2(50) NOT NULL,
    DESCRIPTION NVARCHAR2(500) NOT NULL,
    REGION NVARCHAR2(100) NOT NULL,
    CATEGORY NVARCHAR2(100) NOT NULL,
    STATUS NVARCHAR2(50) NOT NULL,
    CREATED_DATE TIMESTAMP(7) NOT NULL,
    UPDATED_DATE TIMESTAMP(7) NOT NULL,
    CONSTRAINT PK_FINANCIAL_RECORDS PRIMARY KEY (RECORD_ID)
);

CREATE TABLE COMPANY_ANNOUNCEMENTS (
    ANNOUNCEMENT_ID NUMBER(10) NOT NULL,
    TITLE NVARCHAR2(500) NOT NULL,
    CONTENT NVARCHAR2(2000) NOT NULL,
    ANNOUNCEMENT_TYPE NVARCHAR2(100) NOT NULL,
    PUBLISHED_DATE TIMESTAMP(7) NOT NULL,
    PUBLISHER NVARCHAR2(100) NOT NULL,
    PRIORITY NVARCHAR2(20) NOT NULL,
    STATUS NVARCHAR2(50) NOT NULL,
    CREATED_DATE TIMESTAMP(7) NOT NULL,
    UPDATED_DATE TIMESTAMP(7) NOT NULL,
    CONSTRAINT PK_COMPANY_ANNOUNCEMENTS PRIMARY KEY (ANNOUNCEMENT_ID)
);

-- 建立索引以提升查詢效能
CREATE INDEX IX_COMPANY_ANNOUNCEMENTS_TYPE ON COMPANY_ANNOUNCEMENTS (ANNOUNCEMENT_TYPE);
CREATE INDEX IX_COMPANY_ANNOUNCEMENTS_STATUS ON COMPANY_ANNOUNCEMENTS (STATUS);
CREATE INDEX IX_COMPANY_ANNOUNCEMENTS_PUBLISHED_DATE ON COMPANY_ANNOUNCEMENTS (PUBLISHED_DATE);

-- 注意：此為模擬腳本，實際使用時請使用真實的Oracle資料庫
";
    }

    public string GetDatabaseConfigurationInfo()
    {
        var useOracle = _configuration.GetValue<bool>("UseOracleDatabase", false);
        var simulationMode = _configuration.GetValue<bool>("SimulationMode", false);
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        return $@"
資料庫配置資訊：
- 使用Oracle資料庫: {useOracle}
- 模擬模式: {simulationMode}
- 連線字串: {connectionString}
- 實際使用: {(simulationMode ? "記憶體資料庫 (模擬)" : (useOracle ? "Oracle資料庫" : "記憶體資料庫"))}
";
    }

    // --- DTOs for simulated data ---
    
    public class EmployeeDto
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? Region { get; set; }
        public string? Position { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
    }

    // [修改] Stock Purchase DTO - 股票購買記錄
    public class FinancialRecordDto
    {
        public int RecordId { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime RecordDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; } // 買入/賣出
        public string? Description { get; set; }
        public string? Region { get; set; }
        public string? Category { get; set; } // 股票代號
        public string? Status { get; set; }
        public string? StockCode { get; set; } // 股票代號
        public string? CompanyName { get; set; } // 公司名稱
        public int Quantity { get; set; } // 股數
        public decimal PricePerShare { get; set; } // 每股價格
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    // [修改] Stock Company Announcement DTO - 股票公司重訊
    public class CompanyAnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? AnnouncementType { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? Publisher { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public string? StockCode { get; set; } // 股票代號
        public string? CompanyName { get; set; } // 公司名稱
        public string? EmployeeId { get; set; } // 員工代碼 - 用於關聯查詢
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public async Task<List<EmployeeDto>> GetSimulatedEmployeesAsync()
    {
        await Task.Delay(50); // 模擬查詢延遲
        return new List<EmployeeDto>
        {
            new EmployeeDto { EmployeeId = "E001", EmployeeName = "王小明", Department = "財務部", Region = "台北", Position = "會計", HireDate = new DateTime(2020,1,15), Salary = 52000 },
            new EmployeeDto { EmployeeId = "E002", EmployeeName = "李美麗", Department = "人資部", Region = "新竹", Position = "人資專員", HireDate = new DateTime(2019,5,10), Salary = 48000 },
            new EmployeeDto { EmployeeId = "E003", EmployeeName = "陳大華", Department = "資訊部", Region = "台中", Position = "工程師", HireDate = new DateTime(2021,3,1), Salary = 60000 },
            new EmployeeDto { EmployeeId = "E004", EmployeeName = "林志玲", Department = "行銷部", Region = "高雄", Position = "行銷經理", HireDate = new DateTime(2018,7,20), Salary = 75000 },
            new EmployeeDto { EmployeeId = "E005", EmployeeName = "張偉", Department = "財務部", Region = "台北", Position = "審計", HireDate = new DateTime(2022,2,14), Salary = 53000 },
            new EmployeeDto { EmployeeId = "E006", EmployeeName = "黃小強", Department = "資訊部", Region = "新竹", Position = "資安工程師", HireDate = new DateTime(2020,11,30), Salary = 67000 },
            new EmployeeDto { EmployeeId = "E007", EmployeeName = "吳佩珊", Department = "人資部", Region = "台中", Position = "招募專員", HireDate = new DateTime(2017,9,5), Salary = 46000 },
            new EmployeeDto { EmployeeId = "E008", EmployeeName = "周杰倫", Department = "行銷部", Region = "高雄", Position = "品牌主管", HireDate = new DateTime(2016,4,18), Salary = 82000 },
            new EmployeeDto { EmployeeId = "E009", EmployeeName = "鄭伊健", Department = "財務部", Region = "台北", Position = "財務分析師", HireDate = new DateTime(2023,1,2), Salary = 55000 },
            new EmployeeDto { EmployeeId = "E010", EmployeeName = "劉德華", Department = "資訊部", Region = "新竹", Position = "系統架構師", HireDate = new DateTime(2015,8,25), Salary = 90000 }
        };
    }

    // [修改] 產生模擬股票購買記錄的方法
    public async Task<List<FinancialRecordDto>> GetSimulatedFinancialRecordsAsync()
    {
        await Task.Delay(80); // 模擬查詢延遲
        var now = DateTime.UtcNow;

        return new List<FinancialRecordDto>
        {
            // === E001 王小明 (財務部-台北) - 台積電投資 ===
            new FinancialRecordDto { RecordId = 101, EmployeeId = "E001", Region="台北", RecordDate = now.AddDays(-60), Amount = 60000, TransactionType = "買入", Description = "台積電股票購買", Category = "2330", Status = "已成交", StockCode = "2330", CompanyName = "台積電", Quantity = 100, PricePerShare = 600, CreatedDate = now.AddDays(-60), UpdatedDate = now.AddDays(-60) },
            new FinancialRecordDto { RecordId = 102, EmployeeId = "E001", Region="台北", RecordDate = now.AddDays(-30), Amount = 30500, TransactionType = "賣出", Description = "台積電股票賣出", Category = "2330", Status = "已成交", StockCode = "2330", CompanyName = "台積電", Quantity = 50, PricePerShare = 610, CreatedDate = now.AddDays(-30), UpdatedDate = now.AddDays(-30) },
            new FinancialRecordDto { RecordId = 103, EmployeeId = "E001", Region="台北", RecordDate = now.AddDays(-5), Amount = 61000, TransactionType = "買入", Description = "台積電股票購買", Category = "2330", Status = "已成交", StockCode = "2330", CompanyName = "台積電", Quantity = 100, PricePerShare = 610, CreatedDate = now.AddDays(-5), UpdatedDate = now.AddDays(-5) },
            
            // === E002 李美麗 (人資部-新竹) - 聯發科投資 ===
            new FinancialRecordDto { RecordId = 104, EmployeeId = "E002", Region="新竹", RecordDate = now.AddDays(-35), Amount = 40000, TransactionType = "買入", Description = "聯發科股票購買", Category = "2454", Status = "已成交", StockCode = "2454", CompanyName = "聯發科", Quantity = 50, PricePerShare = 800, CreatedDate = now.AddDays(-35), UpdatedDate = now.AddDays(-35) },
            new FinancialRecordDto { RecordId = 105, EmployeeId = "E002", Region="新竹", RecordDate = now.AddDays(-15), Amount = 32000, TransactionType = "買入", Description = "聯發科股票購買", Category = "2454", Status = "已成交", StockCode = "2454", CompanyName = "聯發科", Quantity = 40, PricePerShare = 800, CreatedDate = now.AddDays(-15), UpdatedDate = now.AddDays(-15) },
            
            // === E003 陳大華 (資訊部-台中) - 台積電投資 ===
            new FinancialRecordDto { RecordId = 106, EmployeeId = "E003", Region="台中", RecordDate = now.AddDays(-55), Amount = 120000, TransactionType = "買入", Description = "台積電股票購買", Category = "2330", Status = "已成交", StockCode = "2330", CompanyName = "台積電", Quantity = 200, PricePerShare = 600, CreatedDate = now.AddDays(-55), UpdatedDate = now.AddDays(-55) },
            new FinancialRecordDto { RecordId = 107, EmployeeId = "E003", Region="台中", RecordDate = now.AddDays(-20), Amount = 61200, TransactionType = "賣出", Description = "台積電股票賣出", Category = "2330", Status = "已成交", StockCode = "2330", CompanyName = "台積電", Quantity = 100, PricePerShare = 612, CreatedDate = now.AddDays(-20), UpdatedDate = now.AddDays(-20) },
            
            // === E004 林志玲 (行銷部-高雄) - 鴻海投資 ===
            new FinancialRecordDto { RecordId = 108, EmployeeId = "E004", Region="高雄", RecordDate = now.AddDays(-50), Amount = 50000, TransactionType = "買入", Description = "鴻海股票購買", Category = "2317", Status = "已成交", StockCode = "2317", CompanyName = "鴻海", Quantity = 500, PricePerShare = 100, CreatedDate = now.AddDays(-50), UpdatedDate = now.AddDays(-50) },
            new FinancialRecordDto { RecordId = 109, EmployeeId = "E004", Region="高雄", RecordDate = now.AddDays(-2), Amount = 25500, TransactionType = "賣出", Description = "鴻海股票賣出", Category = "2317", Status = "已成交", StockCode = "2317", CompanyName = "鴻海", Quantity = 250, PricePerShare = 102, CreatedDate = now.AddDays(-2), UpdatedDate = now.AddDays(-2) },
            
            // === E005 張偉 (財務部-台北) - 中華電投資 ===
            new FinancialRecordDto { RecordId = 110, EmployeeId = "E005", Region="台北", RecordDate = now.AddDays(-20), Amount = 22000, TransactionType = "買入", Description = "中華電股票購買", Category = "2412", Status = "已成交", StockCode = "2412", CompanyName = "中華電", Quantity = 200, PricePerShare = 110, CreatedDate = now.AddDays(-20), UpdatedDate = now.AddDays(-20) },
            new FinancialRecordDto { RecordId = 111, EmployeeId = "E005", Region="台北", RecordDate = now.AddDays(-12), Amount = 33000, TransactionType = "買入", Description = "中華電股票購買", Category = "2412", Status = "已成交", StockCode = "2412", CompanyName = "中華電", Quantity = 300, PricePerShare = 110, CreatedDate = now.AddDays(-12), UpdatedDate = now.AddDays(-12) },
            
            // === E006 黃小強 (資訊部-新竹) - 鴻海投資 ===
            new FinancialRecordDto { RecordId = 112, EmployeeId = "E006", Region="新竹", RecordDate = now.AddDays(-45), Amount = 20000, TransactionType = "買入", Description = "鴻海股票購買", Category = "2317", Status = "已成交", StockCode = "2317", CompanyName = "鴻海", Quantity = 200, PricePerShare = 100, CreatedDate = now.AddDays(-45), UpdatedDate = now.AddDays(-45) },
            new FinancialRecordDto { RecordId = 113, EmployeeId = "E006", Region="新竹", RecordDate = now.AddDays(-25), Amount = 30000, TransactionType = "買入", Description = "鴻海股票購買", Category = "2317", Status = "已成交", StockCode = "2317", CompanyName = "鴻海", Quantity = 300, PricePerShare = 100, CreatedDate = now.AddDays(-25), UpdatedDate = now.AddDays(-25) },
            
            // === E007 吳佩珊 (人資部-台中) - 台塑投資 ===
            new FinancialRecordDto { RecordId = 114, EmployeeId = "E007", Region="台中", RecordDate = now.AddDays(-15), Amount = 45000, TransactionType = "買入", Description = "台塑股票購買", Category = "1301", Status = "已成交", StockCode = "1301", CompanyName = "台塑", Quantity = 500, PricePerShare = 90, CreatedDate = now.AddDays(-15), UpdatedDate = now.AddDays(-15) },
            new FinancialRecordDto { RecordId = 115, EmployeeId = "E007", Region="台中", RecordDate = now.AddDays(-8), Amount = 18000, TransactionType = "買入", Description = "台塑股票購買", Category = "1301", Status = "已成交", StockCode = "1301", CompanyName = "台塑", Quantity = 200, PricePerShare = 90, CreatedDate = now.AddDays(-8), UpdatedDate = now.AddDays(-8) },
            
            // === E008 周杰倫 (行銷部-高雄) - 台塑投資 ===
            new FinancialRecordDto { RecordId = 116, EmployeeId = "E008", Region="高雄", RecordDate = now.AddDays(-10), Amount = 18000, TransactionType = "買入", Description = "台塑股票購買", Category = "1301", Status = "已成交", StockCode = "1301", CompanyName = "台塑", Quantity = 200, PricePerShare = 90, CreatedDate = now.AddDays(-10), UpdatedDate = now.AddDays(-10) },
            new FinancialRecordDto { RecordId = 117, EmployeeId = "E008", Region="高雄", RecordDate = now.AddDays(-6), Amount = 27000, TransactionType = "買入", Description = "台塑股票購買", Category = "1301", Status = "已成交", StockCode = "1301", CompanyName = "台塑", Quantity = 300, PricePerShare = 90, CreatedDate = now.AddDays(-6), UpdatedDate = now.AddDays(-6) },
            
            // === E009 鄭伊健 (財務部-台北) - 中華電投資 ===
            new FinancialRecordDto { RecordId = 118, EmployeeId = "E009", Region="台北", RecordDate = now.AddDays(-25), Amount = 55000, TransactionType = "買入", Description = "中華電股票購買", Category = "2412", Status = "已成交", StockCode = "2412", CompanyName = "中華電", Quantity = 500, PricePerShare = 110, CreatedDate = now.AddDays(-25), UpdatedDate = now.AddDays(-25) },
            new FinancialRecordDto { RecordId = 119, EmployeeId = "E009", Region="台北", RecordDate = now.AddDays(-18), Amount = 22000, TransactionType = "買入", Description = "中華電股票購買", Category = "2412", Status = "已成交", StockCode = "2412", CompanyName = "中華電", Quantity = 200, PricePerShare = 110, CreatedDate = now.AddDays(-18), UpdatedDate = now.AddDays(-18) },
            
            // === E010 劉德華 (資訊部-新竹) - 聯發科投資 ===
            new FinancialRecordDto { RecordId = 120, EmployeeId = "E010", Region="新竹", RecordDate = now.AddDays(-40), Amount = 80000, TransactionType = "買入", Description = "聯發科股票購買", Category = "2454", Status = "已成交", StockCode = "2454", CompanyName = "聯發科", Quantity = 100, PricePerShare = 800, CreatedDate = now.AddDays(-40), UpdatedDate = now.AddDays(-40) },
            new FinancialRecordDto { RecordId = 121, EmployeeId = "E010", Region="新竹", RecordDate = now.AddDays(-28), Amount = 40000, TransactionType = "買入", Description = "聯發科股票購買", Category = "2454", Status = "已成交", StockCode = "2454", CompanyName = "聯發科", Quantity = 50, PricePerShare = 800, CreatedDate = now.AddDays(-28), UpdatedDate = now.AddDays(-28) },
        };
    }

    // [修改] 產生模擬股票公司重訊資料的方法
    public async Task<List<CompanyAnnouncementDto>> GetSimulatedCompanyAnnouncementsAsync()
    {
        await Task.Delay(60); // 模擬查詢延遲
        var now = DateTime.UtcNow;

        return new List<CompanyAnnouncementDto>
        {
            // === E001 王小明 的相關重訊 (台積電) ===
            new CompanyAnnouncementDto { AnnouncementId = 1, Title = "台積電2024年第二季營收創新高", Content = "台積電公佈2024年第二季營收達新台幣6,253億元，較去年同期成長32.8%，毛利率達53.2%。", AnnouncementType = "財報發佈", PublishedDate = now.AddDays(-30), Publisher = "台積電", Priority = "高", Status = "已發佈", StockCode = "2330", CompanyName = "台積電", EmployeeId = "E001", CreatedDate = now.AddDays(-30), UpdatedDate = now.AddDays(-30) },
            new CompanyAnnouncementDto { AnnouncementId = 2, Title = "台積電董事會通過股利發放", Content = "台積電董事會決議每股配發現金股利11元，配息率約70%，除息日定為8月15日。", AnnouncementType = "股利發放", PublishedDate = now.AddDays(-10), Publisher = "台積電", Priority = "中", Status = "已發佈", StockCode = "2330", CompanyName = "台積電", EmployeeId = "E001", CreatedDate = now.AddDays(-10), UpdatedDate = now.AddDays(-10) },
            new CompanyAnnouncementDto { AnnouncementId = 3, Title = "台積電獲蘋果A19晶片大單", Content = "台積電確認獲得蘋果A19處理器代工訂單，採用2奈米製程，預計2025年第四季量產。", AnnouncementType = "重大合約", PublishedDate = now.AddDays(-1), Publisher = "台積電", Priority = "高", Status = "已發佈", StockCode = "2330", CompanyName = "台積電", EmployeeId = "E001", CreatedDate = now.AddDays(-1), UpdatedDate = now.AddDays(-1) },

            // === E002 李美麗 的相關重訊 (聯發科) ===
            new CompanyAnnouncementDto { AnnouncementId = 4, Title = "聯發科擴大AI晶片研發投資", Content = "聯發科宣佈投入200億台幣發展AI晶片技術，預計2025年推出專用AI處理器。", AnnouncementType = "研發投資", PublishedDate = now.AddDays(-8), Publisher = "聯發科", Priority = "中", Status = "已發佈", StockCode = "2454", CompanyName = "聯發科", EmployeeId = "E002", CreatedDate = now.AddDays(-8), UpdatedDate = now.AddDays(-8) },
            new CompanyAnnouncementDto { AnnouncementId = 5, Title = "聯發科5G晶片銷售創新高", Content = "聯發科第二季5G晶片出貨量達1.2億顆，市佔率提升至35%。", AnnouncementType = "營運成果", PublishedDate = now.AddDays(-18), Publisher = "聯發科", Priority = "中", Status = "已發佈", StockCode = "2454", CompanyName = "聯發科", EmployeeId = "E002", CreatedDate = now.AddDays(-18), UpdatedDate = now.AddDays(-18) },

            // === E003 陳大華 的相關重訊 (台積電) ===
            new CompanyAnnouncementDto { AnnouncementId = 6, Title = "台積電宣佈美國亞利桑那州第二廠動工", Content = "台積電美國亞利桑那州第二座晶圓廠正式動工，預計2026年開始量產2奈米製程。", AnnouncementType = "重大投資", PublishedDate = now.AddDays(-20), Publisher = "台積電", Priority = "高", Status = "已發佈", StockCode = "2330", CompanyName = "台積電", EmployeeId = "E003", CreatedDate = now.AddDays(-20), UpdatedDate = now.AddDays(-20) },
            new CompanyAnnouncementDto { AnnouncementId = 7, Title = "台積電3奈米製程產能滿載", Content = "台積電3奈米製程訂單持續增加，產能利用率達到100%，預計擴建新產線。", AnnouncementType = "產能擴張", PublishedDate = now.AddDays(-12), Publisher = "台積電", Priority = "中", Status = "已發佈", StockCode = "2330", CompanyName = "台積電", EmployeeId = "E003", CreatedDate = now.AddDays(-12), UpdatedDate = now.AddDays(-12) },

            // === E004 林志玲 的相關重訊 (鴻海) ===
            new CompanyAnnouncementDto { AnnouncementId = 8, Title = "鴻海電動車事業獲重大訂單", Content = "鴻海集團電動車平台MIH獲得歐洲知名車廠合作案，預計2025年交付首批電動車。", AnnouncementType = "重大合約", PublishedDate = now.AddDays(-25), Publisher = "鴻海", Priority = "高", Status = "已發佈", StockCode = "2317", CompanyName = "鴻海", EmployeeId = "E004", CreatedDate = now.AddDays(-25), UpdatedDate = now.AddDays(-25) },
            new CompanyAnnouncementDto { AnnouncementId = 9, Title = "鴻海第二季營收超出預期", Content = "鴻海第二季營收達1.2兆台幣，較預期高出8%，主要受惠於iPhone訂單增加。", AnnouncementType = "財報發佈", PublishedDate = now.AddDays(-35), Publisher = "鴻海", Priority = "中", Status = "已發佈", StockCode = "2317", CompanyName = "鴻海", EmployeeId = "E004", CreatedDate = now.AddDays(-35), UpdatedDate = now.AddDays(-35) },

            // === E005 張偉 的相關重訊 (中華電) ===
            new CompanyAnnouncementDto { AnnouncementId = 10, Title = "中華電信攜手微軟發展雲端服務", Content = "中華電信與Microsoft簽署戰略合作協議，共同發展企業雲端解決方案。", AnnouncementType = "策略合作", PublishedDate = now.AddDays(-5), Publisher = "中華電信", Priority = "中", Status = "已發佈", StockCode = "2412", CompanyName = "中華電", EmployeeId = "E005", CreatedDate = now.AddDays(-5), UpdatedDate = now.AddDays(-5) },
            new CompanyAnnouncementDto { AnnouncementId = 11, Title = "中華電信5G基站建設超前達標", Content = "中華電信5G基站建設進度超前，已建置完成12,000座基站，覆蓋率達85%。", AnnouncementType = "營運成果", PublishedDate = now.AddDays(-22), Publisher = "中華電信", Priority = "中", Status = "已發佈", StockCode = "2412", CompanyName = "中華電", EmployeeId = "E005", CreatedDate = now.AddDays(-22), UpdatedDate = now.AddDays(-22) },

            // === E006 黃小強 的相關重訊 (鴻海) ===
            new CompanyAnnouncementDto { AnnouncementId = 12, Title = "鴻海印度新廠正式投產", Content = "鴻海印度iPhone組裝廠正式投產，預計年產能達2000萬支，強化南亞供應鏈佈局。", AnnouncementType = "產能擴張", PublishedDate = now.AddDays(-15), Publisher = "鴻海", Priority = "中", Status = "已發佈", StockCode = "2317", CompanyName = "鴻海", EmployeeId = "E006", CreatedDate = now.AddDays(-15), UpdatedDate = now.AddDays(-15) },
            new CompanyAnnouncementDto { AnnouncementId = 13, Title = "鴻海AI伺服器業務快速成長", Content = "鴻海AI伺服器業務第二季營收成長150%，成為新的營收成長動能。", AnnouncementType = "營運成果", PublishedDate = now.AddDays(-28), Publisher = "鴻海", Priority = "中", Status = "已發佈", StockCode = "2317", CompanyName = "鴻海", EmployeeId = "E006", CreatedDate = now.AddDays(-28), UpdatedDate = now.AddDays(-28) },

            // === E007 吳佩珊 的相關重訊 (台塑) ===
            new CompanyAnnouncementDto { AnnouncementId = 14, Title = "台塑石化擴建乙烯產能", Content = "台塑石化宣佈投資300億元擴建乙烯產能，預計2026年完工，年產能提升30%。", AnnouncementType = "產能擴張", PublishedDate = now.AddDays(-22), Publisher = "台塑", Priority = "高", Status = "已發佈", StockCode = "1301", CompanyName = "台塑", EmployeeId = "E007", CreatedDate = now.AddDays(-22), UpdatedDate = now.AddDays(-22) },
            new CompanyAnnouncementDto { AnnouncementId = 15, Title = "台塑綠能轉型計畫啟動", Content = "台塑啟動綠能轉型計畫，投資200億元發展風力發電和太陽能事業。", AnnouncementType = "重大投資", PublishedDate = now.AddDays(-14), Publisher = "台塑", Priority = "高", Status = "已發佈", StockCode = "1301", CompanyName = "台塑", EmployeeId = "E007", CreatedDate = now.AddDays(-14), UpdatedDate = now.AddDays(-14) },

            // === E008 周杰倫 的相關重訊 (台塑) ===
            new CompanyAnnouncementDto { AnnouncementId = 16, Title = "台塑生醫COVID-19疫苗三期試驗成功", Content = "台塑生醫新冠疫苗完成三期臨床試驗，保護力達95%，預計向衛福部申請緊急使用授權。", AnnouncementType = "研發成果", PublishedDate = now.AddDays(-3), Publisher = "台塑", Priority = "高", Status = "已發佈", StockCode = "1301", CompanyName = "台塑", EmployeeId = "E008", CreatedDate = now.AddDays(-3), UpdatedDate = now.AddDays(-3) },
            new CompanyAnnouncementDto { AnnouncementId = 17, Title = "台塑四寶聯合減碳計畫", Content = "台塑四寶宣佈聯合減碳計畫，預計2030年碳排放量減少50%，投資綠色科技。", AnnouncementType = "環保政策", PublishedDate = now.AddDays(-26), Publisher = "台塑", Priority = "中", Status = "已發佈", StockCode = "1301", CompanyName = "台塑", EmployeeId = "E008", CreatedDate = now.AddDays(-26), UpdatedDate = now.AddDays(-26) },

            // === E009 鄭伊健 的相關重訊 (中華電) ===
            new CompanyAnnouncementDto { AnnouncementId = 18, Title = "中華電信5G用戶突破400萬", Content = "中華電信5G用戶數正式突破400萬，滲透率達37%，持續領先電信三雄。", AnnouncementType = "營運成果", PublishedDate = now.AddDays(-12), Publisher = "中華電信", Priority = "中", Status = "已發佈", StockCode = "2412", CompanyName = "中華電", EmployeeId = "E009", CreatedDate = now.AddDays(-12), UpdatedDate = now.AddDays(-12) },
            new CompanyAnnouncementDto { AnnouncementId = 19, Title = "中華電信IoT物聯網平台上線", Content = "中華電信推出全新IoT物聯網平台，提供企業級物聯網解決方案，預計年底服務10萬家企業。", AnnouncementType = "新品發佈", PublishedDate = now.AddDays(-32), Publisher = "中華電信", Priority = "中", Status = "已發佈", StockCode = "2412", CompanyName = "中華電", EmployeeId = "E009", CreatedDate = now.AddDays(-32), UpdatedDate = now.AddDays(-32) },

            // === E010 劉德華 的相關重訊 (聯發科) ===
            new CompanyAnnouncementDto { AnnouncementId = 20, Title = "聯發科發佈天璣9400旗艦晶片", Content = "聯發科正式發佈天璣9400處理器，採用台積電3奈米製程，AI運算效能提升40%。", AnnouncementType = "新品發佈", PublishedDate = now.AddDays(-18), Publisher = "聯發科", Priority = "高", Status = "已發佈", StockCode = "2454", CompanyName = "聯發科", EmployeeId = "E010", CreatedDate = now.AddDays(-18), UpdatedDate = now.AddDays(-18) },
            new CompanyAnnouncementDto { AnnouncementId = 21, Title = "聯發科WiFi 7晶片量產出貨", Content = "聯發科WiFi 7晶片正式量產出貨，預計第三季出貨量達500萬顆，搶佔高階市場。", AnnouncementType = "產能擴張", PublishedDate = now.AddDays(-24), Publisher = "聯發科", Priority = "中", Status = "已發佈", StockCode = "2454", CompanyName = "聯發科", EmployeeId = "E010", CreatedDate = now.AddDays(-24), UpdatedDate = now.AddDays(-24) },
        };
    }

    // 根據員工ID查詢相關財務記錄
    public async Task<List<FinancialRecordDto>> GetEmployeeFinancialRecordsAsync(string employeeId)
    {
        var allRecords = await GetSimulatedFinancialRecordsAsync();
        return allRecords.Where(r => r.EmployeeId == employeeId).ToList();
    }

    // 根據員工ID查詢相關公司重訊
    public async Task<List<CompanyAnnouncementDto>> GetEmployeeRelatedAnnouncementsAsync(string employeeId)
    {
        var allAnnouncements = await GetSimulatedCompanyAnnouncementsAsync();
        var announcements = allAnnouncements.Where(a => a.EmployeeId == employeeId).ToList();
        Console.WriteLine($"查詢員工 {employeeId} 的相關公司重訊，共找到 {announcements.Count} 筆資料");
        return announcements;
    }

    // 根據員工ID查詢員工基本資料
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(string employeeId)
    {
        var allEmployees = await GetSimulatedEmployeesAsync();
        return allEmployees.FirstOrDefault(e => e.EmployeeId == employeeId);
    }

    // 根據員工ID生成完整的員工金融報告資料
    public async Task<EmployeeFinancialReportDto> GetEmployeeFinancialReportAsync(string employeeId)
    {
        var employee = await GetEmployeeByIdAsync(employeeId);
        var financialRecords = await GetEmployeeFinancialRecordsAsync(employeeId);
        var announcements = await GetEmployeeRelatedAnnouncementsAsync(employeeId);

        return new EmployeeFinancialReportDto
        {
            Employee = employee,
            FinancialRecords = financialRecords,
            CompanyAnnouncements = announcements,
            GeneratedAt = DateTime.UtcNow
        };
    }

    // 員工金融報告 DTO
    public class EmployeeFinancialReportDto
    {
        public EmployeeDto? Employee { get; set; }
        public List<FinancialRecordDto> FinancialRecords { get; set; } = new();
        public List<CompanyAnnouncementDto> CompanyAnnouncements { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    // 測試資料源查詢功能
    public async Task<string> TestDataSourceQueriesAsync(string employeeId)
    {
        var results = new List<string>();
        
        try
        {
            // 測試員工資料查詢
            var employee = await this.GetEmployeeByIdAsync(employeeId);
            if (employee != null)
            {
                results.Add($"✅ 員工資料查詢成功: {employee.EmployeeName} ({employee.EmployeeId})");
            }
            else
            {
                results.Add($"❌ 員工資料查詢失敗: 找不到員工 {employeeId}");
            }
            
            // 測試財務記錄查詢
            var financialRecords = await this.GetEmployeeFinancialRecordsAsync(employeeId);
            results.Add($"✅ 財務記錄查詢成功: 找到 {financialRecords.Count} 筆記錄");
            
            // 測試公司重訊查詢
            var announcements = await this.GetEmployeeRelatedAnnouncementsAsync(employeeId);
            results.Add($"✅ 公司重訊查詢成功: 找到 {announcements.Count} 筆相關重訊");
            
            // 測試完整報告生成
            var fullReport = await this.GetEmployeeFinancialReportAsync(employeeId);
            results.Add($"✅ 完整報告生成成功");
            results.Add($"   - 員工資料: {(fullReport.Employee != null ? "有" : "無")}");
            results.Add($"   - 財務記錄: {fullReport.FinancialRecords.Count} 筆");
            results.Add($"   - 公司重訊: {fullReport.CompanyAnnouncements.Count} 筆");
            
        }
        catch (Exception ex)
        {
            results.Add($"❌ 測試過程中發生錯誤: {ex.Message}");
        }
        
        return string.Join("\n", results);
    }
}