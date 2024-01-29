using System.Text;
using System.Text.Json;

namespace BankKRT.Presentation.Middlewares;

public class CustomValidationErrorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var newBodyStream = new MemoryStream();
        context.Response.Body = newBodyStream;

        await next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        if (context.Response.StatusCode == 400 && !string.IsNullOrWhiteSpace(responseText))
        {
            try
            {
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseText);

                if (responseObject.TryGetProperty("errors", out var errors))
                {
                    var errorsDict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(errors.GetRawText());
                    var customErrors = new Dictionary<string, string[]>();

                    if (errorsDict != null)
                    {
                        foreach (var error in errorsDict)
                        {
                            var key = error.Key.Replace("$.", "");
                            var customMessage = new string[] { $"campo {key} está no formato inválido" };
                            customErrors[key] = customMessage;
                        }
                    }

                    var newResponseText = JsonSerializer.Serialize(new { Errors = customErrors });
                    var newResponseBytes = Encoding.UTF8.GetBytes(newResponseText);

                    context.Response.Body = originalBodyStream;
                    context.Response.ContentType = "application/json";
                    await context.Response.Body.WriteAsync(newResponseBytes);
                    return;
                }
            }
            catch (JsonException)
            {
                // Log ou tratamento de exceção de deserialização
            }
        }

        await newBodyStream.CopyToAsync(originalBodyStream);
    }
}
