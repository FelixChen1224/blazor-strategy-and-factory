using blazor_strategy_and_factory.Models;
using blazor_strategy_and_factory.Services.DataSources;
using blazor_strategy_and_factory.Services.OutputComponents;
using blazor_strategy_and_factory.Data;
using blazor_strategy_and_factory.Services;

namespace blazor_strategy_and_factory.Services.Factory;

public interface IPageFactory
{
    Task<PageConfiguration> CreatePageAsync(FinancialDataRequest request);
}

public class PageConfiguration
{
    public string PageTitle { get; set; } = string.Empty;
    public List<IDataSourceStrategy> DataSources { get; set; } = new();
    public List<IOutputComponentStrategy> OutputComponents { get; set; } = new();
    public List<FinancialDataResponse> DataResponses { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class FinancialPageFactory : IPageFactory
{
    private readonly IDataSourceService _dataSourceService;
    private readonly IOutputComponentService _outputComponentService;

    public FinancialPageFactory(IDataSourceService dataSourceService, IOutputComponentService outputComponentService)
    {
        _dataSourceService = dataSourceService;
        _outputComponentService = outputComponentService;
    }

    public async Task<PageConfiguration> CreatePageAsync(FinancialDataRequest request)
    {
        var pageConfig = new PageConfiguration
        {
            PageTitle = GeneratePageTitle(request)
        };

        // 根據請求選擇合適的資料源策略
        foreach (var dataSourceName in request.DataSources)
        {
            var strategy = _dataSourceService.GetStrategy(dataSourceName);
            if (strategy != null)
            {
                pageConfig.DataSources.Add(strategy);
                
                // 立即執行資料擷取
                var response = await strategy.FetchDataAsync(request);
                pageConfig.DataResponses.Add(response);
            }
        }

        // 根據請求選擇合適的輸出組件策略
        foreach (var componentName in request.OutputComponents)
        {
            var strategy = _outputComponentService.GetStrategy(componentName);
            if (strategy != null)
            {
                pageConfig.OutputComponents.Add(strategy);
            }
        }

        return pageConfig;
    }

    private string GeneratePageTitle(FinancialDataRequest request)
    {
        var title = "資料庫金融資訊摘要";
        
        if (!string.IsNullOrEmpty(request.EmployeeId))
        {
            title += $" - 員工: {request.EmployeeId}";
        }
        
        if (!string.IsNullOrEmpty(request.Region))
        {
            title += $" - 區域: {request.Region}";
        }
        
        if (request.StartDate.HasValue)
        {
            title += $" - 日期: {request.StartDate:yyyy-MM-dd}";
        }
        
        return title;
    }
}

public interface IDataSourceService
{
    IDataSourceStrategy? GetStrategy(string dataSourceName);
    List<IDataSourceStrategy> GetAllStrategies();
}

public class DataSourceService : IDataSourceService
{
    private readonly IServiceProvider _serviceProvider;

    public DataSourceService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDataSourceStrategy? GetStrategy(string dataSourceName)
    {
        // 直接從服務容器獲取所需的服務
        var dbContext = _serviceProvider.GetRequiredService<FinancialDbContext>();
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        var oracleSimulationService = _serviceProvider.GetRequiredService<OracleSimulationService>();
        
        var strategies = new List<IDataSourceStrategy>
        {
            new EmployeeDataSource(dbContext, configuration, oracleSimulationService),
            new FinancialRecordDataSource(dbContext, configuration, oracleSimulationService),
            new CompanyAnnouncementDataSource(dbContext, configuration, oracleSimulationService)
        };

        return strategies.FirstOrDefault(s => s.CanHandle(dataSourceName));
    }

    public List<IDataSourceStrategy> GetAllStrategies()
    {
        var dbContext = _serviceProvider.GetRequiredService<FinancialDbContext>();
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        var oracleSimulationService = _serviceProvider.GetRequiredService<OracleSimulationService>();
        
        return new List<IDataSourceStrategy>
        {
            new EmployeeDataSource(dbContext, configuration, oracleSimulationService),
            new FinancialRecordDataSource(dbContext, configuration, oracleSimulationService),
            new CompanyAnnouncementDataSource(dbContext, configuration, oracleSimulationService)
        };
    }
}

public interface IOutputComponentService
{
    IOutputComponentStrategy? GetStrategy(string componentName);
    List<IOutputComponentStrategy> GetAllStrategies();
}

public class OutputComponentService : IOutputComponentService
{
    private readonly List<IOutputComponentStrategy> _strategies;

    public OutputComponentService()
    {
        _strategies = new List<IOutputComponentStrategy>
        {
            new SQLQueryComponent(),
            new DataTableComponent(),
            new SummaryComponent(),
            new ChartComponent()
        };
    }

    public IOutputComponentStrategy? GetStrategy(string componentName)
    {
        return _strategies.FirstOrDefault(s => s.CanHandle(componentName));
    }

    public List<IOutputComponentStrategy> GetAllStrategies()
    {
        return _strategies;
    }
}
