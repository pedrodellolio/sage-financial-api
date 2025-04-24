using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SageFinancialAPI.Data;
using SageFinancialAPI.Entities;
using SageFinancialAPI.Extensions;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Services
{
    public class LabelService(AppDbContext context) : ILabelService
    {
        public async Task<Label?> GetAsync(Guid labelId)
        {
            return await context.Labels.FindAsync(labelId);
        }

        public async Task<ICollection<LabelDto>> GetAllNotInBudgetGoalAsync(int month, int year, Guid profileId)
        {
            return await context.Labels
                .Where(label =>
                    label.BudgetGoals.Any(bg => bg.Budget.ProfileId == profileId) &&
                    !label.BudgetGoals.Any(bg =>
                        bg.Budget.ProfileId == profileId &&
                        bg.Budget.Month == month &&
                        bg.Budget.Year == year))
                .Select(label => label.ToDto())
                .ToListAsync();
        }



        public async Task<ICollection<LabelDto>> GetAllAsync(Guid profileId)
        {
            return await context.Labels
                .Include(l => l.Profile)
                .Where(l => l.ProfileId == profileId && l.IsActive)
                .Select(label => label.ToDto()).ToListAsync();
        }

        public async Task<Label> PostAsync(LabelDto request, Guid profileId)
        {
            var newLabel = new Label
            {
                Title = request.Title,
                ColorHex = "#FFF",
                ProfileId = profileId
            };

            context.Labels.Add(newLabel);
            await context.SaveChangesAsync();
            return newLabel;
        }

        public async Task<Label> PutAsync(Label label)
        {
            context.Labels.Update(label);
            await context.SaveChangesAsync();
            return label;
        }

        public async Task<bool> DeleteAsync(Label label)
        {
            context.Labels.Remove(label);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}