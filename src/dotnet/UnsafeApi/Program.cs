using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.MapPost("/checkout", async (HttpContext context, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("UnsafeApi.Checkout");
    using var reader = new StreamReader(context.Request.Body);
    var rawBody = await reader.ReadToEndAsync();

    var request = JsonSerializer.Deserialize<CheckoutRequest>(rawBody) ?? CheckoutRequest.Demo();
    var bearerToken = context.Request.Headers.Authorization.ToString();
    var jwtLikeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.demo.signature";

    logger.LogInformation("Raw checkout body: {rawBody}", rawBody);
    logger.LogInformation("Customer login email={email} password={password}", request.Email, request.Password);
    logger.LogWarning("Authorization header {authorization} fallbackToken {token}", bearerToken, jwtLikeToken);
    logger.LogInformation("Payment card {creditCard} ssn {ssn}", request.CardNumber, request.Ssn);
    logger.LogInformation("Checkout object dump {@payload}", request);
    logger.LogInformation("User activity userId={userId} sessionId={sessionId} customerId={customerId} requestId={requestId}",
        request.UserId, request.SessionId, request.CustomerId, context.TraceIdentifier);

    return Results.Accepted($"/checkout/{request.OrderId}", new { request.OrderId });
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
