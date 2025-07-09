namespace AiBlogTrial.Models;

public class BlogPost
{
    public string Title { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public List<BlogSection> MainSections { get; set; } = new();
    public string Conclusion { get; set; } = string.Empty;
    public string CallToAction { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public string MetaDescription { get; set; } = string.Empty;
    public List<MediaSuggestion> MediaSuggestions { get; set; } = new();
}

public class BlogSection
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class MediaSuggestion
{
    public string Description { get; set; } = string.Empty;
    public string PlacementSuggestion { get; set; } = string.Empty;
    public string? GeneratedImageUrl { get; set; }
}