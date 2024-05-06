namespace CoolBlazor.Infrastructure.Utils.Markdown;

public interface IMudMarkdownClipboardService
{
    ValueTask CopyToClipboardAsync(string text);
}