var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Stock.Api is ready!");
    return true;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Stock.Api has committed!");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stock.Api has rolled back!");
});

app.Run();
