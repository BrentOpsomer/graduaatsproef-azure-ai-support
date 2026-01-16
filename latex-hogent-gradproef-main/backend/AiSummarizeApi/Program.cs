using System.Text;
using MsgReader.Outlook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var app = builder.Build();
app.UseCors();

var endpoint = builder.Configuration["AzureOpenAI:Endpoint"];
var deployment = builder.Configuration["AzureOpenAI:Deployment"];
var apiKey = builder.Configuration["AzureOpenAI:ApiKey"];

if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(deployment))
{
    app.MapGet("/", () => Results.Problem("AzureOpenAI configuratie ontbreekt."));
    app.Run();
    return;
}

var openAi = new OpenAiClient(endpoint, deployment, apiKey);
var msgReader = new MsgReaderService();

app.MapPost(
    "/api/summarize",
    async (HttpClient http, HttpRequest request) =>
    {
        var emailText = await InputReader.ReadAsync(request, msgReader);
        if (string.IsNullOrWhiteSpace(emailText))
            return Results.BadRequest("Geen tekst om samen te vatten.");

        emailText = MailCleaner.Clean(emailText);

        var summary = await openAi.SummarizeAsync(http, emailText);
        return Results.Ok(new { summary, originalText = emailText });
    }
);

app.Run();
