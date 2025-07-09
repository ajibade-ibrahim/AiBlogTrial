#pragma warning disable SKEXP0012 // Type is for evaluation purposes only

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using AiBlogTrial.Agents;
using AiBlogTrial.Orchestration;

var builder = Host.CreateApplicationBuilder(args);

// Configure Semantic Kernel
builder.Services.AddScoped<Kernel>(sp =>
{
    var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? 
        throw new Exception("OpenAI API key not found");

    var builder = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(
            modelId: "gpt-4-turbo-preview",
            apiKey: openAIKey)
        .AddOpenAITextToImage(openAIKey);

    return builder.Build();
});

// Register agents
builder.Services.AddScoped<IOutlineAgent, OutlineAgent>();
builder.Services.AddScoped<IContentGenerationAgent, ContentGenerationAgent>();
builder.Services.AddScoped<IEditingAgent, EditingAgent>();
builder.Services.AddScoped<ISeoOptimizationAgent, SeoOptimizationAgent>();
builder.Services.AddScoped<IVisualSuggestionsAgent, VisualSuggestionsAgent>();
builder.Services.AddScoped<ICtaEngagementAgent, CtaEngagementAgent>();
builder.Services.AddScoped<IImageGenerationAgent, ImageGenerationAgent>();

// Register orchestrator
builder.Services.AddScoped<BlogPostOrchestrator>();

var host = builder.Build();

// Example usage
using (var scope = host.Services.CreateScope())
{
    var orchestrator = scope.ServiceProvider.GetRequiredService<BlogPostOrchestrator>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var blogPost = await orchestrator.GenerateBlogPostAsync(
            topic: "Juninho Pernambucano, the Greatest Free Kicker of All Time",
            targetAudience: "Football(Soccer) enthusiasts",
            targetKeywords: new List<string> { "football", "free kick", "free kick", "juninho pernambucano" },
            ctaGoal: "Subscribe to our newsletter for more football insights"
        );

        // Output the generated blog post
        Console.WriteLine($"Generated Blog Post:\n");
        Console.WriteLine($"Title: {blogPost.Title}\n");
        Console.WriteLine($"Meta Description: {blogPost.MetaDescription}\n");
        Console.WriteLine($"Introduction:\n{blogPost.Introduction}\n");
        
        foreach (var section in blogPost.MainSections)
        {
            Console.WriteLine($"## {section.Title}\n");
            Console.WriteLine($"{section.Content}\n");
        }

        Console.WriteLine($"Conclusion:\n{blogPost.Conclusion}\n");
        Console.WriteLine($"Call to Action:\n{blogPost.CallToAction}\n");

        Console.WriteLine("Visual Suggestions:");
        foreach (var suggestion in blogPost.MediaSuggestions)
        {
            Console.WriteLine($"- {suggestion.Description}");
            if (suggestion.GeneratedImageUrl != null)
            {
                Console.WriteLine($"  Image URL: {suggestion.GeneratedImageUrl}");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error generating blog post");
        Console.WriteLine($"Error: {ex.Message}");
    }
}

#pragma warning restore SKEXP0012
