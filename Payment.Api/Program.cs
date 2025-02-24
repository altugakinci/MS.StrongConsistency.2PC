var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Payment.Api is ready!");
    return true;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Payment.Api has committed!");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Payment.Api has rolled back!");
});

app.Run();
