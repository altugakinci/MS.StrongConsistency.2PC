using Coordinator.Models.EntityFramework;
using Coordinator.Services.Concrete;
using Coordinator.Services.Contract;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient("OrderAPI", client => client.BaseAddress = new("https://localhost:7157"));
builder.Services.AddHttpClient("StockAPI", client => client.BaseAddress = new("https://localhost:7076"));
builder.Services.AddHttpClient("PaymentAPI", client => client.BaseAddress = new("https://localhost:7276"));

builder.Services.AddTransient<ITransactionService, TransactionService>();

var app = builder.Build();

app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    // Phase 1 - Preparing

    var transactionId = await transactionService.CreateTransactionAsync();

    await transactionService.SetServiceReadyStatesAsync(transactionId);

    bool readyState = await transactionService.CheckServiceReadyStatesAsync(transactionId);

    if (readyState)
    {
        // Phase 2 - Committing

        await transactionService.CommitAsync(transactionId);

        bool transactionState = await transactionService.CheckServiceTransactionStatesAsync(transactionId);

        // Rollback when fail.

        if (!transactionState)
            await transactionService.RollbackAsync(transactionId);
    }
    else
        await transactionService.RollbackAsync(transactionId);
});

app.Run();
