

using System.Text;
using System.Text.Json;
public class AiAnalysisService
{

private readonly HttpClient _httpClient;
private readonly string _apiKey;

public AiAnalysisService(IConfiguration configuration) // iconfiguration is used to access the appsettings.json file and retrieve the API key for OpenAI
    {
        
        _httpClient = new HttpClient(); // this is used to make HTTP requests to the OpenAI API
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty; // the ?? 
    }


public async Task<IncidentReport> AnalyzeEvents(List <CameraEvent> events)
    {
        if (events == null || events.Count == 0)
        {
            return new IncidentReport
            {
                IncidentLevel = IncidentLevel.Low,
                Summary = "No events to analyze.",
                Details = "No events were provided for analysis."
               
            };

        }

            var systemPrompt = BuildSystemPrompt();
            var userPrompt = BuildUserPrompt(events);
            var aiResponse = await CallOpenAI(systemPrompt, userPrompt);
            return ParseResponse(aiResponse, events);

    }

    private string BuildSystemPrompt()
{
    return @"You are an expert security analyst specializing in CCTV surveillance.
Your job is to analyze camera events and generate structured incident reports.

You MUST respond ONLY with valid JSON. No extra text, no markdown, no code blocks.

Use this exact JSON structure:
{
    ""incidentLevel"": ""Low"" or ""Medium"" or ""High"" or ""Critical"",
    ""summary"": ""one line summary"",
    ""details"": ""full analysis of what happened"",
    ""recommendedAction"": ""what security should do now"",
    ""timeRange"": ""start time to end time""
}";
}


private string BuildUserPrompt(List<CameraEvent> events)
{
    var prompt = "Analyze these CCTV camera events and generate an incident report:\n\n";

    foreach (var evt in events)
    {
        prompt += $"- [{evt.Timestamp:yyyy-MM-dd HH:mm:ss}] ";
        prompt += $"Camera {evt.CameraId} in zone '{evt.Zone}': ";
        prompt += $"{evt.EventType} | ";
        prompt += $"Detected: {evt.DetectedObject} | ";
        prompt += $"Severity: {evt.Severity}\n";
    }

    prompt += "\nReturn JSON only. No extra text.";
    return prompt;
}


     private async Task<string> CallOpenAI(string systemPrompt, string userPrompt)
    {
        
        var request = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            max_tokens = 1000,
            temperature = 0.3

        };


        var jsonContent = JsonSerializer.Serialize(request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);

        return responseData
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }



            private IncidentReport ParseResponse(string jsonResponse, List<CameraEvent> events)
{
    var cleaned = jsonResponse.Trim();

    if (cleaned.Contains("```"))
    {
        var start = cleaned.IndexOf("{");
        var end = cleaned.LastIndexOf("}") + 1;
        if (start >= 0 && end > start)
        {
            cleaned = cleaned.Substring(start, end - start);
        }
    }

    using var document = JsonDocument.Parse(cleaned);
    var root = document.RootElement;

    var levelStr = root.GetProperty("incidentLevel").GetString();
    var level = levelStr switch
    {
        "Critical" => IncidentLevel.Critical,
        "High" => IncidentLevel.High,
        "Medium" => IncidentLevel.Medium,
        _ => IncidentLevel.Low
    };

    return new IncidentReport
    {
        IncidentLevel = level,
        Summary = root.GetProperty("summary").GetString() ?? string.Empty,
        Details = root.GetProperty("details").GetString() ?? string.Empty,
        RecommendedAction = root.GetProperty("recommendedAction").GetString() ?? string.Empty,
        TimeRange = root.GetProperty("timeRange").GetString() ?? string.Empty,
        InvolvedCameras = events.Select(e => e.CameraId).Distinct().ToList(),
        GeneratedAt = DateTime.UtcNow
    };
}
    }

