using System.Text;
using System.Text.Json;

namespace Services;

public class GeminiService
{
    private readonly string _url;

    public GeminiService(string apiKey)
    {
        _url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
    }

    public async Task<string> Call(string prompt)
    {
        using var client = new HttpClient();
        var requestBody = new
        {
            contents = new[]
            {
                new {
                    parts = new[] {
                        new { text = prompt }
                    }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(_url, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(jsonResponse);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";
    }
}
