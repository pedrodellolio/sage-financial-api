using Hangfire;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Models;
using System.Transactions;

namespace SageFinancialAPI.Services
{
    public class TransactionService(AppDbContext context, IWalletService walletService) : ITransactionService
    {
        #region Query Methods

        public async Task<Entities.Transaction?> GetAsync(Guid transactionId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<ICollection<Entities.Transaction>> GetAllByWalletAsync(Guid walletId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.WalletId == walletId)
                .ToListAsync();
        }

        public async Task<ICollection<Entities.Transaction>> GetAllInstallmentsAsync(Guid transactionId)
        {
            return await context.Transactions
                .Where(t => t.ParentTransactionId == transactionId)
                .Include(t => t.Wallet)
                .ToListAsync();
        }

        public async Task<ICollection<Entities.Transaction>> GetAllAsync(Guid profileId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        public async Task<ICollection<Entities.Transaction>> GetAllByMonthAndYearLabelAsync(int month, int year, Guid labelId, Guid profileId, TransactionType? type = null)
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

        public async Task<ICollection<Entities.Transaction>> GetAllByMonthAndYearAsync(int month, int year, Guid profileId)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t => t.OccurredAt.Month == month &&
                            t.OccurredAt.Year == year &&
                            t.Wallet.ProfileId == profileId)
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        public async Task<ICollection<Entities.Transaction>> GetByPeriodAsync(DateTime start, DateTime end, Guid profileId, TransactionType? type = null)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t =>
                    t.OccurredAt > start &&
                    t.OccurredAt < end &&
                    t.Wallet.ProfileId == profileId &&
                    (type == null || t.Type == type))
                .OrderByDescending(t => t.OccurredAt)
                .ToListAsync();
        }

        #endregion

        #region Posting Methods

        public async Task<bool> PostAsync(TransactionDto request, Guid profileId, bool scheduleRecurrence = true)
        {
            return await ExecuteInOptionalScopeAsync(async () =>
            {
                if (request.TotalInstallments > 0)
                {
                    await PostManyAsync(request, profileId);
                }
                else
                {
                    await ProcessSingleTransactionAsync(request, profileId, installment: 1, scheduleRecurrence: scheduleRecurrence);
                    await context.SaveChangesAsync();
                }
            });
        }

        public async Task<bool> PostManyAsync(TransactionDto request, Guid profileId)
        {
            return await ExecuteInOptionalScopeAsync(async () =>
            {
                // Calculate the installment amount.
                request.ValueBrl = CalculateInstallment(request.ValueBrl, request.InterestPercentage, request.TotalInstallments);

                Entities.Transaction originalTransaction = await ProcessSingleTransactionAsync(request, profileId, installment: 1);
                await context.SaveChangesAsync();

                for (int i = 2; i <= request.TotalInstallments; i++)
                {
                    if (originalTransaction.Frequency != null)
                    {
                        request.OccurredAt = GetIncrementedDate(originalTransaction.OccurredAt, i - 1, originalTransaction.Frequency);
                    }

                    await ProcessSingleTransactionAsync(request, profileId, i, originalTransaction.Id);
                }

                await context.SaveChangesAsync();
            });
        }

        public async Task<bool> PutAsync(TransactionDto newTransaction, Entities.Transaction oldTransaction)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var profileId = oldTransaction.Wallet.ProfileId;
                await DeleteAsync(oldTransaction); // Delete original transaction and its installments

                // Adjust new transaction’s value based on the original total.
                newTransaction.ValueBrl = GetTransactionOriginalTotalValue(oldTransaction);
                await PostAsync(newTransaction, profileId);
                await context.SaveChangesAsync();

                transactionScope.Complete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessRecurringTransaction(Guid originalTransactionId)
        {
            var originalTransaction = await GetAsync(originalTransactionId);
            if (originalTransaction == null || originalTransaction.Frequency == null)
                return;

            var newTransaction = new TransactionDto
            {
                Title = originalTransaction.Title,
                ValueBrl = originalTransaction.ValueBrl,
                OccurredAt = GetIncrementedDate(originalTransaction.OccurredAt, originalTransaction.Installment, originalTransaction.Frequency),
                Type = originalTransaction.Type,
                Frequency = originalTransaction.Frequency,
                Label = originalTransaction.Label?.ToDto(),
                ParentTransactionId = originalTransaction.Id
            };

            await PostAsync(newTransaction, originalTransaction.Wallet.ProfileId, scheduleRecurrence: newTransaction.ParentTransactionId != null);
        }

        #endregion

        #region Deletion Methods

        public async Task<bool> DeleteAsync(Entities.Transaction transaction)
        {
            return await ExecuteInOptionalScopeAsync(async () =>
            {
                var installments = await GetAllInstallmentsAsync(transaction.Id);
                RemoveScheduledJobs(transaction.Id);

                context.Transactions.RemoveRange(installments);
                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();

                // After deletion, update the wallets for all affected transactions.
                var allTransactions = new List<Entities.Transaction>(installments) { transaction };
                foreach (var t in allTransactions)
                {
                    await walletService.PatchAsync(t.Wallet);
                }
            });
        }

        #endregion

        #region Recurring Transactions

        private void ScheduleRecurringTransaction(Entities.Transaction transaction)
        {
            try
            {
                string jobId = $"RecurringTransaction-{transaction.ParentTransactionId ?? transaction.Id}";
                int dayOfWeek = ((int)transaction.OccurredAt.DayOfWeek + 6) % 7 + 1;
                string cronExpression = transaction.Frequency switch
                {
                    RecurrenceType.WEEKLY => $"0 0 * * {dayOfWeek}",
                    RecurrenceType.MONTHLY => $"0 0 {transaction.OccurredAt.Day} * *",
                    RecurrenceType.YEARLY => $"0 0 {transaction.OccurredAt.Day} {transaction.OccurredAt.Month} *",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(cronExpression))
                {
                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => ProcessRecurringTransaction(transaction.Id),
                        cronExpression);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void RemoveScheduledJobs(Guid transactionId)
        {
            string jobId = $"RecurringTransaction-{transactionId}";
            RecurringJob.RemoveIfExists(jobId);
        }


        private void ScheduleRecurringIfNeeded(Entities.Transaction transaction)
        {
            if (transaction.Frequency != null)
            {
                ScheduleRecurringTransaction(transaction);
            }
        }

        #endregion

        #region Transaction Processing & Helpers

        private async Task<Entities.Transaction> ProcessSingleTransactionAsync(TransactionDto request, Guid profileId, int installment, Guid? parentId = null, bool scheduleRecurrence = true)
        {
            Wallet wallet = await walletService.CreateOrUpdateAsync(request, profileId);
            Entities.Transaction newTransaction = BuildTransaction(request, wallet, installment, parentId);
            context.Transactions.Add(newTransaction);
            if (scheduleRecurrence)
                ScheduleRecurringIfNeeded(newTransaction);
            return newTransaction;
        }

        private static Entities.Transaction BuildTransaction(TransactionDto request, Wallet wallet, int installment, Guid? parentId = null)
        {
            return new Entities.Transaction
            {
                Title = request.Title,
                Type = request.Type,
                ValueBrl = request.ValueBrl,
                OccurredAt = request.OccurredAt,
                WalletId = wallet.Id,
                LabelId = request.Label?.Id,
                Frequency = request.Frequency,
                InterestPercentage = request.InterestPercentage,
                Installment = installment,
                TotalInstallments = request.TotalInstallments,
                ParentTransactionId = parentId ?? request.ParentTransactionId,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static decimal CalculateInstallment(decimal fullValue, decimal interestRate, int totalInstallments)
        {
            if (totalInstallments == 0)
                return fullValue;
            if (interestRate == 0)
                return fullValue / totalInstallments;
            var total = fullValue * interestRate / (1 - (decimal)Math.Pow((double)(1 + interestRate), -totalInstallments));
            return Math.Round(total, 2);
        }

        private static decimal GetTransactionOriginalTotalValue(Entities.Transaction transaction)
        {
            if (transaction.TotalInstallments == 0)
                return transaction.ValueBrl;
            if (transaction.InterestPercentage == 0)
                return transaction.ValueBrl * transaction.TotalInstallments;
            var total = transaction.ValueBrl * (1 - (decimal)Math.Pow((double)(1 + transaction.InterestPercentage), -transaction.TotalInstallments)) / transaction.InterestPercentage;
            return Math.Round(total, 2);
        }

        private static DateTimeOffset GetIncrementedDate(DateTimeOffset occurredAt, int installment, RecurrenceType? frequency)
        {
            return frequency switch
            {
                RecurrenceType.WEEKLY => occurredAt.AddDays(7 * installment),
                RecurrenceType.BIWEEKLY => occurredAt.AddDays(14 * installment),
                RecurrenceType.MONTHLY => occurredAt.AddMonths(installment),
                RecurrenceType.YEARLY => occurredAt.AddYears(installment),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, "Unsupported interval"),
            };
        }

        /// <summary>
        /// Wraps the execution of an async action in a TransactionScope if none is already present.
        /// </summary>
        /// <param name="action">The async delegate to execute.</param>
        /// <returns>True if execution succeeds; false if an exception is caught (when no ambient transaction exists).</returns>
        private static async Task<bool> ExecuteInOptionalScopeAsync(Func<Task> action)
        {
            if (System.Transactions.Transaction.Current == null)
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    await action();
                    scope.Complete();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                await action();
                return true;
            }
        }

        #endregion
    }
}
