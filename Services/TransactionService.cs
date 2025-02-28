using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class TransactionService(AppDbContext context, IWalletService walletService, ILabelService labelService) : ITransactionService
    {
        public async Task<Transaction?> GetAsync(Guid transactionId)
        {
            return await context.Transactions.FindAsync(transactionId);
        }

        public async Task<ICollection<Transaction>> GetAllAsync(Guid profileId)
        {
            return await context.Transactions.Where(t => t.Wallet.ProfileId == profileId).OrderByDescending(t => t.OccurredAt).ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetByMonthAndYearAsync(int month, int year, Guid profileId)
        {
            return await context.Transactions
                .Where(t => t.CreatedAt.Month == month && t.CreatedAt.Year == year && t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null)
        {
            return await context.Transactions
                .Where(t =>
                    t.CreatedAt > start &&
                    t.CreatedAt < end &&
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
            };

            var labels = new List<Label>();
            foreach (var labelId in request.Labels)
            {
                var label = await labelService.GetAsync(labelId);
                if (label != null)
                    labels.Add(label);
            }

            newTransaction.Labels = labels;
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