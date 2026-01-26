using System.Text.Json;

static class InputReader
{
    public static async Task<string> ReadAsync(
        HttpRequest request,
        MsgReaderService msgReader,
        EmlReaderService emlReader
    )
    {
        string text = "";

        if (request.HasFormContentType)
        {
            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null)
                return "";

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ext == ".msg")
            {
                text = await msgReader.ReadAsync(file);
            }
            else if (ext == ".eml")
            {
                text = await emlReader.ReadAsync(file);
            }
            else
            {
                using var sr = new StreamReader(file.OpenReadStream());
                text = await sr.ReadToEndAsync();
            }

            return text ?? "";
        }

        using (var reader = new StreamReader(request.Body))
        {
            var body = await reader.ReadToEndAsync();
            var data = JsonSerializer.Deserialize<SummarizeRequest>(body);
            text = data?.EmailText ?? "";
        }

        return text;
    }
}
