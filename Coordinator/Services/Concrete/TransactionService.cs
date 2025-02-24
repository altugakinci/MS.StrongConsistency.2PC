using Coordinator.Enums;
using Coordinator.Models;
using Coordinator.Models.EntityFramework;
using Coordinator.Services.Contract;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services.Concrete;

public class TransactionService(
    IHttpClientFactory _httpClientFactory,
    AppDbContext _context) 
    : ITransactionService
{
    HttpClient _orderHttpClient = _httpClientFactory.CreateClient("OrderAPI");
    HttpClient _stockHttpClient = _httpClientFactory.CreateClient("StockAPI");
    HttpClient _paymentHttpClient = _httpClientFactory.CreateClient("PaymentAPI");

    public async Task<Guid> CreateTransactionAsync()
    {
        Guid transactionId = Guid.NewGuid();
        var nodes = await _context.Nodes.ToListAsync();

        foreach (var node in nodes)
        {
            var nodeState = new NodeState()
            {
                TransactionId = transactionId,
                ReadyState = ReadyStates.Pending,
                TransactionState = TransactionStates.Pending
            };

            node.NodeStates = new List<NodeState>() { nodeState };
        }

        await _context.SaveChangesAsync();
        return transactionId;
    }

    public async Task SetServiceReadyStatesAsync(Guid transactionId)
    {
        var nodes = await _context.NodeStates
            .Include(ns => ns.Node)
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync();

        foreach (var nodeState in nodes)
        {
            try
            {
                var response = await (nodeState.Node.Name switch
                {
                    "Order.Api" => _orderHttpClient.GetAsync("ready"),
                    "Stock.Api" => _stockHttpClient.GetAsync("ready"),
                    "Payment.Api" => _paymentHttpClient.GetAsync("ready")
                });

                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                nodeState.ReadyState = result ? ReadyStates.Ready : ReadyStates.Unready;
            }
            catch (Exception)
            {
                nodeState.ReadyState = ReadyStates.Unready;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckServiceReadyStatesAsync(Guid transactionId)
    {
        var nodeStates = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync();

        foreach (var nodeState in nodeStates)
            if (nodeState.ReadyState == ReadyStates.Unready)
                return false;

        return true;
    }

    public async Task CommitAsync(Guid transactionId)
    {
        var nodeStates = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .Include(ns => ns.Node)
            .ToListAsync();

        foreach (var nodeState in nodeStates)
        {
            try
            {
                var response = await (nodeState.Node.Name switch
                {
                    "Order.Api" => _orderHttpClient.GetAsync("commit"),
                    "Stock.Api" => _stockHttpClient.GetAsync("commit"),
                    "Payment.Api" => _paymentHttpClient.GetAsync("commit")
                });

                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                nodeState.TransactionState = result ? TransactionStates.Done : TransactionStates.Abort;
            }
            catch (Exception)
            {
                nodeState.TransactionState = TransactionStates.Abort;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckServiceTransactionStatesAsync(Guid transactionId)
    {
        var nodeStates = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .Include(ns => ns.Node)
            .ToListAsync();

        foreach (var nodeState in nodeStates)
            if (nodeState.TransactionState == TransactionStates.Abort)
                return false;

        return true;
    }

    public async Task RollbackAsync(Guid transactionId)
    {
        var nodeStates = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .Include(ns => ns.Node)
            .ToListAsync();

        foreach (var nodeState in nodeStates)
        {
            try
            {
                if(nodeState.TransactionState == TransactionStates.Done)
                {
                    var response = await (nodeState.Node.Name switch
                    {
                        "Order.Api" => _orderHttpClient.GetAsync("rollback"),
                        "Stock.Api" => _stockHttpClient.GetAsync("rollback"),
                        "Payment.Api" => _paymentHttpClient.GetAsync("rollback")
                    });
                }

                nodeState.TransactionState = TransactionStates.Abort;
            }
            catch (Exception)
            {
                nodeState.TransactionState = TransactionStates.Abort;
            }
        }

        await _context.SaveChangesAsync();
    }
}
