using AiBlogTrial.Models;

namespace AiBlogTrial.Agents;

public interface IOutlineAgent
{
    Task<BlogPost> CreateOutlineAsync(string topic, string targetAudience);
}

public interface IContentGenerationAgent
{
    Task<BlogPost> GenerateContentAsync(BlogPost outline);
}

public interface IEditingAgent
{
    Task<BlogPost> EditAndProofreadAsync(BlogPost blogPost);
}

public interface ISeoOptimizationAgent
{
    Task<BlogPost> OptimizeForSeoAsync(BlogPost blogPost, List<string> targetKeywords);
}

public interface IVisualSuggestionsAgent
{
    Task<List<MediaSuggestion>> GenerateVisualSuggestionsAsync(BlogPost blogPost);
}

public interface ICtaEngagementAgent
{
    Task<string> GenerateCallToActionAsync(BlogPost blogPost, string goal);
}

public interface IImageGenerationAgent
{
    Task<string> GenerateImageAsync(string description, string style);
}