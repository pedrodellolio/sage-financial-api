using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class TransactionService(AppDbContext context, IWalletService walletService, RecurringTransactionService recurringService) : ITransactionService
    {
        public async Task<Transaction?> GetAsync(Guid transactionId)
        {
            var transaction = await context.Transactions
                .Include(t => t.Label)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
            return transaction;
        }

        public async Task<ICollection<Transaction>> GetAllByWalletAsync(Guid walletId)
        {
            var transactions = await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.WalletId == walletId)
                .ToListAsync();

            return transactions;
        }

        public async Task<ICollection<Transaction>> GetAllAsync(Guid profileId)
        {
            var transactions = await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();

            return transactions;
        }

        public async Task<ICollection<Transaction>> GetAllByMonthAndYearLabelAsync(int month, int year, Guid labelId, Guid profileId, TransactionType? type = null)
        {
            var transactions = await context.Transactions
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

            return transactions;
        }

        public async Task<ICollection<Transaction>> GetAllByMonthAndYearAsync(int month, int year, Guid profileId)
        {
            var transactions = await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.OccurredAt.Month == month &&
                            t.OccurredAt.Year == year &&
                            t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();

            return transactions;
        }

        public async Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null)
        {
            var transactions = await context.Transactions
                .Include(t => t.Label)
                .Where(t =>
                    t.OccurredAt > start &&
                    t.OccurredAt < end &&
                    t.Wallet.ProfileId == profileId &&
                    (type == null || t.Type == type)
                )
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();

            return transactions;
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
                LabelId = request.Label?.Id,
                Frequency = request.Frequency
            };

            context.Transactions.Add(newTransaction);
            await context.SaveChangesAsync();

            if (newTransaction.Frequency != null)
            {
                //Criar todas as transações até a data atual e agendar um job para as futuras
                recurringService.ScheduleRecurringTransaction(newTransaction);
            }

            return newTransaction;
        }

        public async Task<Transaction> PutAsync(Transaction transaction, decimal oldValue)
        {
            await walletService.IncrementAsync(transaction, oldValue);
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();

            if (transaction.Frequency != null)
            {
                //Criar todas as transações até a data atual e agendar um job para as futuras
                recurringService.ScheduleRecurringTransaction(transaction);
            }
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