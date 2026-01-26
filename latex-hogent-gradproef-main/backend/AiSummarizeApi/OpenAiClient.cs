using System.Text;
using System.Text.Json;

class OpenAiClient
{
    private readonly string _endpoint;
    private readonly string _deployment;
    private readonly string _apiKey;

    public OpenAiClient(string endpoint, string deployment, string apiKey)
    {
        _endpoint = endpoint;
        _deployment = deployment;
        _apiKey = apiKey;
    }

    private string Url =>
        $"{_endpoint}/openai/deployments/{_deployment}/chat/completions?api-version=2024-02-15-preview";

    private const string SystemPrompt =
        @"Je bent een support-assistent bij het bedrijf Carfac in Oudenaarde die enkel nederlandse output geeft.

        Je krijgt de inhoud van een e-mail.

        Taak:
        - Analyseer de mail en vat het probleem zo concreet mogelijk samen.
        - Als het probleem of de vraag niet letterlijk wordt gesteld, leid deze af uit de context, maar verzin niets.
        - Gebruik uitsluitend informatie die duidelijk uit de tekst blijkt.

        Geef uitsluitend deze structuur:

        PROBLEEM:
        <beschrijf zo concreet mogelijk wat er fout loopt of waarover de mail gaat>

        URGENTIE:
        <laag | normaal | hoog>
        <baseer je op toon, timing, woorden zoals 'asap', impact, frustratie>

        VRAAG VAN KLANT:
        <beschrijf wat de afzender wil bereiken of verwacht, ook als dit impliciet is>

        Regels:
        - Verzin geen feiten die niet in de mail staan.
        - Maak geen aannames over technische oorzaken die niet genoemd worden.
        - Vat samen zoals een supportmedewerker dat zou doen in een ticket.
        - Negeer begroetingen, handtekeningen en disclaimers.
        - Geef niets anders terug.
        ";

    public async Task<string> SummarizeAsync(HttpClient http, string text)
    {
        var payload = JsonSerializer.Serialize(
            new
            {
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = text },
                },
                max_tokens = 200,
                temperature = 0.2,
            }
        );

        var req = new HttpRequestMessage(HttpMethod.Post, Url);
        req.Headers.Add("api-key", _apiKey);
        req.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var resp = await http.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
            ?? "";
    }
}
