using System.Text.RegularExpressions;

static class MailCleaner
{
    public static string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        text = Regex.Replace(
            text,
            @"(Mvg|Met vriendelijke groet|Met vriendelijke groeten|Regards)[\s\S]{0,600}?(From:|Sent:)",
            "$1\n[HANDTEKENING VERWIJDERD]\n$2",
            RegexOptions.IgnoreCase
        );

        text = Regex.Replace(text, @"\[cid:.*?\]", "", RegexOptions.IgnoreCase);

        text = Regex.Replace(text, @"[\w\.-]+@[\w\.-]+\.\w+", "[EMAIL]");

        text = Regex.Replace(
            text,
            @"\+?\d{2,3}[\s\-]?\(?\d{1,3}\)?[\s\-]?\d{2,3}[\s\-]?\d{2,4}[\s\-]?\d{2,4}",
            "[TELEFOON]"
        );

        text = Regex.Replace(text, @"(?m)^[A-Z][a-z]+ [A-Z][a-z]+$", "[NAAM]");

        text = Regex.Replace(text, @"https?:\/\/\S+", "");
        text = Regex.Replace(text, @"www\.\S+", "");
        text = Regex.Replace(text, @"mailto:\S+", "");

        text = Regex.Replace(text, @"\r\n", "\n");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        text = Regex.Replace(text, @"\s{2,}", " ");

        return text.Trim();
    }
}
