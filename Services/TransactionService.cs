using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class TransactionService(AppDbContext context, IWalletService walletService) : ITransactionService
    {
        public async Task<Transaction?> GetAsync(Guid transactionId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<ICollection<Transaction>> GetAllByWalletAsync(Guid walletId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.WalletId == walletId)
                .ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetAllAsync(Guid profileId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetAllByMonthAndYearLabelAsync(int month, int year, Guid labelId, Guid profileId, TransactionType? type = null)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Include(t => t.Wallet)
                .Where(t => t.Label != null &&
                            t.Label.Id == labelId &&
                            t.OccurredAt.Month == month &&
                            t.OccurredAt.Year == year &&
                            t.Wallet.ProfileId == profileId &&
                            (type == null || t.Type == type))
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetAllByMonthAndYearAsync(int month, int year, Guid profileId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.OccurredAt.Month == month &&
                            t.OccurredAt.Year == year &&
                            t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null)
        {
            return await context.Transactions
                .Where(t =>
                    t.OccurredAt > start &&
                    t.OccurredAt < end &&
                    t.Wallet.ProfileId == profileId &&
                    (type == null || t.Type == type)
                )
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }


        public async Task<Transaction> PostAsync(TransactionDto request, Guid profileId)
        {
            Wallet wallet = await walletService.CreateOrUpdateAsync(request, profileId);
            var newTransaction = new Transaction
            {
                Title = request.Title,
                Type = request.Type,
                ValueBrl = request.ValueBrl,
                OccurredAt = request.OccurredAt,
                WalletId = wallet.Id,
                LabelId = request.Label?.Id
            };

            context.Transactions.Add(newTransaction);
            await context.SaveChangesAsync();
            return newTransaction;
        }

        public async Task<Transaction> PutAsync(Transaction transaction, decimal oldValue)
        {
            await walletService.IncrementAsync(transaction, oldValue);
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteAsync(Transaction transaction)
        {
            context.Transactions.Remove(transaction);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}