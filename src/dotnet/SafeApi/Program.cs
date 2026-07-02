var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

const string ServiceName = "checkout-api";
var environment = app.Environment.EnvironmentName;

app.MapPost("/checkout", (SafeCheckoutRequest request, HttpContext context, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("SafeApi.Checkout");
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

app.MapGet("/demo", () => new SafeCheckoutRequest("ord_10001", 149.95m));

app.Run();

public sealed record SafeCheckoutRequest(string OrderId, decimal Amount);

public static class AmountBucket
{
    public static string From(decimal amount) => amount switch
    {
        < 50m => "low",
        < 500m => "standard",
        _ => "high"
    };
}
