using System.Text;
using System.Text.Json;

namespace blazor_strategy_and_factory.Services;

public class GeminiAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public GeminiAIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["GeminiAI:ApiKey"] ?? "AIzaSyDTbG2f61LaCrgOukrWSTEMjpwQvj1n4aw";
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        try
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _apiKey);

            var response = await _httpClient.PostAsync(_baseUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                
                return result?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text ?? "無法取得回應";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Gemini API 錯誤: {response.StatusCode} - {errorContent}");
                return $"API 呼叫失敗: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini AI 服務錯誤: {ex.Message}");
            return $"服務錯誤: {ex.Message}";
        }
    }

    public async Task<string> AnalyzeFinancialDataAsync(Dictionary<string, object> data)
    {
        var prompt = $@"
請分析以下財務資料並提供洞察：

資料內容：
{JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })}

請提供：
1. 資料摘要
2. 主要發現
3. 潛在風險或機會
4. 建議行動

請用繁體中文回答，並保持簡潔明瞭。
";

        return await GenerateContentAsync(prompt);
    }

    public async Task<string> ExplainDataPatternAsync(string dataType, int recordCount)
    {
        var prompt = $@"
在財務系統中，{dataType}有{recordCount}筆記錄。
請解釋這個數量是否合理，並說明這類資料的典型用途。
請用繁體中文回答，不超過100字。
";

        return await GenerateContentAsync(prompt);
    }

    // 根據員工金融報告資料進行AI分析
    public async Task<string> AnalyzeEmployeeFinancialReportAsync(string employeeId, 
        object employeeData, 
        List<object> financialRecords, 
        List<object> companyAnnouncements)
    {
        var prompt = $@"
請分析員工 {employeeId} 的完整金融投資報告：

員工基本資料：
{JsonSerializer.Serialize(employeeData, new JsonSerializerOptions { WriteIndented = true })}

財務交易記錄：
{JsonSerializer.Serialize(financialRecords, new JsonSerializerOptions { WriteIndented = true })}

相關公司重訊：
{JsonSerializer.Serialize(companyAnnouncements, new JsonSerializerOptions { WriteIndented = true })}

請提供專業的投資分析報告，包含：
1. 投資組合分析 - 分析該員工的股票持股狀況
2. 投資績效評估 - 評估買賣交易的獲利情況
3. 風險評估 - 分析投資風險和集中度
4. 市場資訊影響 - 分析相關公司重訊對投資的影響
5. 投資建議 - 提供具體的投資建議和策略

請用繁體中文回答，提供詳細且專業的分析。
";

        return await GenerateContentAsync(prompt);
    }

    // 生成員工投資摘要
    public async Task<string> GenerateEmployeeInvestmentSummaryAsync(string employeeId, 
        int totalTransactions, 
        decimal totalInvestment, 
        List<string> stockCodes)
    {
        var prompt = $@"
員工 {employeeId} 的投資摘要：
- 總交易次數：{totalTransactions}
- 總投資金額：{totalInvestment:C}
- 投資股票：{string.Join(", ", stockCodes)}

請生成一份簡潔的投資摘要報告，包含：
1. 投資活躍度評估
2. 投資多元化程度
3. 投資風格分析
4. 總體評價

請用繁體中文回答，不超過200字。
";

        return await GenerateContentAsync(prompt);
    }
}

// Gemini API 回應的資料模型
public class GeminiResponse
{
    public Candidate[]? candidates { get; set; }
}

public class Candidate
{
    public Content? content { get; set; }
}

public class Content
{
    public Part[]? parts { get; set; }
}

public class Part
{
    public string? text { get; set; }
}
