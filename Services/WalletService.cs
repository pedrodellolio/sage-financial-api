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
    public class WalletService(AppDbContext context) : IWalletService
    {
        public async Task<Wallet?> GetAsync(Guid walletId)
        {
            return await context.Wallets.FindAsync(walletId);
        }

        public async Task<ICollection<Wallet>> GetAllAsync(Guid profileId)
        {
            return await context.Wallets.Where(w => w.ProfileId == profileId).ToListAsync();
        }

        public async Task<Wallet?> GetByMonthAndYearAsync(int month, int year, Guid profileId)
        {
            return await context.Wallets.FirstOrDefaultAsync(w => w.Month == month && w.Year == year);
        }

        public async Task<Wallet> PostAsync(int month, int year, Guid profileId)
        {
            var newWallet = new Wallet
            {
                Month = month,
                Year = year,
                ProfileId = profileId
            };

            context.Wallets.Add(newWallet);
            await context.SaveChangesAsync();
            return newWallet;
        }


        public async Task<Wallet> CreateOrUpdateAsync(TransactionDto request, Guid profileId)
        {
            var month = request.OccurredAt.Month;
            var year = request.OccurredAt.Year;

            var existingWallet = await GetByMonthAndYearAsync(month, year, profileId);
            if (existingWallet != null)
            {
                var isExpense = request.Type == TransactionType.EXPENSE;
                if (isExpense)
                    existingWallet.ExpensesBrl -= request.ValueBrl;
                else
                    existingWallet.ExpensesBrl += request.ValueBrl;

                existingWallet.ExpensesBrl = Math.Abs(existingWallet.ExpensesBrl);
                return await PutAsync(existingWallet);
            }

            return await PostAsync(month, year, profileId);
        }

        public async Task<Wallet> PutAsync(Wallet wallet)
        {
            context.Wallets.Update(wallet);
            await context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet?> IncrementAsync(Transaction transaction, decimal oldValue)
        {
            var existingWallet = await GetAsync(transaction.WalletId);
            if (existingWallet == null)
                return null;

            var diff = transaction.ValueBrl - oldValue;
            if (transaction.Type == TransactionType.EXPENSE)
                existingWallet.ExpensesBrl += diff;
            else
                existingWallet.IncomesBrl += diff;

            return await PutAsync(existingWallet);
        }

        public async Task<bool> DeleteAsync(Wallet wallet)
        {
            context.Wallets.Remove(wallet);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}