using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

const string ServiceName = "checkout-api";
var environment = app.Environment.EnvironmentName;

app.MapPost("/checkout", async (HttpContext context, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("UnsafeApi.Checkout");
    using var reader = new StreamReader(context.Request.Body);
    var rawBody = await reader.ReadToEndAsync();

    var request = JsonSerializer.Deserialize<CheckoutRequest>(rawBody) ?? CheckoutRequest.Demo();
    var correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var value)
        ? value.ToString()
        : context.TraceIdentifier;

    logger.LogInformation(
        "{eventName} service={service} environment={environment} correlationId={correlationId} outcome={outcome} amountBucket={amountBucket}",
        "checkout.accepted",
        ServiceName,
        environment,
        correlationId,
        "accepted",
        AmountBucket.From(request.Amount));

    return Results.Accepted($"/checkout/{request.OrderId}", new { request.OrderId, correlationId });
});

app.MapGet("/demo", () => CheckoutRequest.Demo());

app.Run();

public sealed record CheckoutRequest(
    string OrderId,
    string Email,
    string Password,
    string CardNumber,
    string Ssn,
    string UserId,
    string SessionId,
    string CustomerId,
    decimal Amount)
{
    public static CheckoutRequest Demo() => new(
        "ord_10001",
        "jane.buyer@example.com",
        "CorrectHorseBatteryStaple!",
        "4111-1111-1111-1111",
        "123-45-6789",
        "user_2f4fd3e4-7c11-45b9-88f1-7e0134e0f6ad",
        "sess_8b9c7d6e5f4a3b2c1d0e",
        "cust_900000123456",
        149.95m);
}

public static class AmountBucket
{
    public static string From(decimal amount) => amount switch
    {
        < 50m => "low",
        < 500m => "standard",
        _ => "high"
    };
}
