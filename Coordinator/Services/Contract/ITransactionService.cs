namespace Coordinator.Services.Contract;

public interface ITransactionService
{
    Task<Guid> CreateTransactionAsync();
    Task SetServiceReadyStatesAsync(Guid transactionId);
    Task<bool> CheckServiceReadyStatesAsync(Guid transactionId);
    Task CommitAsync(Guid transactionId);
    Task<bool> CheckServiceTransactionStatesAsync(Guid transactionId);
    Task RollbackAsync(Guid transactionId);
}
