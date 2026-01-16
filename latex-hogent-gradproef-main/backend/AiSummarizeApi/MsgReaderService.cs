using System.Text.RegularExpressions;
using MsgReader.Outlook;

class MsgReaderService
{
    public async Task<string> ReadAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        try
        {
            using var msg = new Storage.Message(ms);

            if (!string.IsNullOrWhiteSpace(msg.BodyText))
                return Clean(msg.BodyText);

            if (!string.IsNullOrWhiteSpace(msg.BodyHtml))
                return Clean(StripHtml(msg.BodyHtml));

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
