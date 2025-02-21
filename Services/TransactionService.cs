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
    public class TransactionService(AppDbContext context, IWalletService walletService) : ITransactionService
    {
        public async Task<Transaction?> GetAsync(Guid transactionId)
        {
            return await context.Transactions.FindAsync(transactionId);
        }

        public async Task<ICollection<Transaction>> GetAllAsync(Guid profileId)
        {
            return await context.Transactions.Where(t => t.Wallet.ProfileId == profileId).ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId)
        {
            return await context.Transactions
                .Where(t => t.CreatedAt > start && t.CreatedAt < end && t.Wallet.ProfileId == profileId)
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
                WalletId = wallet.Id
            };

            context.Transactions.Add(newTransaction);
            await context.SaveChangesAsync();
            return newTransaction;
        }

        public async Task<Transaction> PutAsync(Transaction transaction)
        {
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