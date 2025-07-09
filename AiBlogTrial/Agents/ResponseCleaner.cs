using System.Text.RegularExpressions;

namespace AiBlogTrial.Agents;

public static class ResponseCleaner
{
    private static readonly Regex JsonBlockRegex = new(@"```(?:json)?\s*([\s\S]*?)```", RegexOptions.Compiled);
    
    public static string CleanJsonResponse(string response)
    {
        var match = JsonBlockRegex.Match(response);
        return match.Success ? match.Groups[1].Value.Trim() : response.Trim();
    }

    public static string CleanTextContent(string content)
    {
        // Remove any markdown code blocks or response formatting
        var cleaned = JsonBlockRegex.Replace(content, "$1");
        return cleaned.Trim();
    }
}