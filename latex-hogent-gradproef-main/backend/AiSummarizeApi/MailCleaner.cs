using System.Text.RegularExpressions;

static class MailCleaner
{
    public static string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        // 1. Alleen echte links weg
        text = Regex.Replace(text, @"https?:\/\/\S+", "", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"www\.\S+", "", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"mailto:\S+", "", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"tel:\S+", "", RegexOptions.IgnoreCase);

        // 2. Whitespace netjes maken
        text = Regex.Replace(text, @"\r\n", "\n");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        text = Regex.Replace(text, @"\s{2,}", " ");

        return text.Trim();
    }
}
