using AiBlogTrial.Agents;
using AiBlogTrial.Models;
using Microsoft.Extensions.Logging;

namespace AiBlogTrial.Orchestration;

public class BlogPostOrchestrator
{
    private readonly IOutlineAgent _outlineAgent;
    private readonly IContentGenerationAgent _contentAgent;
    private readonly IEditingAgent _editingAgent;
    private readonly ISeoOptimizationAgent _seoAgent;
    private readonly IVisualSuggestionsAgent _visualAgent;
    private readonly ICtaEngagementAgent _ctaAgent;
    private readonly IImageGenerationAgent _imageAgent;
    private readonly ILogger<BlogPostOrchestrator> _logger;

    public BlogPostOrchestrator(
        IOutlineAgent outlineAgent,
        IContentGenerationAgent contentAgent,
        IEditingAgent editingAgent,
        ISeoOptimizationAgent seoAgent,
        IVisualSuggestionsAgent visualAgent,
        ICtaEngagementAgent ctaAgent,
        IImageGenerationAgent imageAgent,
        ILogger<BlogPostOrchestrator> logger)
    {
        _outlineAgent = outlineAgent;
        _contentAgent = contentAgent;
        _editingAgent = editingAgent;
        _seoAgent = seoAgent;
        _visualAgent = visualAgent;
        _ctaAgent = ctaAgent;
        _imageAgent = imageAgent;
        _logger = logger;
    }

    public async Task<BlogPost> GenerateBlogPostAsync(string topic, string targetAudience, List<string> targetKeywords, string ctaGoal)
    {
        try
        {
            _logger.LogInformation("Starting blog post generation for topic: {Topic}", topic);

            // Step 1: Generate Outline
            var blogPost = await _outlineAgent.CreateOutlineAsync(topic, targetAudience);
            _logger.LogInformation("Outline generated successfully");

            // Step 2: Generate Content
            blogPost = await _contentAgent.GenerateContentAsync(blogPost);
            _logger.LogInformation("Content generated successfully");

            // Step 3: Parallel Processing - SEO and Visual Suggestions
            var seoTask = _seoAgent.OptimizeForSeoAsync(blogPost, targetKeywords);
            var visualTask = _visualAgent.GenerateVisualSuggestionsAsync(blogPost);

            await Task.WhenAll(seoTask, visualTask);
            
            blogPost = await seoTask;
            var visualSuggestions = await visualTask;
            _logger.LogInformation("SEO optimization and visual suggestions completed");

            // Step 4: Generate Images
            foreach (var suggestion in visualSuggestions)
            {
                try
                {
                    suggestion.GeneratedImageUrl = await _imageAgent.GenerateImageAsync(
                        suggestion.Description,
                        "professional, blog-style");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating image for suggestion: {Description}", suggestion.Description);
                }
            }

            blogPost.MediaSuggestions = visualSuggestions;
            _logger.LogInformation("Image generation completed");

            // Step 5: Add Call-to-Action
            blogPost.CallToAction = await _ctaAgent.GenerateCallToActionAsync(blogPost, ctaGoal);
            _logger.LogInformation("Call-to-action generated successfully");

            // Step 6: Final Editing Pass
            blogPost = await _editingAgent.EditAndProofreadAsync(blogPost);
            _logger.LogInformation("Final editing pass completed");

            return blogPost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating blog post for topic: {Topic}", topic);
            throw;
        }
    }
}