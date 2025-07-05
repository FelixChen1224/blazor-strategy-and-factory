using Microsoft.EntityFrameworkCore;
using blazor_strategy_and_factory.Models.Database;

namespace blazor_strategy_and_factory.Data;

public class FinancialDbContext : DbContext
{
    public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<FinancialRecord> FinancialRecords { get; set; }
    public DbSet<CompanyAnnouncement> CompanyAnnouncements { get; set; }
    public DbSet<MarketData> MarketData { get; set; }
    public DbSet<BudgetRecord> BudgetRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Oracle特定配置
        ConfigureOracleSpecificSettings(modelBuilder);

        // 配置實體關係和索引
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(200);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Salary).HasPrecision(18, 2);
            
            entity.HasIndex(e => e.Region);
            entity.HasIndex(e => e.Department);
        });

        modelBuilder.Entity<FinancialRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.Property(e => e.TransactionType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            
            entity.HasIndex(e => e.EmployeeId);
            entity.HasIndex(e => e.RecordDate);
            entity.HasIndex(e => e.Region);
            entity.HasIndex(e => new { e.EmployeeId, e.RecordDate });
        });

        modelBuilder.Entity<CompanyAnnouncement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Content).HasMaxLength(4000);
            entity.Property(e => e.AnnouncementType).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.ImportanceLevel).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            
            entity.HasIndex(e => e.AnnouncementDate);
            entity.HasIndex(e => e.Region);
            entity.HasIndex(e => e.AnnouncementType);
        });

        modelBuilder.Entity<MarketData>(entity =>
        {
            entity.HasKey(e => e.DataId);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.MarketType).HasMaxLength(50);
            entity.Property(e => e.OpenPrice).HasPrecision(18, 4);
            entity.Property(e => e.ClosePrice).HasPrecision(18, 4);
            entity.Property(e => e.HighPrice).HasPrecision(18, 4);
            entity.Property(e => e.LowPrice).HasPrecision(18, 4);
            entity.Property(e => e.ChangePercent).HasPrecision(8, 4);
            
            entity.HasIndex(e => e.MarketDate);
            entity.HasIndex(e => e.Symbol);
            entity.HasIndex(e => e.Region);
            entity.HasIndex(e => new { e.Symbol, e.MarketDate });
        });

        modelBuilder.Entity<BudgetRecord>(entity =>
        {
            entity.HasKey(e => e.BudgetId);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.PlannedAmount).HasPrecision(18, 2);
            entity.Property(e => e.ActualAmount).HasPrecision(18, 2);
            entity.Property(e => e.Variance).HasPrecision(18, 2);
            
            entity.HasIndex(e => e.Department);
            entity.HasIndex(e => e.Region);
            entity.HasIndex(e => new { e.BudgetYear, e.BudgetMonth });
        });
    }

    private void ConfigureOracleSpecificSettings(ModelBuilder modelBuilder)
    {
        // Oracle資料庫特定設定
        if (Database.IsOracle())
        {
            // 設定Oracle的命名約定
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // 表名使用大寫
                entity.SetTableName(entity.GetTableName()?.ToUpperInvariant());
                
                // 欄位名使用大寫
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnBaseName()?.ToUpperInvariant());
                }
            }
        }
    }
}
