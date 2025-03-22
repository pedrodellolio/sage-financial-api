using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class RecurringTransactionService(AppDbContext context)
    {
        private static DateTimeOffset CalculateNextDueDate(DateTimeOffset occurredAt, RecurrenceType frequency)
        {
            return frequency switch
            {
                RecurrenceType.WEEKLY => occurredAt.AddDays(7),
                RecurrenceType.BIWEEKLY => occurredAt.AddDays(14),
                RecurrenceType.MONTHLY => occurredAt.AddMonths(1),
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), "Unsupported frequency type.")
            };
        }

        public void ScheduleRecurringTransaction(Transaction transaction)
        {
            string jobId = $"RecurringTransaction-{transaction.Id}";
            int dayOfWeek = ((int)transaction.OccurredAt.DayOfWeek + 6) % 7 + 1;
            string cronExpression = transaction.Frequency switch
            {
                RecurrenceType.WEEKLY => $"0 0 * * {dayOfWeek}",
                // RecurrenceType.BIWEEKLY => "0 0 * * 5 [ $(expr $(date +\%U) \% 2) -eq 0 ] && /path/to/command",
                RecurrenceType.MONTHLY => $"0 0 {transaction.OccurredAt.Day} * *",
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

        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessRecurringTransaction(Guid originalTransactionId)
        {
            var originalTransaction = await context.Transactions.FindAsync(originalTransactionId);
            if (originalTransaction == null) return;
            if (originalTransaction.Frequency == null) return;

            var newTransaction = new Transaction
            {
                Title = originalTransaction.Title,
                ValueBrl = originalTransaction.ValueBrl,
                OccurredAt = CalculateNextDueDate(originalTransaction.OccurredAt, originalTransaction.Frequency.Value), // Set the new occurrence date
                Type = originalTransaction.Type,
                Frequency = originalTransaction.Frequency,
                WalletId = originalTransaction.WalletId,
                LabelId = originalTransaction.LabelId,
                ParentTransactionId = originalTransaction.Id,
                FileId = originalTransaction.FileId
            };

            context.Transactions.Add(newTransaction);
            await context.SaveChangesAsync();
            // ScheduleRecurringTransaction(newTransaction);
        }
    }
}