using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blazor_strategy_and_factory.Models.Database;

// 員工資料表
[Table("EMPLOYEES")]
public class Employee
{
    [Key]
    [Column("EMPLOYEE_ID")]
    public string EmployeeId { get; set; } = string.Empty;

    [Column("EMPLOYEE_NAME")]
    public string EmployeeName { get; set; } = string.Empty;

    [Column("DEPARTMENT")]
    public string Department { get; set; } = string.Empty;

    [Column("REGION")]
    public string Region { get; set; } = string.Empty;

    [Column("POSITION")]
    public string Position { get; set; } = string.Empty;

    [Column("HIRE_DATE")]
    public DateTime HireDate { get; set; }

    [Column("SALARY")]
    public decimal Salary { get; set; }

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; }

    [Column("UPDATED_DATE")]
    public DateTime UpdatedDate { get; set; }
}

// 財務資料表
[Table("FINANCIAL_RECORDS")]
public class FinancialRecord
{
    [Key]
    [Column("RECORD_ID")]
    public int RecordId { get; set; }

    [Column("EMPLOYEE_ID")]
    public string EmployeeId { get; set; } = string.Empty;

    [Column("RECORD_DATE")]
    public DateTime RecordDate { get; set; }

    [Column("AMOUNT")]
    public decimal Amount { get; set; }

    [Column("TRANSACTION_TYPE")]
    public string TransactionType { get; set; } = string.Empty;

    [Column("DESCRIPTION")]
    public string Description { get; set; } = string.Empty;

    [Column("REGION")]
    public string Region { get; set; } = string.Empty;

    [Column("CATEGORY")]
    public string Category { get; set; } = string.Empty;

    [Column("STATUS")]
    public string Status { get; set; } = string.Empty;

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; }

    [Column("UPDATED_DATE")]
    public DateTime UpdatedDate { get; set; }
}

// 公司重訊資料表
[Table("COMPANY_ANNOUNCEMENTS")]
public class CompanyAnnouncement
{
    [Key]
    [Column("ANNOUNCEMENT_ID")]
    public int AnnouncementId { get; set; }

    [Column("TITLE")]
    public string Title { get; set; } = string.Empty;

    [Column("CONTENT")]
    public string Content { get; set; } = string.Empty;

    [Column("ANNOUNCEMENT_DATE")]
    public DateTime AnnouncementDate { get; set; }

    [Column("ANNOUNCEMENT_TYPE")]
    public string AnnouncementType { get; set; } = string.Empty;

    [Column("REGION")]
    public string Region { get; set; } = string.Empty;

    [Column("IMPORTANCE_LEVEL")]
    public string ImportanceLevel { get; set; } = string.Empty;

    [Column("STATUS")]
    public string Status { get; set; } = string.Empty;

    [Column("CREATED_BY")]
    public string CreatedBy { get; set; } = string.Empty;

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; }

    [Column("UPDATED_DATE")]
    public DateTime UpdatedDate { get; set; }
}

// 市場資料表
[Table("MARKET_DATA")]
public class MarketData
{
    [Key]
    [Column("DATA_ID")]
    public int DataId { get; set; }

    [Column("SYMBOL")]
    public string Symbol { get; set; } = string.Empty;

    [Column("MARKET_DATE")]
    public DateTime MarketDate { get; set; }

    [Column("OPEN_PRICE")]
    public decimal OpenPrice { get; set; }

    [Column("CLOSE_PRICE")]
    public decimal ClosePrice { get; set; }

    [Column("HIGH_PRICE")]
    public decimal HighPrice { get; set; }

    [Column("LOW_PRICE")]
    public decimal LowPrice { get; set; }

    [Column("VOLUME")]
    public long Volume { get; set; }

    [Column("CHANGE_PERCENT")]
    public decimal ChangePercent { get; set; }

    [Column("REGION")]
    public string Region { get; set; } = string.Empty;

    [Column("MARKET_TYPE")]
    public string MarketType { get; set; } = string.Empty;

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; }

    [Column("UPDATED_DATE")]
    public DateTime UpdatedDate { get; set; }
}

// 預算資料表
[Table("BUDGET_RECORDS")]
public class BudgetRecord
{
    [Key]
    [Column("BUDGET_ID")]
    public int BudgetId { get; set; }

    [Column("DEPARTMENT")]
    public string Department { get; set; } = string.Empty;

    [Column("BUDGET_YEAR")]
    public int BudgetYear { get; set; }

    [Column("BUDGET_MONTH")]
    public int BudgetMonth { get; set; }

    [Column("PLANNED_AMOUNT")]
    public decimal PlannedAmount { get; set; }

    [Column("ACTUAL_AMOUNT")]
    public decimal ActualAmount { get; set; }

    [Column("VARIANCE")]
    public decimal Variance { get; set; }

    [Column("REGION")]
    public string Region { get; set; } = string.Empty;

    [Column("CATEGORY")]
    public string Category { get; set; } = string.Empty;

    [Column("STATUS")]
    public string Status { get; set; } = string.Empty;

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; }

    [Column("UPDATED_DATE")]
    public DateTime UpdatedDate { get; set; }
}
