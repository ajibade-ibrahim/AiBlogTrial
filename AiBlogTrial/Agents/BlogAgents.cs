using Microsoft.SemanticKernel;
using AiBlogTrial.Models;
using System.Text.Json; // <-- Add this

namespace AiBlogTrial.Agents;

public class OutlineAgent : IOutlineAgent
{
    private readonly Kernel _kernel;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public OutlineAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<BlogPost> CreateOutlineAsync(string topic, string targetAudience)
    {
        var outlineFunction = _kernel.CreateFunctionFromPrompt("""
            Create a detailed blog post outline for the topic: {{$topic}}
            Target audience: {{$targetAudience}}

            Generate a structured outline with:
            1. An engaging title
            2. Introduction section
            3. 3-5 main body sections with subtopics
            4. Conclusion section
            5. Call-to-action suggestion

            Format the response as valid JSON (not as a code block) matching this structure:
            {
              "title": "string",
              "introduction": "string",
              "mainSections": [
                {
                  "title": "string",
                  "content": "string"
                }
              ],
              "conclusion": "string",
              "callToAction": "string"
            }
            """);

        var result = await _kernel.InvokeAsync(outlineFunction, new() {
            { "topic", topic },
            { "targetAudience", targetAudience }
        });

        var cleanJson = ResponseCleaner.CleanJsonResponse(result.ToString());
        var post = JsonSerializer.Deserialize<BlogPost>(cleanJson, _jsonOptions);
        return post ?? new BlogPost();
    }
}

public class ContentGenerationAgent : IContentGenerationAgent
{
    private readonly Kernel _kernel;

    public ContentGenerationAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<BlogPost> GenerateContentAsync(BlogPost outline)
    {
        var contentFunction = _kernel.CreateFunctionFromPrompt("""
            Generate detailed content for the blog post outline below:
            Title: {{$title}}
            Introduction topic: {{$intro}}
            Section title: {{$sectionTitle}}

            For this section, generate engaging, informative content that flows naturally.
            Maintain a consistent tone and style.
            Include relevant examples and explanations.
            Keep technical accuracy and readability in mind.

            Return the content as plain text (not JSON formatted).
            """);

        foreach (var section in outline.MainSections)
        {
            var result = await _kernel.InvokeAsync(contentFunction, new() {
                { "title", outline.Title },
                { "intro", outline.Introduction },
                { "sectionTitle", section.Title }
            });

            section.Content = ResponseCleaner.CleanTextContent(result.ToString());
        }

        return outline;
    }
}

public class EditingAgent : IEditingAgent
{
    private readonly Kernel _kernel;

    public EditingAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<BlogPost> EditAndProofreadAsync(BlogPost blogPost)
    {
        var editFunction = _kernel.CreateFunctionFromPrompt("""
            Proofread and edit the following blog post content:
            {{$content}}

            Focus on:
            1. Grammar and spelling
            2. Clarity and coherence
            3. Style consistency
            4. Technical accuracy
            5. Flow between sections

            Return the improved content as plain text (not JSON formatted).
            """);

        foreach (var section in blogPost.MainSections)
        {
            var result = await _kernel.InvokeAsync(editFunction, new() {
                { "content", section.Content }
            });
            section.Content = ResponseCleaner.CleanTextContent(result.ToString());
        }

        return blogPost;
    }
}

public class SeoOptimizationAgent : ISeoOptimizationAgent
{
    private readonly Kernel _kernel;

    public SeoOptimizationAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<BlogPost> OptimizeForSeoAsync(BlogPost blogPost, List<string> targetKeywords)
    {
        var seoFunction = _kernel.CreateFunctionFromPrompt("""
            Optimize the following content for SEO using these keywords: {{$keywords}}

            Content: {{$content}}

            1. Naturally integrate keywords
            2. Optimize headings
            3. Improve meta description
            4. Enhance readability
            5. Maintain natural flow

            Return the optimized content as plain text (not JSON formatted).
            Also provide a meta description in a new line starting with "META_DESCRIPTION:"
            """);

        foreach (var section in blogPost.MainSections)
        {
            var result = await _kernel.InvokeAsync(seoFunction, new() {
                { "keywords", string.Join(", ", targetKeywords) },
                { "content", section.Content }
            });

            var response = result.ToString().Split("\nMETA_DESCRIPTION:", 2);
            section.Content = ResponseCleaner.CleanTextContent(response[0]);
            
            if (response.Length > 1 && string.IsNullOrEmpty(blogPost.MetaDescription))
            {
                blogPost.MetaDescription = ResponseCleaner.CleanTextContent(response[1]);
            }
        }

        blogPost.Keywords = targetKeywords;
        return blogPost;
    }
}

public class VisualSuggestionsAgent : IVisualSuggestionsAgent
{
    private readonly Kernel _kernel;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public VisualSuggestionsAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<List<MediaSuggestion>> GenerateVisualSuggestionsAsync(BlogPost blogPost)
    {
        var visualFunction = _kernel.CreateFunctionFromPrompt("""
            Analyze the blog post content and suggest relevant visuals:
            Title: {{$title}}
            Content: {{$content}}

            Generate suggestions for:
            1. Featured image
            2. Section-specific illustrations
            3. Infographics or diagrams
            4. Placement recommendations

            Format the response as valid JSON (not as a code block) matching this structure:
            [
              {
                "description": "string",
                "placementSuggestion": "string"
              }
            ]
            """);

        var result = await _kernel.InvokeAsync(visualFunction, new() {
            { "title", blogPost.Title },
            { "content", string.Join("\n", blogPost.MainSections.Select(s => s.Content)) }
        });

        var cleanJson = ResponseCleaner.CleanJsonResponse(result.ToString());
        return JsonSerializer.Deserialize<List<MediaSuggestion>>(cleanJson, _jsonOptions)
            ?? new List<MediaSuggestion>();
    }
}

public class CtaEngagementAgent : ICtaEngagementAgent
{
    private readonly Kernel _kernel;

    public CtaEngagementAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> GenerateCallToActionAsync(BlogPost blogPost, string goal)
    {
        var ctaFunction = _kernel.CreateFunctionFromPrompt("""
            Generate an engaging call-to-action for the blog post:
            Title: {{$title}}
            Goal: {{$goal}}

            Create a compelling CTA that:
            1. Aligns with the post content
            2. Drives the desired action
            3. Uses persuasive language
            4. Maintains the post's tone

            Return the CTA as plain text (not JSON formatted).
            """);

        var result = await _kernel.InvokeAsync(ctaFunction, new() {
            { "title", blogPost.Title },
            { "goal", goal }
        });

        return ResponseCleaner.CleanTextContent(result.ToString());
    }
}

public class ImageGenerationAgent : IImageGenerationAgent
{
    private readonly Kernel _kernel;

    public ImageGenerationAgent(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> GenerateImageAsync(string description, string style)
    {
        var imageFunction = _kernel.CreateFunctionFromPrompt("""
            Generate an image matching this description: {{$description}}
            Style: {{$style}}

            Return only the image URL.
            """);

        var result = await _kernel.InvokeAsync(imageFunction, new() {
            { "description", description },
            { "style", style }
        });

        return ResponseCleaner.CleanTextContent(result.ToString());
    }
}