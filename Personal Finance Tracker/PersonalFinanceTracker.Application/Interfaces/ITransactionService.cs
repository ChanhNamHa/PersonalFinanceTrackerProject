using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDTO> CreateTransactionAsync(Guid userId, CreateTransactionRequest request);
        Task<IEnumerable<TransactionDTO>> GetUserTransactionsAsync(Guid userId);
        Task<bool> DeleteTransactionAsync(Guid userId, Guid transactionId);
        Task<TransactionDTO?> GetTransactionByIdAsync(Guid userId, Guid transactionId);
    }
}