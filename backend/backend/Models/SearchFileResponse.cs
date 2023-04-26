namespace backend.Models;

public class SearchFileResponse
{
    public string? FileName { get; set; } = null!;
    public string? FileContent { get; set; } = null!;
    public string? Url { get; set; } = null!;
}