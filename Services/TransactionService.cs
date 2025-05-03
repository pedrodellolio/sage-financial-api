using Hangfire;
using Microsoft.EntityFrameworkCore;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Models;
using System;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace SageFinancialAPI.Services
{
    public class TransactionService(AppDbContext context, IWalletService walletService, INotificationService notificationService) : ITransactionService
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

        public async Task<ICollection<Entities.Transaction>> GetAllByMonthAndYearAsync(int month, int year, Guid profileId, string? input = null, TransactionFiltersDto? filters = null)
        {
            var query = context.Transactions
                .Include(t => t.Label)
                .Where(t => t.OccurredAt.Month == month &&
                            t.OccurredAt.Year == year &&
                            t.Wallet.ProfileId == profileId);

            if (!string.IsNullOrEmpty(input))
                query = query.Where(t => t.Title.StartsWith(input.ToUpper()));

            if (filters is not null)
            {
                if (filters.OnlyInstallment)
                    query = query.Where(t => t.TotalInstallments > 0);

                if (filters.OnlyRecurrent)
                    query = query.Where(t => t.Frequency != null);

                if (filters.MinValue.HasValue && filters.MinValue.Value > 0)
                    query = query.Where(t => t.ValueBrl >= filters.MinValue.Value);

                if (filters.MaxValue.HasValue && filters.MaxValue.Value > 0)
                    query = query.Where(t => t.ValueBrl <= filters.MaxValue.Value);

                if (filters.Type.HasValue)
                    query = query.Where(t => t.Type == filters.Type.Value);
                //if (filters.LabelIds.Count != 0)
                //    query = query.Where(t => t.LabelId.HasValue && filters.LabelIds.Contains(t.LabelId.Value));
            }

            return await query
                    .OrderByDescending(t => t.OccurredAt)
                    .ToListAsync();
        }

        public async Task<ICollection<Entities.Transaction>> GetByPeriodAsync(DateTime start, DateTime end, bool onlyRecurrentOrInstallment, Guid profileId, TransactionType? type = null)
        {
            return await context.Transactions
                .Include(t => t.Label)
                .Where(t =>
                    t.OccurredAt >= start &&
                    t.OccurredAt <= end &&
                    t.Wallet.ProfileId == profileId &&
                    (!onlyRecurrentOrInstallment || (t.Frequency != null || t.TotalInstallments > 0)) &&
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

                // Recalculate transactions only if it has installments
                if (newTransaction.TotalInstallments > 0)
                    newTransaction.ValueBrl = GetTransactionOriginalTotalValue(oldTransaction); // Adjust new transaction’s value based on the original total.

                await DeleteAsync(oldTransaction); // Delete original transaction and its installments
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

        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessRecurringNotification(TransactionDto originalTransaction, Guid profileId)
        {
            var _originalTransaction = await GetAsync(originalTransaction.Id);
            if (_originalTransaction == null || _originalTransaction.Frequency == null)
                return;

            var tipo = _originalTransaction.Type == TransactionType.EXPENSE ? "pagamento" : "recebimento";

            var notification = await notificationService.GetAsync(originalTransaction.Id, profileId);
            if (notification is not null && notification.IsEnabled)
            {
                // Schedule notifications to alert the user when a goal's limit is reached
                await notificationService.SendNotificationByProfileAsync(
                    profileId,
                    $"Seu próximo {tipo} está próximo!",
                    $" Fique atento! O {tipo}: {originalTransaction.Title.Trim()} está agendado para amanhã");
            }
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
                if (transaction.Notification is not null)
                    context.Notifications.Remove(transaction.Notification);
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

        private void ScheduleRecurringTransaction(TransactionDto transaction)
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
            catch
            {
                throw;
            }
        }

        private void ScheduleNotification(TransactionDto transaction, Guid profileId)
        {
            try
            {
                var id = transaction.ParentTransactionId ?? transaction.Id;
                string jobId = $"Notification-{id}";
                string cronExpression = string.Empty;
                DateTimeOffset notificationDate = DateTime.Now;
                switch (transaction.Frequency)
                {
                    case RecurrenceType.WEEKLY:
                        int transactionDayOfWeek = (int)transaction.OccurredAt.DayOfWeek;
                        int notificationDayOfWeek = (transactionDayOfWeek + 6) % 7;
                        cronExpression = $"0 0 * * {notificationDayOfWeek}";

                        // Calcular a próxima data da notificação
                        notificationDate = transaction.OccurredAt.AddDays(-1);
                        break;

                    case RecurrenceType.MONTHLY:
                        int transactionDay = transaction.OccurredAt.Day;
                        int notificationDay = transactionDay - 1;
                        if (notificationDay < 1)
                        {
                            notificationDay = 28;
                        }

                        cronExpression = $"0 0 {notificationDay} * *";

                        // Calcular a data de notificação para o mês da transação
                        notificationDate = new DateTimeOffset(transaction.OccurredAt.Year, transaction.OccurredAt.Month, notificationDay, 0, 0, 0, TimeSpan.Zero);
                        break;

                    case RecurrenceType.YEARLY:
                        notificationDate = transaction.OccurredAt.AddDays(-1);
                        int day = notificationDate.Day;
                        int month = notificationDate.Month;
                        cronExpression = $"0 0 {day} {month} *";
                        break;
                }

                context.Notifications.Add(new Notification
                {
                    TransactionId = id,
                    ProfileId = profileId,
                    TriggerDate = notificationDate
                });

                if (!string.IsNullOrEmpty(cronExpression))
                {
                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => ProcessRecurringNotification(transaction, profileId),
                        cronExpression);
                }
            }
            catch
            {
                throw;
            }
        }

        private static void RemoveScheduledJobs(Guid transactionId)
        {
            string transactionJobId = $"RecurringTransaction-{transactionId}";
            string notificationJobId = $"Notification-{transactionId}";

            RecurringJob.RemoveIfExists(transactionJobId);
            RecurringJob.RemoveIfExists(notificationJobId);
        }

        private void ScheduleRecurringIfNeeded(TransactionDto transaction, Guid profileId)
        {
            if (transaction.Frequency != null)
            {
                ScheduleRecurringTransaction(transaction);
                ScheduleNotification(transaction, profileId);
            }
        }

        #endregion

        #region Transaction Processing & Helpers

        private async Task<Entities.Transaction> ProcessSingleTransactionAsync(TransactionDto request, Guid profileId, int installment, Guid? parentId = null, bool scheduleRecurrence = true)
        {
            try
            {

                Wallet wallet = await walletService.CreateOrUpdateAsync(request, profileId);
                Entities.Transaction newTransaction = BuildTransaction(request, wallet, installment, parentId);

                // Notify user of goals reaching its limit
                if (newTransaction.LabelId is not null)
                {
                    var budgetGoal = await context.BudgetGoals.FirstOrDefaultAsync(bg => bg.LabelId == newTransaction.LabelId);
                    if (budgetGoal is not null)
                    {
                        var transactions = await GetAllByMonthAndYearLabelAsync(newTransaction.OccurredAt.Month, newTransaction.OccurredAt.Year, newTransaction.LabelId.Value, profileId, TransactionType.EXPENSE);
                        var totalLabelExpenses = transactions.Sum(t => t.ValueBrl) + request.ValueBrl;
                        var totalMonthExpenses = wallet.ExpensesBrl;

                        decimal limit = 0;
                        if (budgetGoal.Type == BudgetGoalType.PERCENTAGE)
                            limit = (budgetGoal.Value / 100) * totalMonthExpenses;
                        else
                            limit = budgetGoal.Value;

                        // if the label’s total expenses reach 80% of the goal limit.
                        var limitThreshold = limit * 0.8M;
                        if (totalLabelExpenses >= limitThreshold)
                            RecurringJob.TriggerJob($"Notification-{budgetGoal.Id}");
                    }
                }
                context.Transactions.Add(newTransaction);
                request.ParentTransactionId = newTransaction.ParentTransactionId;
                request.Id = newTransaction.Id;
                if (scheduleRecurrence)
                    ScheduleRecurringIfNeeded(request, profileId);
                return newTransaction;
            }
            catch (Exception ex)
            {
                throw;
            }
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
