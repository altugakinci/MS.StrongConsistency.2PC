var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Order.Api is ready!");
    return false;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Order.Api has committed!");
    return false;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Order.Api has rolled back!");
});

app.Run();
