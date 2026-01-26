using System.Text.RegularExpressions;
using MimeKit;

class EmlReaderService
{
    public async Task<string> ReadAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        try
        {
            var msg = MimeMessage.Load(ms);

            if (!string.IsNullOrWhiteSpace(msg.TextBody))
                return Clean(msg.TextBody);

            if (!string.IsNullOrWhiteSpace(msg.HtmlBody))
                return Clean(StripHtml(msg.HtmlBody));

            return "";
        }
        catch
        {
            return "";
        }
    }

    private static string StripHtml(string html) => Regex.Replace(html ?? "", "<.*?>", " ");

    private static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        input = Regex.Replace(input, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
        return string.Join("\n", input.Split('\n').Select(s => s.Trim()));
    }
}
