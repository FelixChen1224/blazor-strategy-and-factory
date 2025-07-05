using blazor_strategy_and_factory.Components;
using blazor_strategy_and_factory.Services.Factory;
using blazor_strategy_and_factory.Services.DataSources;
using blazor_strategy_and_factory.Services.OutputComponents;
using blazor_strategy_and_factory.Data;
using blazor_strategy_and_factory.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 配置資料庫連線 (Oracle)
builder.Services.AddDbContext<FinancialDbContext>(options =>
{
    // 可以通過環境變數或配置來切換資料庫模式
    var useOracle = builder.Configuration.GetValue<bool>("UseOracleDatabase", false);
    var simulationMode = builder.Configuration.GetValue<bool>("SimulationMode", false);
    
    if (useOracle && !simulationMode)
    {
        // 使用Oracle資料庫
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseOracle(connectionString, b => b.MigrationsAssembly("blazor_strategy_and_factory"));
        options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        options.EnableDetailedErrors(builder.Environment.IsDevelopment());
    }
    else
    {
        // 使用記憶體資料庫進行演示/模擬
        options.UseInMemoryDatabase("FinancialDb");
        Console.WriteLine("使用記憶體資料庫模式進行模擬");
    }
});

// 註冊策略模式和工廠模式的服務
builder.Services.AddScoped<IDataSourceService, DataSourceService>();
builder.Services.AddScoped<IOutputComponentService, OutputComponentService>();
builder.Services.AddScoped<IPageFactory, FinancialPageFactory>();

// 移除 DatabaseInitializationService - 不再需要

// 註冊Oracle模擬服務
builder.Services.AddScoped<OracleSimulationService>();

// 註冊 Gemini AI 服務
builder.Services.AddHttpClient<GeminiAIService>();
builder.Services.AddScoped<GeminiAIService>();

// 註冊金融分析服務
builder.Services.AddScoped<FinancialAnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 移除資料庫初始化程式碼 - 改用模擬資料服務

app.Run();
